## Online Help Desk (ASP.NET MVC)

An ASP.NET MVC 5 application that provides a simple internal help desk for submitting, assigning, and resolving facility/service requests. It includes role-based dashboards for Admin, Assignee (technician), Facility Header (dispatcher/lead), and End User.

### Key Features
- Request lifecycle: create, assign, reply, resolve, reject, and summarize
- Role-based access control with multiple layouts and views per role
- Basic authentication and session-driven navigation
- Simple reporting pages and Excel-ready tabular views
- Bootstrap-based UI with prebuilt layouts and components

### Tech Stack
- ASP.NET MVC 5 (.NET Framework 4.7.2)
- Entity Framework 6 (code-first style DbContext)
- SQL Server (LocalDB or SQL Server Express/Full)
- Bootstrap, jQuery, jQuery Validation

### Repository Layout (high level)
- `OnlineHelpDesk2/Controllers` – MVC controllers per area/role
- `OnlineHelpDesk2/Models` – EF entities, DbContext, view models
- `OnlineHelpDesk2/Views` – Razor views (role-specific layouts and pages)
- `OnlineHelpDesk2/Content` and `OnlineHelpDesk2/assets` – CSS, fonts, images
- `OnlineHelpDesk2/Scripts` and `OnlineHelpDesk2/Js` – client-side scripts
- `OnlineHelpDesk2/Web.config` – app and connection string configuration

### Getting Started
1) Requirements
   - Windows with Visual Studio 2019/2022
   - .NET Framework Developer Pack 4.7.2
   - SQL Server (LocalDB/Express/Developer)

2) Open the solution
   - Open `OnlineHelpDesk2.sln` in Visual Studio
   - Allow NuGet to restore packages on first load

3) Database Setup
   **Option A: Using the provided SQL script (Recommended for HR/Testing)**
   - Ensure SQL Server (LocalDB, Express, or Full) is installed and running
   - Open SQL Server Management Studio (SSMS) or any SQL client
   - Connect to your SQL Server instance
   - Create a new database named `OnlineHelpDesk` or use the script to create it
   - Open and execute the `online-help-desk.sql` file from the project root
     - This will create all tables, relationships, and insert sample data
     - Sample accounts are included for testing (see "Default Accounts" section below)
   - Update the connection string in `OnlineHelpDesk2/Web.config`:
     ```xml
     <connectionStrings>
       <add name="OHDDBContext" 
            connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineHelpDesk;Integrated Security=True;MultipleActiveResultSets=True" 
            providerName="System.Data.SqlClient" />
     </connectionStrings>
     ```
   
   **Option B: Using Entity Framework Code-First (For Developers)**
   - In `OnlineHelpDesk2/Web.config`, update the connection string named `OHDDBContext` to point to your SQL Server instance
   - Example LocalDB: `Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineHelpDeskDb;Integrated Security=True;MultipleActiveResultSets=True`
   - Run the application - EF will create the database automatically (but without sample data)

4) Run the app
   - Set `OnlineHelpDesk2` as the startup project
   - Press F5 to run with IIS Express

5) Ready to Use
   - The database is now configured with sample data and accounts
   - See the "Easy Database Setup" section above for login credentials
   - All sample accounts use "123456" as password for testing purposes

### Common Tasks
- Change site branding and navigation: edit layout files under `OnlineHelpDesk2/Views/Shared/`
- Modify roles/authorization: see attributes and filters under `OnlineHelpDesk2/Attributes` and `OnlineHelpDesk2/App_Start`
- Update EF models: edit entities under `OnlineHelpDesk2/Models`, then update schema via migrations or a manual script

### Security Notes
- Enable HTTPS in the project settings and enforce HSTS in production
- Use anti-forgery tokens on form posts (MVC helpers) and validate user inputs
- Validate and limit file uploads (size, MIME, storage path) if enabled

### Testing
- Unit tests are not included by default
- Suggested: add xUnit/NUnit test project and cover key flows (authentication, request lifecycle, authorization)

### CI/CD (Optional)
- Add a GitHub Actions workflow to build the solution and run tests on push/PR
- Example step: `windows-latest`, setup MSBuild, restore NuGet, build, run tests

### Easy Database Setup (Recommended for HR)

