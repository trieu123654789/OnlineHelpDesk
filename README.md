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

3) Configure database
   - In `OnlineHelpDesk2/Web.config`, update the connection string named `OHDDBContext` to point to your SQL Server instance
   - Example LocalDB: `Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineHelpDeskDb;Integrated Security=True;MultipleActiveResultSets=True`

4) Run the app
   - Set `OnlineHelpDesk2` as the startup project
   - Press F5 to run with IIS Express

5) Seeding/admin account
   - If you use a fresh database, create an initial Admin account directly in the database or add a lightweight seeding routine as needed

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

### Troubleshooting
- “Cannot attach database” – verify the connection string and SQL Server instance
- NuGet errors – run `Tools → NuGet Package Manager → Restore` or `Update-Package -reinstall`
- 404/route issues – check `RouteConfig.cs` and controller/action names

### License
This project is provided for educational/portfolio purposes. Add a license if you plan to distribute.

### Author
`trieu <cqtrieua22045@cusc.ctu.edu.vn>`


