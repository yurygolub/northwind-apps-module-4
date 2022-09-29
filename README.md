# Northwind Applications

## Module 4. ASP.NET Core Web API application

### Build and Run
```sh
git clone https://github.com/yurygolub/northwind-apps-module-4.git
cd northwind-apps-module-4\NorthwindApiApp
dotnet run
```

### API

#### Products

| Operation        | HTTP Verb | URI                | Request body | Response body |
| ---------------- | --------- | ------------------ | ------------ |  ------------ |
| Create           | POST      | /api/products      |              |               |
| Read (all items) | GET       | /api/products      |              |               |
| Read (item)      | GET       | /api/products/{id} |              |               |
| Update           | PUT       | /api/products/{id} |              |               |
| Delete           | DELETE    | /api/products/{id} |              |               |

#### ProductCategories

| Operation        | HTTP Verb | URI                  | Request body | Response body |
| ---------------- | --------- | -------------------- | ------------ |  ------------ |
| Create           | POST      | /api/categories      |              |               |
| Read (all items) | GET       | /api/categories      |              |               |
| Read (item)      | GET       | /api/categories/{id} |              |               |
| Update           | PUT       | /api/categories/{id} |              |               |
| Delete           | DELETE    | /api/categories/{id} |              |               |

| Operation        | HTTP Verb | URI                                  | Request body    | Response body  |
| ---------------- | --------- | ------------------------------------ | --------------- | -------------- |
| Upload picture   | PUT       | /api/categories/{categoryId}/picture | Picture stream  | None           |
| Get picture      | GET       | /api/categories/{categoryId}/picture | None            | Picture stream |
| Delete picture   | DELETE    | /api/categories/{categoryId}/picture | None            | None           |

#### Employees

| Operation        | HTTP Verb | URI                 | Request body | Response body |
| ---------------- | --------- | ------------------- | ------------ | ------------- |
| Create           | POST      | /api/employees      |              |               |
| Read (all items) | GET       | /api/employees      |              |               |
| Read (item)      | GET       | /api/employees/{id} |              |               |
| Update           | PUT       | /api/employees/{id} |              |               |
| Delete           | DELETE    | /api/employees/{id} |              |               |

| Operation        | HTTP Verb | URI                               | Request body    | Response body  |
| ---------------- | --------- | --------------------------------- | --------------- | -------------- |
| Upload photo     | PUT       | /api/employees/{employeeId}/photo | Photo stream    | None           |
| Get photo        | GET       | /api/employees/{employeeId}/photo | None            | Photo stream   |
| Delete photo     | DELETE    | /api/employees/{employeeId}/photo | None            | None           |

### Change start mode
use file \northwind-apps-module-4\NorthwindApiApp\Properties\launchSettings.json
set the "ASPNETCORE_ENVIRONMENT" environment variable to "Prod" to run in production mode

### Change services
use following files to configure services
* in production mode: \northwind-apps-module-4\NorthwindApiApp\appsettings.json
* in development mode: \northwind-apps-module-4\NorthwindApiApp\appsettings.Development.json

set "Mode" to use one of the following service types
* "InMemory" - use in memory database, generates fake data
* "Ef" - use local database using Entity Framework Core
* "Sql" use local database using ADO.NET

### Create database
before using local database services you have to create a database
* create database using SQL script [instnwnd.sql](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql)
* create stored procedures using this file: \northwind-apps-module-4\Northwind.DataAccess.SqlServer\Sql scripts\dbo.CreaterProcedures.sql
