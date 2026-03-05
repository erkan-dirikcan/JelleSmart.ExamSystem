# ASP.NET Identity API Endpoints

Complete API endpoint documentation for Koala CRM Identity system.

## Controllers Structure

```
Koala.WebAPI/Controllers/
├── AuthController.cs       - Authentication endpoints
├── UsersController.cs       - User management endpoints
└── RolesController.cs      - Role management endpoints
```

## AuthController

### POST /api/auth/login

Login with email/username and password. Returns JWT access token and refresh token.

**Request:**
```json
{
  "emailOrUserName": "admin@koalacrm.com",
  "password": "Admin@123",
  "rememberMe": true
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyIsIGlzIGEgcmVmcmVzaCB0b2tlbg==",
  "expiration": "2025-02-22T15:30:00Z",
  "user": {
    "id": "d8f7e9c6-5b4a-4f8e-9c1d-2b3a4c5d6e7f",
    "email": "admin@koalacrm.com",
    "userName": "admin@koalacrm.com",
    "firstName": "System",
    "lastName": "Administrator",
    "avatarUrl": null,
    "isActive": true,
    "lastLoginDate": "2025-02-22T14:30:00Z",
    "createdAt": "2025-02-01T10:00:00Z",
    "roles": ["Admin"],
    "claims": []
  },
  "roles": ["Admin"],
  "permissions": ["ManageUsers", "ManageRoles"]
}
```

**Error Responses:**
- `401 Unauthorized` - Invalid credentials or account locked
- `400 Bad Request` - Validation failed

### POST /api/auth/register

Register a new user account. Sends confirmation email.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "NewUser@123",
  "confirmPassword": "NewUser@123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Registration successful. Please check your email to confirm your account."
}
```

### POST /api/auth/logout

Logout current user (invalidate refresh token).

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

### POST /api/auth/refresh-token

Refresh access token using refresh token.

**Request:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyIsIGlzIGEgcmVmcmVzaCB0b2tlbg=="
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "bmV3IHJlZnJlc2ggdG9rZW4=",
  "expiration": "2025-02-22T16:30:00Z",
  "user": { ... },
  "roles": ["User"],
  "permissions": []
}
```

### POST /api/auth/forgot-password

Initiate password reset. Sends reset email with token.

**Request:**
```json
{
  "email": "user@example.com"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "If an account exists with this email, a password reset link has been sent."
}
```

### POST /api/auth/reset-password

Reset password using token from email.

**Request:**
```json
{
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "newPassword": "NewPassword@123",
  "confirmPassword": "NewPassword@123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password has been reset successfully."
}
```

### POST /api/auth/confirm-email

Confirm user email with token.

**Request:**
```json
{
  "userId": "d8f7e9c6-5b4a-4f8e-9c1d-2b3a4c5d6e7f",
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Email confirmed successfully."
}
```

### POST /api/auth/change-password

Change password for authenticated user.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "currentPassword": "OldPassword@123",
  "newPassword": "NewPassword@123",
  "confirmPassword": "NewPassword@123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully."
}
```

### GET /api/auth/profile

Get current user profile.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": "d8f7e9c6-5b4a-4f8e-9c1d-2b3a4c5d6e7f",
  "email": "user@example.com",
  "userName": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "avatarUrl": "/uploads/avatars/user.jpg",
  "lastLoginDate": "2025-02-22T14:30:00Z"
}
```

### PUT /api/auth/profile

Update current user profile.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "avatarUrl": "/uploads/avatars/new-avatar.jpg"
}
```

**Response (200 OK):**
```json
{
  "id": "d8f7e9c6-5b4a-4f8e-9c1d-2b3a4c5d6e7f",
  "email": "user@example.com",
  "userName": "user@example.com",
  "firstName": "John",
  "lastName": "Smith",
  "avatarUrl": "/uploads/avatars/new-avatar.jpg"
}
```

## UsersController

### GET /api/users

Get all users with pagination.

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 20)
- `search` (optional - search in email, firstName, lastName)
- `isActive` (optional - filter by active status)
- `role` (optional - filter by role name)

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "d8f7e9c6-5b4a-4f8e-9c1d-2b3a4c5d6e7f",
      "email": "admin@koalacrm.com",
      "userName": "admin@koalacrm.com",
      "firstName": "System",
      "lastName": "Administrator",
      "avatarUrl": null,
      "isActive": true,
      "lastLoginDate": "2025-02-22T14:30:00Z",
      "createdAt": "2025-02-01T10:00:00Z",
      "roles": ["Admin"]
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20
}
```

### GET /api/users/{id}