**🚀 Quick Setup (1-Click Method):**
1. **Check Requirements:** Double-click `check-requirements.ps1` to verify your system
2. **Auto Setup:** Double-click `setup-database.bat` to automatically create the database
3. **Run Application:** Open `OnlineHelpDesk2.sln` in Visual Studio and press F5

*That's it! The scripts will handle everything automatically.*

**📋 Sample Accounts (Pre-configured):**

| Username | Password | Role |
|----------|----------|------|
| admin | 123456 | Administrator |
| quoctrieu | 123456 | Student |
| giathoai | 123456 | Student |
| thuy | 123456 | Facility Head |
| giabao | 123456 | Assignee |

**🔧 What the Scripts Do:**
- `check-requirements.ps1`: Verifies .NET Framework, SQL Server LocalDB, Visual Studio, and project files
- `setup-database.bat`: Creates LocalDB instance, imports schema, updates Web.config automatically

---

### Manual Database Setup Guide (Alternative Method)

**Prerequisites:**
1. Install SQL Server Express (free) from Microsoft's website
2. Install SQL Server Management Studio (SSMS) - free download from Microsoft

**Step-by-Step Database Setup:**

1. **Start SQL Server Management Studio (SSMS)**
   - Open SSMS from Start Menu
   - Connect to your local SQL Server instance (usually `(localdb)\MSSQLLocalDB` or `.\SQLEXPRESS`)

2. **Create the Database**
   - Right-click on "Databases" in Object Explorer
   - Select "New Database..."
   - Name it `OnlineHelpDesk`
   - Click "OK"

3. **Import the Database Schema and Data**
   - Right-click on the `OnlineHelpDesk` database you just created
   - Select "New Query"
   - Open the `online-help-desk.sql` file from the project folder
   - Copy all contents and paste into the query window
   - Click "Execute" (or press F5)
   - Wait for "Commands completed successfully" message

4. **Verify the Setup**
   - Expand the `OnlineHelpDesk` database in Object Explorer
   - You should see tables like: Accounts, Requests, Facilities, etc.
   - Right-click on "Accounts" table → "Select Top 1000 Rows" to verify sample data exists

5. **Update Application Configuration**
   - Open the project in Visual Studio
   - Navigate to `OnlineHelpDesk2/Web.config`
   - Find the `<connectionStrings>` section
   - Update the connection string to match your SQL Server instance:
     ```xml
     <add name="OHDDBContext" 
          connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineHelpDesk;Integrated Security=True;MultipleActiveResultSets=True" 
          providerName="System.Data.SqlClient" />
     ```

**Testing the Application:**
- Run the application (F5 in Visual Studio)
- Navigate to the login page
- Use admin credentials: Username: `admin`, Password: `123456`

### Troubleshooting

**🔧 Automated Setup Issues:**

| Problem | Solution |
|---------|----------|
| **Script won't run** | Right-click `.ps1` file → "Run with PowerShell" or run: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| **LocalDB not found** | Download SQL Server Express LocalDB from [Microsoft](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **"Access denied"** | Run Command Prompt as Administrator before running `setup-database.bat` |
| **Database already exists** | Normal behavior - script will use existing database |
| **PowerShell execution policy** | Open PowerShell as Admin: `Set-ExecutionPolicy RemoteSigned` |

**📋 Common Application Issues:**

| Problem | Solution |
|---------|----------|
| "Cannot attach database" | Verify connection string and SQL Server instance |
| "Login failed" | Ensure SQL Server is running and connection string is correct |
| NuGet errors | Run `Tools → NuGet Package Manager → Restore` |
| 404/route issues | Check `RouteConfig.cs` and controller/action names |

**🔄 Quick Reset Process:**
If something goes wrong, reset everything:
1. **Delete Database:** Open SSMS → Connect to `(localdb)\MSSQLLocalDB` → Right-click "OnlineHelpDesk" → Delete
2. **Re-run Setup:** Double-click `setup-database.bat` again
3. **Alternative:** Use SQL command: `DROP DATABASE OnlineHelpDesk`

**💡 Pro Tips:**
- Always run scripts from the project root directory
- Check Windows Event Viewer for detailed error messages
- Ensure no antivirus is blocking the scripts
- LocalDB runs automatically when accessed - no manual start needed

### License
This project is provided for educational/portfolio purposes. Add a license if you plan to distribute.

### Author
`trieu <cqtrieua22045@cusc.ctu.edu.vn>`


