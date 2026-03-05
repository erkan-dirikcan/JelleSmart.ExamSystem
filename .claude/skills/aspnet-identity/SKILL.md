---
name: aspnet-identity
description: ASP.NET Core Identity expert for .NET 10. Complete authentication and authorization system setup including JWT tokens, Identity pages, user management, role/claim authorization, MFA/2FA, external providers, and security policies. Use this skill when user requests: (1) "identity kur" or "setup identity" - creates full Identity system with all pages, APIs, and services in one go, (2) Identity page creation (Login, Register, Forgot Password, Reset Password, Profile, Users, Roles), (3) JWT authentication setup, (4) Role/Claim/Policy authorization, (5) MFA/2FA configuration, (6) External login providers (Google, Facebook). Works with Koala CRM project structure using EF Core 10, MediatR, FluentValidation, AutoMapper, and Serilog.
---

# ASP.NET Identity Expert for .NET 10

Complete authentication and authorization system for Koala CRM using ASP.NET Core Identity EF Core 10 with JWT Bearer tokens.

## Tech Stack

- .NET 10
- Entity Framework Core 10
- ASP.NET Identity EF Core 10
- JWT Bearer Authentication
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Serilog
- BCrypt.Net-Next

## Project Structure Integration

```
Koala.Core.Domain/
├── Entities/
│   ├── AppUser.cs          : IdentityUser with extensions
│   ├── AppRole.cs          : IdentityRole with extensions
│   ├── AppUserRole.cs      : Join table
│   ├── AppUserClaim.cs     : User claims
│   ├── AppRoleClaim.cs     : Role claims
│   └── AppUserToken.cs     : Refresh tokens
│
Koala.Core.Application/
├── DTOs/
│   ├── Auth/
│   │   ├── LoginDto.cs
│   │   ├── RegisterDto.cs
│   │   ├── ForgotPasswordDto.cs
│   │   ├── ResetPasswordDto.cs
│   │   ├── ChangePasswordDto.cs
│   │   ├── RefreshTokenDto.cs
│   │   └── LoginResponseDto.cs
│   ├── User/
│   │   ├── UserDto.cs
│   │   ├── CreateUserDto.cs
│   │   ├── UpdateUserDto.cs
│   │   └── UserProfileDto.cs
│   └── Role/
│       ├── RoleDto.cs
│       └── CreateRoleDto.cs
├── Validators/            : FluentValidation validators
├── Mappings/              : AutoMapper profiles
├── Commands/              : CQRS commands
├── Queries/               : CQRS queries
├── Services/
│   ├── IAuthService.cs
│   ├── IUserService.cs
│   ├── IRoleService.cs
│   ├── IClaimService.cs
│   └── ITokenService.cs
│
Koala.Core.Infrastructure/
├── Identity/
│   ├── PasswordHasher.cs
│   ├── JwtTokenGenerator.cs
│   └── EmailService.cs
│
Koala.Core.Persistence/
├── Data/
│   └── KoalaDbContext.cs   : With Identity configuration
│
Koala.WebAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   └── RolesController.cs
│
Koala.WebUI/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   └── RolesController.cs
├── Views/
│   ├── Auth/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── ForgotPassword.cshtml
│   │   ├── ResetPassword.cshtml
│   │   └── ConfirmEmail.cshtml
│   ├── User/
│   │   ├── Profile.cshtml
│   │   ├── ChangePassword.cshtml
│   │   ├── Index.cshtml (User list)
│   │   ├── Create.cshtml
│   │   └── Edit.cshtml
│   └── Role/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       └── Edit.cshtml
```

## Quick Start Commands

### "identity kur" - Full Setup

When user says "identity kur" or "setup identity", create EVERYTHING in ONE response:

1. **Domain Layer**: AppUser, AppRole entities
2. **DbContext**: Add Identity configuration to KoalaDbContext
3. **DTOs**: All Auth, User, Role DTOs
4. **Validators**: FluentValidation validators
5. **Mappings**: AutoMapper profiles
6. **Commands/Queries**: All CQRS handlers
7. **Services**: All service interfaces and implementations
8. **API Controllers**: All WebAPI controllers
9. **MVC Controllers & Views**: All WebUI pages
10. **JWT Configuration**: Program.cs setup
11. **Seed Data**: Default admin user and roles
12. **Email Service**: Password reset email setup

See [IDENTITY-SETUP.md](references/IDENTITY-SETUP.md) for complete step-by-step.

### Individual Commands

| Command | Creates |
|---------|---------|
| "login sayfası" | Login page + controller + API |
| "register sayfası" | Register page + email confirmation |
| "şifremi unuttum" | Forgot/Reset password flow |
| "kullanıcı yönetimi" | User CRUD pages + API |
| "rol yönetimi" | Role management + authorization |
| "JWT kur" | JWT configuration + token service |
| "MFA kur" | Multi-factor authentication setup |
| "external login" | Google/Facebook OAuth setup |

## API Endpoints

See [API-ENDPOINTS.md](references/API-ENDPOINTS.md) for complete API documentation.

### Auth Endpoints

