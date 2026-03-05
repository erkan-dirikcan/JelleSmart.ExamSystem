# ASP.NET Identity Security Configuration

Complete security setup for JWT, Identity options, and authorization policies.

## appsettings.json Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=KoalaCrmDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "Secret": "YOUR-SECRET-KEY-MUST-BE-AT-LEAST-32-CHARACTERS-LONG-CHANGE-IN-PRODUCTION",
    "Issuer": "KoalaCRM",
    "Audience": "KoalaCRMUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "UseSSL": true,
    "FromEmail": "noreply@koalacrm.com",
    "FromName": "Koala CRM",
    "Username": "your-email@example.com",
    "Password": "your-password"
  },
  "IdentityOptions": {
    "Password": {
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": true,
      "RequiredLength": 12,
      "RequiredUniqueChars": 1
    },
    "Lockout": {
      "AllowedForNewUsers": true,
      "DefaultLockoutTimeSpan": "00:05:00",
      "MaxFailedAccessAttempts": 5
    },
    "User": {
      "RequireUniqueEmail": true
    },
    "SignIn": {
      "RequireConfirmedEmail": true,
      "RequireConfirmedPhoneNumber": false
    }
  }
}
```

## Program.cs - Complete Identity Configuration

```csharp
using Koala.Core.Domain.Entities.Identity;
using Koala.Core.Infrastructure.Identity;
using Koala.Core.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Koala.Core.Persistence.Data;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext());

// DbContext
builder.Services.AddDbContext<KoalaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// Identity Configuration
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Password Settings
    options.Password.RequireDigit = builder.Configuration.GetValue<bool>("IdentityOptions:Password:RequireDigit", true);
    options.Password.RequireLowercase = builder.Configuration.GetValue<bool>("IdentityOptions:Password:RequireLowercase", true);
    options.Password.RequireUppercase = builder.Configuration.GetValue<bool>("IdentityOptions:Password:RequireUppercase", true);
    options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("IdentityOptions:Password:RequireNonAlphanumeric", true);
    options.Password.RequiredLength = builder.Configuration.GetValue<int>("IdentityOptions:Password:RequiredLength", 12);
    options.Password.RequiredUniqueChars = builder.Configuration.GetValue<int>("IdentityOptions:Password:RequiredUniqueChars", 1);

    // Lockout Settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.Parse(builder.Configuration["IdentityOptions:Lockout:DefaultLockoutTimeSpan"] ?? "00:05:00");
    options.Lockout.MaxFailedAccessAttempts = builder.Configuration.GetValue<int>("IdentityOptions:Lockout:MaxFailedAccessAttempts", 5);
    options.Lockout.AllowedForNewUsers = builder.Configuration.GetValue<bool>("IdentityOptions:Lockout:AllowedForNewUsers", true);

    // User Settings
    options.User.RequireUniqueEmail = builder.Configuration.GetValue<bool>("IdentityOptions:User:RequireUniqueEmail", true);

    // SignIn Settings
    options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedEmail", true);
    options.SignIn.RequireConfirmedPhoneNumber = builder.Configuration.GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedPhoneNumber", false);
})
.AddEntityFrameworkStores<KoalaDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<KoalaIdentityErrorDescriber>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero,
        LifetimeValidator = (token, notBefore, expires, validationParameters, validationDelegate) =>
        {
            // Additional validation logic if needed
            return expires != null && expires > DateTime.UtcNow;
        }
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var tokenValidator = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
            var userId = tokenValidator.GetUserIdFromToken(context.SecurityToken as string);

            if (!string.IsNullOrEmpty(userId))
            {
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                var user = userManager.FindByIdAsync(userId).Result;

                if (user == null || !user.IsActive)
                {
                    context.Fail("User is not active or doesn't exist");
                }
            }
            return Task.CompletedTask;
        }
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Admin only
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    // Managers and above
    options.AddPolicy("ManagerOrAbove", policy =>
        policy.RequireRole("Admin", "Manager"));

    // Can manage users
    options.AddPolicy("ManageUsersPolicy", policy =>
        policy.RequireClaim("Permission", "ManageUsers")
               .RequireRole("Admin", "Manager"));

    // Can manage roles (Admin only)
    options.AddPolicy("ManageRolesPolicy", policy =>
        policy.RequireClaim("Permission", "ManageRoles")
               .RequireRole("Admin"));

    // Can view own profile
    options.AddPolicy("EditOwnProfilePolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "UserId" && c.Value == context.Resource as string)));

    // Sales users
    options.AddPolicy("SalesUsers", policy =>
        policy.RequireRole("Admin", "Manager", "Sales"));

    // Can manage firms
    options.AddPolicy("ManageFirmsPolicy", policy =>
        policy.RequireClaim("Permission", "ManageFirms"));

    // Can view reports
    options.AddPolicy("ViewReportsPolicy", policy =>
        policy.RequireClaim("Permission", "ViewReports"));
});

