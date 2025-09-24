# Role-Based Access Control Implementation

This document outlines the implementation of role-based access control for the Online Help Desk system.

## Overview

The role-based access control system prevents users from accessing pages and functionality that are restricted to their role. Each user role has specific access permissions:

- **Admin (TypeID = 1)**: Access to Admin controller and related pages
- **End User/Student (TypeID = 2)**: Access to Enduser controller and related pages  
- **Facility Header (TypeID = 3)**: Access to FaciHeader controller and related pages
- **Assignee (TypeID = 4)**: Access to Assignee controller and related pages

## Implementation Components

### 1. Custom Authorization Attributes (`Attributes/RoleAuthorizationAttributes.cs`)

Custom ActionFilter attributes that check user roles:

- `AdminOnlyAttribute`: Restricts access to Admin users only
- `EndUserOnlyAttribute`: Restricts access to End Users only
- `FacilityHeaderOnlyAttribute`: Restricts access to Facility Headers only
- `AssigneeOnlyAttribute`: Restricts access to Assignees only
- `FacilityStaffOnlyAttribute`: Allows both Facility Headers and Assignees
- `AuthenticatedOnlyAttribute`: Requires user to be logged in (any role)

### 2. Base Controller (`Controllers/BaseController.cs`)

All controllers inherit from `BaseController` which provides:

- Common user session management
- Role checking methods (`HasRole`, `HasAnyRole`)
- User information retrieval (`GetCurrentUser`)
- Automatic ViewBag population with user info
- Error handling for session issues

### 3. Global Authorization Filter (`Attributes/GlobalAuthorizationFilter.cs`)

A global filter that:

- Validates user sessions on every request
- Ensures users can only access their appropriate controllers
- Redirects unauthorized users to their correct home page
- Handles session integrity checks

### 4. Controller-Level Protection

Each controller is protected with appropriate attributes:

```csharp
[AdminOnly]
public class AdminController : BaseController

[EndUserOnly] 
public class EnduserController : BaseController

[FacilityHeaderOnly]
public class FaciHeaderController : BaseController

[AssigneeOnly]
public class AssigneeController : BaseController
```

## How It Works

### 1. User Login
When a user logs in via `WelcomePageController.Login()`, their `AccountID` and `TypeID` are stored in the session.

### 2. Request Processing
For every subsequent request:

1. **Global Filter Check**: The `GlobalAuthorizationFilter` runs first and validates:
   - Session exists and contains valid `AccountID`
   - User still exists in the database
   - User is accessing an appropriate controller for their role

2. **Controller Attribute Check**: If accessing a protected controller, the role-specific attribute (e.g., `AdminOnly`) validates:
   - User has the correct role for this controller
   - If not, redirects to user's appropriate home page

### 3. Unauthorized Access Handling
If a user tries to access a restricted page:

- **Same Role**: Redirected to their home page (e.g., Admin trying to access another admin page they don't have access to)
- **Different Role**: Redirected to their role-appropriate home page (e.g., Student trying to access Admin pages → redirected to Student home)
- **No Session**: Redirected to login page

## Security Features

### Session Integrity
- Validates user exists in database on every request
- Clears corrupted sessions automatically
- Handles database connection errors gracefully

### Role Validation
- Database-driven role checking (not just session-based)
- Multiple role support for shared functionality
- Automatic redirection to appropriate pages

### Error Handling
- Graceful handling of session timeouts
- Database error recovery
- User-friendly redirections

## Usage Examples

### Protecting a New Controller
```csharp
[AdminOnly]
public class NewAdminController : BaseController
{
    public ActionResult Index()
    {
        // User's info is automatically available in ViewBag.FullName
        // Role checking is handled by the attribute
        return View();
    }
}
```

### Checking Roles in Actions
```csharp
public ActionResult SomeAction()
{
    if (HasRole(1)) // Check if user is Admin
    {
        // Admin-specific logic
    }
    
    if (HasAnyRole(3, 4)) // Check if user is Facility Head or Assignee
    {
        // Facility staff logic
    }
    
    var currentUser = GetCurrentUser(); // Get full user object
    return View();
}
```

### Custom Role Combinations
```csharp
[FacilityStaffOnly] // Allows both TypeID 3 and 4
public class SharedController : BaseController
{
    // Actions accessible by both Facility Headers and Assignees
}
```

## Testing the Implementation

### Test Scenarios

1. **Valid Access**: 
   - Admin user accessing Admin pages ✓
   - Student user accessing Student pages ✓

2. **Invalid Access**:
   - Admin user trying to access Student pages → Redirected to Admin home
   - Student user trying to access Admin pages → Redirected to Student home
   - User with invalid session → Redirected to login

3. **Session Management**:
   - User logs out → Session cleared, redirected to login
   - Session expires → Automatic redirect to login
   - User deleted from database → Session cleared, redirected to login

### Manual Testing Steps

1. **Login as different user types** and verify appropriate landing pages
2. **Manually navigate to restricted URLs** in browser address bar
3. **Verify redirections** happen correctly without error pages
4. **Test session timeout** by waiting or clearing cookies
5. **Test with invalid session data** by modifying session values

## Security Benefits

- **Prevents unauthorized access** to sensitive functionality
- **Maintains user context** throughout the application
- **Graceful error handling** improves user experience  
- **Database-driven validation** ensures data integrity
- **Automatic session cleanup** prevents stale sessions
- **Role-based redirection** keeps users in their appropriate areas

## Maintenance

To add new roles:
1. Add new `TypeID` to database
2. Create new attribute in `RoleAuthorizationAttributes.cs`
3. Update `GetRedirectRouteForUserType` methods
4. Apply attribute to new controllers
5. Update login logic in `WelcomePageController`

The system is designed to be extensible and maintainable for future role additions or modifications.
