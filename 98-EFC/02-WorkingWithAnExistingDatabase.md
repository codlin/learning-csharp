# Activity 2-0: Working with a pre-existing database
## Task 1: Prerequisites
### Task 1-1: Prerequisite – SQL Server Developer edition
created `docker-compose.yaml` for mssql.  
start up docker:
```shell
docker-copmpose up -d
```
## Task 2: Download and restore the backup file for the latest version of the AdventureWorks database to your machine
### Task 2-1: Download the latest version of AdventureWorks DB:
https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver15&tabs=ssms  
下载文件 `AdventureWorks2022.bak`。

### Task 2-2: Restore the AdventureWorks database to your local SQL instance
1. vscode 安装 mssql，并连接数据库。
2. 创建 `restore-data.sql`
3. 把 DB 备份文件 AdventureWorks2022.bak 复制到 mssql 所在的容器，路径为 `/var/opt/mssql/backup/AdventureWorks2022.bak`
4. 打开 restore-data.sql 文件，`ctrl + shift + e`恢复数据

# Activity 2-1: Reverse-engineering an existing database with EFCore6
## Task 1: Creating the solution with a new project and referencing the DBLibrary project
### Step 1: Create the project and solution
```shell
dotnet new console -n EFCore_Activity0201 -o ./02-ExistingDatabase/EFCore_Activity0201 -f net6.0
dotnet new sln -n EFCore_Activity0201 -o ./02-ExistingDatabase/
cd 02-ExistingDatabase
dotnet sln add EFCore_Activity0201
```
### Step 2: Reference the EFCore_DbLibrary project
```shell
dotnet sln add ../01-EFC/EFCore_DBLibrary/EFCore_DBLibrary.csproj
dotnet add 
```
### Step 3: Reference the code library that will be used to interact
with the database  

## Task 2: Ensure .Net 6 and update all of the NuGet packages for both projects
### Step 1: Ensure .Net 6 on the console project
already support  

### Step 2: Install the Entity Framework Tools (Microsoft.EntityFrameworkCore.Tools) NuGet package
### Step 3: Install the Entity Framework SQL Server (Microsoft.EntityFrameworkCore.SqlServer) NuGet package to both projects in the solution
```shell
cd 01-EFC/EFCore_DBLibrary -f net6.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.16

cd 02-ExistingDatabase/EFCore_Activity0201
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.16
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.16
```

## Task 3: Scaffold a new database context using the Scaffold-Context command
### Step 1: Install the Microsoft.EntityFrameworkCore.Design package to the EFCore_Activity0201 project
```shell
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.16
```
### Step 2: Determine your connection string
```
"Data Source=localhost;Initial Catalog=AdventureWorks;Trusted_Connection=True"
```

## Task 4: Create a settings file and leverage it from code
### Step 1: Add the appsettings.json file to store connection details
add an appsettings.json file to the project that contains the connection string for use when connecting the context and database.

可以不用 `appsettings.json` ，直接用代码连接。
```cs
_optionsBuilder = new DbContextOptionsBuilder<AdventureWorksContext>();
_optionsBuilder.UseSqlServer( configuration.GetConnectionString("AdventureWorks"));
```

### Step 2: Add the libraries to leverage the config file in the activity project
```shell
cd 02-ExistingDatabase/EFCore_Activity0201
dotnet add package Microsoft.Extensions.Configuration --version 7.0.0
dotnet add package Microsoft.Extensions.Configuration.FileExtensions --version 7.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 7.0.0
```

### Step 3: Load up the config and leverage the results in the Main method of the Program.cs file in the activity project

## Task 5: Connect to the database and show results in code
### Step 1: Create the ability to connect and use the AdventureWorks DBContext
### Step 2: Query the data
### Step 3: Print the results to the console