// Application Services
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<KoalaDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    // Apply pending migrations
    context.Database.Migrate();

    // Seed identity data
    await IdentitySeedData.SeedAsync(context, userManager, roleManager);
}

app.Run();
```

## Custom Error Describer

```csharp
// Koala.Core.Infrastructure/Identity/KoalaIdentityErrorDescriber.cs
using Microsoft.AspNetCore.Identity;

namespace Koala.Core.Infrastructure.Identity;

public class KoalaIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"E-posta adresi '{email}' zaten kullanımda."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"Kullanıcı adı '{userName}' zaten kullanımda."
        };
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = $"E-posta adresi '{email}' geçersiz."
        };
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateRoleName),
            Description = $"Rol adı '{role}' zaten kullanımda."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Şifre en az {length} karakter olmalıdır."
        };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"Şifre en az {uniqueChars} benzersiz karakter içermelidir."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Şifre en az bir özel karakter (@$!%*?& vb.) içermelidir."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Şifre en az bir rakam (0-9) içermelidir."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Şifre en az bir küçük harf içermelidir."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Şifre en az bir büyük harf içermelidir."
        };
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError
        {
            Code = nameof(InvalidToken),
            Description = "Geçersiz veya süresi dolmuş token."
        };
    }

    public override IdentityError LoginAlreadyAssociated()
    {
        return new IdentityError
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = "Bu kullanıcı hesabı başka bir giriş ile zaten ilişkilendirilmiş."
        };
    }

    public override IdentityError InvalidUserName(string? userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = $"Kullanıcı adı '{userName}' geçersiz. Sadece harf ve rakam kullanılabilir."
        };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"Kullanıcı zaten '{role}' rolüne sahip."
        };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserNotInRole),
            Description = $"Kullanıcı '{role}' rolüne sahip değil."
        };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = "Kullanıcı kilidi etkin değil."
        };
    }

    public override IdentityError InvalidRoleName(string? role)
    {
        return new IdentityError
        {
            Code = nameof(InvalidRoleName),
            Description = $"Rol adı '{role}' geçersiz."
        };
    }
}
```

## Security Stamp Validation

Security Stamp is used for sign-out everywhere when user credentials change:

```csharp
// In Program.cs, after AddIdentity():
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // This controls how often the security stamp is validated
    options.ValidationInterval = TimeSpan.FromMinutes(5);

    // On failure, redirect to login
    options.OnRefreshingPrincipal = principalContext =>
    {
        var newId = principalContext.NewPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var newStamp = principalContext.NewPrincipal.FindFirstValue("AspNet.Identity.SecurityStamp");

        return Task.CompletedTask;
    };
});
```

## Password History Validation

```csharp
// Koala.Core.Infrastructure/Identity/PasswordHistoryValidator.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Koala.Core.Infrastructure.Identity;

public class PasswordHistoryValidator
{
    private readonly KoalaDbContext _context;
    private const int PasswordHistoryCount = 5; // Last 5 passwords

    public PasswordHistoryValidator(KoalaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsPasswordInHistoryAsync(string userId, string newPasswordHash)
    {
        // Get recent password hashes (would need to store password history)
        var recentPasswords = await _context.AppUserTokens
            .Where(t => t.UserId == userId && t.LoginProvider == "PasswordHistory")
            .OrderByDescending(t => t.Id)
            .Take(PasswordHistoryCount)
            .Select(t => t.Value)
            .ToListAsync();

        return recentPasswords.Contains(newPasswordHash);
    }

    public async Task AddPasswordToHistoryAsync(string userId, string passwordHash)
    {
        var token = new IdentityUserToken<string>
        {
            UserId = userId,
            LoginProvider = "PasswordHistory",
            Name = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            Value = passwordHash
        };

        await _context.UserTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }
}
```

## API Key Authentication (Optional)

For API-only access without JWT:

```csharp
// Koala.Core.Domain/Entities/Identity/ApiKey.cs
namespace Koala.Core.Domain.Entities.Identity;

public class ApiKey
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Key { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }

    public AppUser User { get; set; }
}

// Middleware for API Key authentication
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
        {
            // Validate API key
            // Add user claims to context
        }

        await _next(context);
    }
}
```
