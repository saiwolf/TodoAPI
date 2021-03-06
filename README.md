﻿# Todo Web API in ASP.NET Core

### By Robert Cato 
[saiwolf@swmnu.net](mailto:saiwolf@swmnu.net)

This project is licensed under the [MIT](https://opensource.org/licenses/MIT) license and copyright to [Robert Cato](mailto:saiwolf@swmnu.net)

This is a simple example of me learning how to construct Web APIs using [ASP.NET Core 2.1 Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.1) utilizing Microsoft SQL Server as the DB backend.

The User code was borrowed/modified from this [Tutorial](http://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api)
by [Jason Watmore](http://jasonwatmore.com).

It builds off a simple To-Do API and adds User Authentication via JWT Tokens.

## Prerequisites
* [Visual Studio](https://visualstudio.microsoft.com/downloads/) - The Community Edition is free. You'll need .NET Core 2.1 and the ASP.NET Core Templates.
* [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/) - I used Microsoft SQL Server in this project, but due to EntityFramework, you could just as easily use another DB provider.
* [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - A really awesome package for documenting and testing APIs.
* [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net) - For hashing passwords for users and password checking using BCrypt.

## Instructions
1. Rename appsettings.json.sample to appsettings.json
1. Edit appsettings.json to update your connection string for SQL Server and specify a random string for the secret.
2. In the Package Manager Console (or a shell), change to the directory containing `TodoAPI.csproj` and run the following:
```
dotnet restore
dotnet ef database update
```
##### Note: `dotnet restore` will restore the packages outlined in the prerequisites section.
3. At this point, you should have a table on your SQL Server Instance.
4. Press `F5` in Visual Studio to run the project. You should see IISExpress appear in your system tray.
5. Navigate to [https://localhost:44397/swagger/index.html](https://localhost:44397/swagger/index.html) in your browser (Your port number may vary.)
6. If all has gone well, you should see the Swagger UI and the display of all the endpoints! Congrats!