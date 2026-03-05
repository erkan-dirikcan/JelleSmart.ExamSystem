# ASP.NET Identity Complete Setup Guide

This guide creates the entire Identity system for Koala CRM in ONE execution when user says "identity kur".

## Phase 1: Domain Layer - Entities

### 1.1 AppUser Entity

Create `Koala.Core.Domain/Entities/Identity/AppUser.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Koala.Core.Domain.Entities.Identity;

public class AppUser : IdentityUser
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(256)]
    public string? AvatarUrl { get; set; }

    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public string? DepartmentId { get; set; }
    public string? ManagerId { get; set; }

    // Navigation properties
    public virtual AppUser? Manager { get; set; }
    public virtual ICollection<AppUser> Subordinates { get; set; } = new List<AppUser>();
    public virtual ICollection<MT_Firm> Firms { get; set; } = new List<MT_Firm>();
    public virtual ICollection<MT_Contact> Contacts { get; set; } = new List<MT_Contact>();
}
```

### 1.2 AppRole Entity

Create `Koala.Core.Domain/Entities/Identity/AppRole.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Koala.Core.Domain.Entities.Identity;

public class AppRole : IdentityRole
{
    [MaxLength(200)]
    public string? Description { get; set; }

    public bool IsSystem { get; set; } = false;
    public int DisplayOrder { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### 1.3 Identity Join Entities (if needed for custom properties)

```csharp
// Koala.Core.Domain/Entities/Identity/AppUserRole.cs
using Microsoft.AspNetCore.Identity;

namespace Koala.Core.Domain.Entities.Identity;

public class AppUserRole : IdentityUserRole<string>
{
    public DateTime? AssignedAt { get; set; } = DateTime.UtcNow;
    public string? AssignedBy { get; set; }
}

// Koala.Core.Domain/Entities/Identity/AppUserClaim.cs
using Microsoft.AspNetCore.Identity;

namespace Koala.Core.Domain.Entities.Identity;

public class AppUserClaim : IdentityUserClaim<string>
{
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}

// Koala.Core.Domain/Entities/Identity/AppRoleClaim.cs
using Microsoft.AspNetCore.Identity;

namespace Koala.Core.Domain.Entities.Identity;

public class AppRoleClaim : IdentityRoleClaim<string>
{
    public string? Description { get; set; }
}
```

### 1.4 Refresh Token Entity

```csharp
// Koala.Core.Domain/Entities/Identity/AppRefreshToken.cs
namespace Koala.Core.Domain.Entities.Identity;

public class AppRefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
}
```

## Phase 2: Persistence Layer - DbContext

### 2.1 Update KoalaDbContext

Update `Koala.Core.Persistence/Data/KoalaDbContext.cs`:

```csharp
using Koala.Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Koala.Core.Persistence.Data;

