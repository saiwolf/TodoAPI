# Todo Web API in ASP.NET Core

### By Robert Cato 
[saiwolf@swmnu.net](mailto:saiwolf@swmnu.net)

This project is licensed under the [MIT](https://opensource.org/licenses/MIT) license and copyright to [Robert Cato](mailto:saiwolf@swmnu.net)

This is a simple example of me learning how to construct Web APIs using [ASP.NET Core 2.1 Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.1) utilizing [SQLite](https://www.sqlite.org/index.html) as the DB backend.

## Prerequisites
* [Visual Studio](https://visualstudio.microsoft.com/downloads/) - The Community Edition is free. You'll need .NET Core 2.1 and the ASP.NET Core Templates.
* [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/2.1.4) - I used SQLite in this project, but due to EntityFramework, you could just as easily use another DB provider.
* [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - A really awesome package for documenting and testing APIs.

## Instructions
1. Add a folder to the project called `Database`. Do this through VS.
2. In the Package Manager Console (or a shell), change to the directory containing `TodoAPI.csproj` and run the following:
```
dotnet restore
dotnet ef database update
```
##### Note: `dotnet restore` will restore the packages outlined in the prerequisites section.
3. At this point, you should have a `todo.db` under your `Database` folder that you made earlier. You can use something like [SQLite Browser](http://sqlitebrowser.org/) to examine it and verify the table schema is correct.
4. Press `F5` in Visual Studio to run the project. You should see IISExpress appear in your system tray.
5. Navigate to [https://localhost:44397/swagger/index.html](https://localhost:44397/swagger/index.html) in your browser (Your port number may vary.)
6. If all has gone well, you should see the Swagger UI and the display of all the endpoints! Congrats!