Get user by ID.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": "d8f7e9c6-5b4a-4f8e-9c1d-2b3a4c5d6e7f",
  "email": "user@example.com",
  "userName": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "avatarUrl": "/uploads/avatars/user.jpg",
  "isActive": true,
  "lastLoginDate": "2025-02-22T14:30:00Z",
  "createdAt": "2025-02-01T10:00:00Z",
  "roles": ["User", "Sales"],
  "claims": [
    { "type": "Permission", "value": "ViewDashboard" },
    { "type": "Department", "value": "Sales" }
  ]
}
```

**Error Responses:**
- `404 Not Found` - User not found

### POST /api/users

Create a new user.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "email": "newuser@example.com",
  "userName": "newuser@example.com",
  "password": "NewUser@123",
  "firstName": "Jane",
  "lastName": "Smith",
  "departmentId": "dept-123",
  "roles": ["User"]
}
```

**Response (201 Created):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "email": "newuser@example.com",
  "userName": "newuser@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "isActive": true,
  "roles": ["User"]
}
```

### PUT /api/users/{id}

Update user details.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "firstName": "Jane",
  "lastName": "Johnson",
  "avatarUrl": "/uploads/avatars/jane.jpg",
  "isActive": true,
  "departmentId": "dept-456"
}
```

**Response (200 OK):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "email": "newuser@example.com",
  "userName": "newuser@example.com",
  "firstName": "Jane",
  "lastName": "Johnson",
  "avatarUrl": "/uploads/avatars/jane.jpg",
  "isActive": true
}
```

### DELETE /api/users/{id}

Soft delete user (sets IsDeleted = true).

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "success": true,
  "message": "User deleted successfully"
}
```

### POST /api/users/{id}/roles

Assign roles to user.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "roles": ["Admin", "Manager"]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Roles assigned successfully",
  "roles": ["Admin", "Manager"]
}
```

### DELETE /api/users/{id}/roles/{roleId}

Remove specific role from user.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Role removed successfully"
}
```

### POST /api/users/{id}/claims

Assign claims to user.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "claims": [
    { "type": "Permission", "value": "ManageUsers" },
    { "type": "Department", "value": "Sales" }
  ]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Claims assigned successfully"
}
```

### DELETE /api/users/{id}/claims/{claimType}

Remove specific claim from user.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Claim removed successfully"
}
```

### POST /api/users/{id}/lock

Lock user account for specified minutes.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "lockoutMinutes": 30
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "User locked for 30 minutes"
}
```

### POST /api/users/{id}/unlock

Unlock user account.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "success": true,
  "message": "User unlocked successfully"
}
```

## RolesController

### GET /api/roles

Get all roles.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
[
  {
    "id": "role-1-id",
    "name": "Admin",
    "description": "System Administrator",
    "isSystem": true,
    "displayOrder": 1
  },
  {
    "id": "role-2-id",
    "name": "Manager",
    "description": "Department Manager",
    "isSystem": false,
    "displayOrder": 2
  }
]
```

### GET /api/roles/{id}

Get role details with claims and users.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": "role-1-id",
  "name": "Admin",
  "description": "System Administrator",
  "isSystem": true,
  "displayOrder": 1,
  "claims": [
    { "type": "Permission", "value": "ManageUsers" },
    { "type": "Permission", "value": "ManageRoles" }
  ],
  "users": [
    { "id": "user-1", "email": "admin@koalacrm.com" }
  ]
}
```

### POST /api/roles

Create a new role.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "name": "SalesManager",
  "description": "Sales Department Manager",
  "claims": [
    { "type": "Permission", "value": "ViewReports" }
  ]
}
```

**Response (201 Created):**
```json
{
  "id": "new-role-id",
  "name": "SalesManager",
  "description": "Sales Department Manager",
  "isSystem": false
}
```

### PUT /api/roles/{id}

Update role details.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "name": "SalesManager",
  "description": "Updated description"
}
```

**Response (200 OK):**
```json
{
  "id": "role-1-id",
  "name": "SalesManager",
  "description": "Updated description",
  "isSystem": false
}
```

### DELETE /api/roles/{id}

Delete role (if not system role).

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Role deleted successfully"
}
```

**Error Response:**
- `400 Bad Request` - Cannot delete system role

### GET /api/roles/{id}/users

Get all users in a specific role.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
[
  {
    "id": "user-1",
    "email": "admin@koalacrm.com",
    "firstName": "System",
    "lastName": "Administrator"
  }
]
```

### GET /api/roles/{id}/claims

Get all claims for a role.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
[
  { "type": "Permission", "value": "ManageUsers" },
  { "type": "Permission", "value": "ManageRoles" }
]
```

### POST /api/roles/{id}/claims

Add claims to a role.

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "claims": [
    { "type": "Permission", "value": "ViewDashboard" }
  ]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Claims added to role successfully"
}
```

## Controller Implementation Template

```csharp
// Koala.WebAPI/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Koala.Core.Application.DTOs.Auth;
using Koala.Core.Application.Services;

namespace Koala.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var success = await _authService.RegisterAsync(request);
        return Ok(new { success, message = "Registration successful. Please confirm your email." });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _authService.LogoutAsync(userId!);
        return Ok(new { success = true, message = "Logged out successfully" });
    }

    // ... other endpoints
}
```