public class KoalaDbContext : IdentityDbContext<
    AppUser,
    AppRole,
    string,
    IdentityUserClaim<string>,
    AppUserRole,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>>
{
    public KoalaDbContext(DbContextOptions<KoalaDbContext> options) : base(options) { }

    public DbSet<AppRefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AppUser configuration
        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("ST_User");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.AvatarUrl).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.DepartmentId);
            entity.HasIndex(e => e.ManagerId);

            // Self-reference for manager
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.Subordinates)
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AppRole configuration
        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("ST_Role");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsSystem).HasDefaultValue(false);
            entity.HasIndex(e => e.DisplayOrder);
        });

        // AppUserRole configuration
        builder.Entity<AppUserRole>(entity =>
        {
            entity.ToTable("RL_User_Role");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // AppUserClaim configuration
        builder.Entity<AppUserClaim>(entity =>
        {
            entity.ToTable("ST_User_Claim");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // AppRoleClaim configuration
        builder.Entity<AppRoleClaim>(entity =>
        {
            entity.ToTable("ST_Role_Claim");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // RefreshToken configuration
        builder.Entity<AppRefreshToken>(entity =>
        {
            entity.ToTable("ST_Refresh_Token");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
```

## Phase 3: Application Layer - DTOs

### 3.1 Auth DTOs

Create `Koala.Core.Application/DTOs/Auth/`:

```csharp
// LoginDto.cs
using FluentValidation;

namespace Koala.Core.Application.DTOs.Auth;

public class LoginDto
{
    public string EmailOrUserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .NotEmpty().WithMessage("Email or username is required")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

// RegisterDto.cs
public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain uppercase, lowercase, number and special character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100);
    }
}

// ForgotPasswordDto.cs
public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

// ResetPasswordDto.cs
public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

// ChangePasswordDto.cs
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

// RefreshTokenDto.cs
public class RefreshTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

// LoginResponseDto.cs
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UserDto User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
```

### 3.2 User DTOs

```csharp
// UserDto.cs
namespace Koala.Core.Application.DTOs.User;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<ClaimDto> Claims { get; set; } = new();
}

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DepartmentId { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public string? DepartmentId { get; set; }
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
```

### 3.3 Role DTOs

```csharp
// RoleDto.cs
namespace Koala.Core.Application.DTOs.Role;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<ClaimDto> Claims { get; set; } = new();
}

public class ClaimDto
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
```

## Phase 4: Application Layer - AutoMapper Profiles

```csharp
// Koala.Core.Application/Mappings/IdentityMappingProfile.cs
using AutoMapper;
using Koala.Core.Application.DTOs.Auth;
using Koala.Core.Application.DTOs.Role;
using Koala.Core.Application.DTOs.User;
using Koala.Core.Domain.Entities.Identity;

namespace Koala.Core.Application.Mappings;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        // AppUser mappings
        CreateMap<AppUser, UserDto>();
        CreateMap<AppUser, UserProfileDto>();
        CreateMap<CreateUserDto, AppUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));
        CreateMap<UpdateUserDto, AppUser>();

        // AppRole mappings
        CreateMap<AppRole, RoleDto>();
        CreateMap<CreateRoleDto, AppRole>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()));

        // Claim mappings
        CreateMap<ClaimDto, AppUserClaim>()
            .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ClaimValue, opt => opt.MapFrom(src => src.Value));
    }
}
```

## Phase 5: Infrastructure Layer - JWT Service

```csharp
// Koala.Core.Infrastructure/Identity/JwtTokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Koala.Core.Application.DTOs.Auth;
using Koala.Core.Application.Services;
using Koala.Core.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Koala.Core.Infrastructure.Identity;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(AppUser user, IList<string> roles, IList<Claim> additionalClaims)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new Claim("FirstName", user.FirstName ?? string.Empty),
            new Claim("LastName", user.LastName ?? string.Empty)
        };

        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add additional claims
        claims.AddRange(additionalClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }

    public string GetUserIdFromToken(string token)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        return principal?.FindClaim(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
    }
}
```

## Phase 6: Application Layer - Services

```csharp
// Koala.Core.Application/Services/IAuthService.cs
using Koala.Core.Application.DTOs.Auth;

namespace Koala.Core.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto request);
    Task<bool> RegisterAsync(RegisterDto request);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordDto request);
    Task<bool> ChangePasswordAsync(ChangePasswordDto request, string userId);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto request);
    Task LogoutAsync(string userId);
}

// Koala.Core.Application/Services/AuthService.cs
using Koala.Core.Application.DTOs.Auth;
using Koala.Core.Application.DTOs.User;
using Koala.Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Koala.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.EmailOrUserName) ??
                  await _userManager.FindByNameAsync(request.EmailOrUserName);

        if (user == null || !user.IsActive)
            throw new Exception("Invalid credentials");

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                throw new Exception("Account is locked out");
            throw new Exception("Invalid credentials");
        }

        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user, roles.ToList(), claims.ToList());
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new LoginResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt ?? DateTime.UtcNow,
                Roles = roles.ToList()
            },
            Roles = roles.ToList(),
            Permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList()
        };
    }

    // Implement other methods similarly...
}
```

## Phase 7: Program.cs Configuration

```csharp
// Koala.WebAPI/Program.cs
using Koala.Core.Domain.Entities.Identity;
using Koala.Core.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Koala.Core.Persistence.Data;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// DbContext
builder.Services.AddDbContext<KoalaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<KoalaDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

// Services
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Phase 8: Seed Data

```csharp
// Koala.Core.Persistence/SeedData/IdentitySeedData.cs
using Koala.Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Koala.Core.Persistence.SeedData;

public static class IdentitySeedData
{
    public static async Task SeedAsync(KoalaDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Admin User
        await SeedAdminUserAsync(userManager, roleManager);
    }

    private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
    {
        var roles = new[]
        {
            new AppRole { Name = "Admin", Description = "System Administrator", IsSystem = true, DisplayOrder = 1 },
            new AppRole { Name = "Manager", Description = "Department Manager", DisplayOrder = 2 },
            new AppRole { Name = "User", Description = "Standard User", DisplayOrder = 3 },
            new AppRole { Name = "Sales", Description = "Sales Representative", DisplayOrder = 4 }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        var adminUser = await userManager.FindByEmailAsync("admin@koalacrm.com");

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = "admin@koalacrm.com",
                Email = "admin@koalacrm.com",
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@12345");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
```

This complete setup will create:
- 4 Entity files
- 1 DbContext update
- 15+ DTO files with validators
- 1 AutoMapper profile
- 1 JWT service
- 4 Service interfaces
- 4 Service implementations
- Program.cs configuration
- Seed data