```
POST   /api/auth/login              - JWT token generation
POST   /api/auth/register           - User registration
POST   /api/auth/logout             - Logout (invalidate token)
POST   /api/auth/refresh-token      - Refresh JWT token
POST   /api/auth/forgot-password    - Send reset email
POST   /api/auth/reset-password     - Reset password with token
POST   /api/auth/confirm-email      - Confirm email with token
POST   /api/auth/change-password    - Change password (authenticated)
GET    /api/auth/profile            - Get current user profile
PUT    /api/auth/profile            - Update current user profile
```

### User Management

```
GET    /api/users                   - Get all users (paged)
GET    /api/users/{id}              - Get user by id
POST   /api/users                   - Create new user
PUT    /api/users/{id}              - Update user
DELETE /api/users/{id}              - Delete user (soft delete)
POST   /api/users/{id}/roles        - Assign roles to user
DELETE /api/users/{id}/roles/{roleId} - Remove role from user
POST   /api/users/{id}/claims       - Assign claims to user
DELETE /api/users/{id}/claims/{claimType} - Remove claim from user
POST   /api/users/{id}/lock         - Lock user
POST   /api/users/{id}/unlock       - Unlock user
```

### Role Management

```
GET    /api/roles                   - Get all roles
GET    /api/roles/{id}              - Get role by id
POST   /api/roles                   - Create new role
PUT    /api/roles/{id}              - Update role
DELETE /api/roles/{id}              - Delete role
GET    /api/roles/{id}/users        - Get users in role
GET    /api/roles/{id}/claims       - Get role claims
POST   /api/roles/{id}/claims       - Add claims to role
```

## Services

### IAuthService

```csharp
interface IAuthService
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
```

### IUserService

```csharp
interface IUserService
{
    Task<PagedDto<UserDto>> GetUsersAsync(int page, int pageSize);
    Task<UserDto> GetUserByIdAsync(string id);
    Task<UserDto> CreateUserAsync(CreateUserDto request);
    Task<UserDto> UpdateUserAsync(string id, UpdateUserDto request);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> AssignRolesAsync(string userId, List<string> roleNames);
    Task<bool> AssignClaimsAsync(string userId, List<ClaimDto> claims);
    Task<bool> LockUserAsync(string userId, int lockoutMinutes);
    Task<bool> UnlockUserAsync(string userId);
}
```

### IRoleService

```csharp
interface IRoleService
{
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<RoleDto> GetRoleByIdAsync(string id);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto request);
    Task<RoleDto> UpdateRoleAsync(string id, CreateRoleDto request);
    Task<bool> DeleteRoleAsync(string id);
    Task<bool> AssignClaimsToRoleAsync(string roleId, List<ClaimDto> claims);
}
```

### ITokenService

```csharp
interface ITokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles, IList<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GetUserIdFromToken(string token);
}
```

## Security Configuration

### JWT Settings (appsettings.json)

```json
{
  "JwtSettings": {
    "Secret": "YOUR-SECRET-KEY-MIN-32-CHARS",
    "Issuer": "KoalaCRM",
    "Audience": "KoalaCRMUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Identity Options

```csharp
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Signin settings
    options.SignIn.RequireConfirmedEmail = true;
});
```

See [SECURITY.md](references/SECURITY.md) for complete security configuration.

## Authorization Patterns

### Role-Based Authorization

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // Only admins can access
}
```

### Claim-Based Authorization

```csharp
[Authorize(Policy = "ManageUsersPolicy")]
public class UsersController : ControllerBase
{
    // Users with "ManageUsers" claim can access
}
```

### Policy-Based Authorization

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageUsersPolicy", policy =>
        policy.RequireClaim("Permission", "ManageUsers")
               .RequireRole("Admin"));

    options.AddPolicy="EditOwnProfilePolicy", policy =>
        policy.RequireClaim("UserId"));
});
```

## MFA / 2FA Setup

See [MFA-2FA.md](references/MFA-2FA.md) for complete MFA configuration.

### Authenticator App Setup

```csharp
// Enable 2FA for user
await _userManager.SetTwoFactorEnabledAsync(user, true);

// Generate QR code URI
var qrCodeUri = await _userManager.GetAuthenticatorKeyAsync(user);
```

## External Login Providers

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = configuration["Authentication:Google:ClientId"];
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = configuration["Authentication:Facebook:AppId"];
        options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
    });
```

## Email Templates

Place email templates in `assets/templates/email/`:

- `confirm-email.html`
- `forgot-password.html`
- `lockout-notification.html`

## References

- [IDENTITY-SETUP.md](references/IDENTITY-SETUP.md) - Complete setup guide
- [API-ENDPOINTS.md](references/API-ENDPOINTS.md) - Full API documentation
- [SERVICES.md](references/SERVICES.md) - Service implementations
- [SECURITY.md](references/SECURITY.md) - Security configuration
- [MFA-2FA.md](references/MFA-2FA.md) - Multi-factor authentication
- [EXTERNAL-LOGIN.md](references/EXTERNAL-LOGIN.md) - OAuth providers
