# Testing Guide for Role-Based Access Control

## Quick Test Implementation

The role-based access control system has been implemented with the following components:

### 1. Authorization Attributes Applied
- **AdminController**: `[AdminOnly]` - Only users with TypeID = 1 can access
- **EnduserController**: `[EndUserOnly]` - Only users with TypeID = 2 can access  
- **AssigneeController**: `[AssigneeOnly]` - Only users with TypeID = 4 can access
- **FaciHeaderController**: `[FacilityHeaderOnly]` - Only users with TypeID = 3 can access

### 2. How It Works
When a user tries to access any of these controllers:

1. The authorization attribute checks the user's session
2. If no session exists → redirects to login
3. If user doesn't have the right role → redirects to their appropriate home page
4. If user has the correct role → allows access

### 3. Testing Steps

#### Step 1: Test Admin Access
1. Login as an Admin user (TypeID = 1)
2. Try to navigate to:
   - `/Admin/Index` → Should work ✓
   - `/Enduser/Index` → Should redirect to `/Admin/Index` ✓
   - `/Assignee/Index` → Should redirect to `/Admin/Index` ✓
   - `/FaciHeader/Index` → Should redirect to `/Admin/Index` ✓

#### Step 2: Test Student/End User Access
1. Login as a Student user (TypeID = 2)
2. Try to navigate to:
   - `/Enduser/Index` → Should work ✓
   - `/Admin/Index` → Should redirect to `/Enduser/Index` ✓
   - `/Assignee/Index` → Should redirect to `/Enduser/Index` ✓
   - `/FaciHeader/Index` → Should redirect to `/Enduser/Index` ✓

#### Step 3: Test Facility Header Access
1. Login as a Facility Header user (TypeID = 3)
2. Try to navigate to:
   - `/FaciHeader/Index` → Should work ✓
   - `/Admin/Index` → Should redirect to `/FaciHeader/Index` ✓
   - `/Enduser/Index` → Should redirect to `/FaciHeader/Index` ✓
   - `/Assignee/Index` → Should redirect to `/FaciHeader/Index` ✓

#### Step 4: Test Assignee Access
1. Login as an Assignee user (TypeID = 4)
2. Try to navigate to:
   - `/Assignee/Index` → Should work ✓
   - `/Admin/Index` → Should redirect to `/Assignee/Index` ✓
   - `/Enduser/Index` → Should redirect to `/Assignee/Index` ✓
   - `/FaciHeader/Index` → Should redirect to `/Assignee/Index` ✓

#### Step 5: Test No Session
1. Without logging in (or after logging out)
2. Try to navigate to any protected controller
3. Should redirect to `/WelcomePage/Login` ✓

### 4. Manual URL Testing

You can manually type these URLs in the browser address bar to test:
- `http://localhost:port/Admin/Index`
- `http://localhost:port/Enduser/Index`
- `http://localhost:port/Assignee/Index`
- `http://localhost:port/FaciHeader/Index`

### 5. Expected Behavior

**✅ SUCCESS**: Users can only access their designated controllers and are automatically redirected when accessing restricted areas.

**❌ PROBLEM**: If you see error pages instead of redirects, there may be a compilation issue.

### 6. Implementation Files

The following files have been created/modified:

1. **`Attributes/RoleAuthorizationAttributes.cs`** - Contains all the authorization attributes
2. **`Controllers/AdminController.cs`** - Modified with `[AdminOnly]` attribute
3. **`Controllers/EnduserController.cs`** - Modified with `[EndUserOnly]` attribute
4. **`Controllers/AssigneeController.cs`** - Modified with `[AssigneeOnly]` attribute
5. **`Controllers/FaciHeaderController.cs`** - Modified with `[FacilityHeaderOnly]` attribute
6. **`OnlineHelpDesk2.csproj`** - Updated to include new attribute files

### 7. Compilation Check

If you see compilation errors:

1. **Check namespaces**: Make sure `using OnlineHelpDesk2.Attributes;` is added to controllers
2. **Check project file**: Ensure `Attributes/RoleAuthorizationAttributes.cs` is included in the project
3. **Rebuild solution**: Clean and rebuild the entire solution in Visual Studio

### 8. Troubleshooting

**Problem**: "AdminOnly could not be found"
**Solution**: 
1. Clean and rebuild the solution
2. Check that `using OnlineHelpDesk2.Attributes;` is at the top of the controller file
3. Verify that `RoleAuthorizationAttributes.cs` is included in the project

**Problem**: Getting error pages instead of redirects
**Solution**: 
1. Check that the database connection is working
2. Verify that user sessions are properly set during login
3. Check that the controller names in the redirect logic match your actual controllers

### 9. Security Benefits

✅ **Admins cannot access student pages** - they get redirected to admin home
✅ **Students cannot access admin pages** - they get redirected to student home  
✅ **Facility staff stay in their area** - they get redirected to facility home
✅ **No unauthorized access** - users without sessions are redirected to login
✅ **Database validation** - checks user roles from database, not just session

The system provides comprehensive role-based protection while maintaining a smooth user experience through automatic redirections rather than error pages.
