# Command Line Interface Commands for Migrations

Use .NET Core Command List Interface to execute entity framework core commands. To use .NET CLI, add <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" /> under <ItemGroup> node by editing your .NET Core project's .csproj file.

Open command prompt and navigate to your project's root folder and enter dotnet ef --help to list EF Core commands, as shown below.

C:> dotnet ef --help
Entity Framework Core .NET Command Line Tools 2.0.0-rtm-26452

Usage: dotnet ef [options] [command]

Options:
  --version        Show version information
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  database    Commands to manage the database.
  dbcontext   Commands to manage DbContext types.
  migrations  Commands to manage migrations.

Use "dotnet ef [command] --help" for more information about a command.
    
As you can see above, there are three main EF commands available: database, dbcontext and migrations. The following table lists all EF commands and sub commands.

Command	Sub Commands	Usage
Database	drop	Drops the database.
update	Updates the database to a specified migration.
DbContext	info	Gets information about a DbContext type.
list	Lists available DbContext types.
scaffold	Scaffolds a DbContext and entity types for a database.
Migration	add	Adds a new migration.
list	Lists available migrations.
remove	Removes the last migration.
script:	Generates a SQL script from migrations.
Let's see available options for each command.

Database Drop
> dotnet ef database drop

Usage: dotnet ef database drop [options]

Options:
  -f|--force                             Don't confirm.
  --dry-run                              Show which database would be dropped, but don't drop it.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this when 
                                         the build is up-to-date.
Database Update
> dotnet ef database update

Usage: dotnet ef database update [arguments] [options]

Arguments:
  <MIGRATION>  The target migration. If '0', all migrations will be reverted. De
faults to the last migration.

Options:
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this when
                                         the build is up-to-date.
DbContext Info
> dotnet ef dbcontext info

Usage: dotnet ef dbcontext info [options]

Options:
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
    
ADVERTISEMENT
DbContext List
> dotnet ef dbcontext list

Usage: dotnet ef dbcontext list [options]

Options:
  --json                                 Show JSON output.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
    
DbContext Scaffold
> dotnet ef dbcontext scaffold

Usage: dotnet ef dbcontext scaffold [arguments] [options]

Arguments:
  <CONNECTION>  The connection string to the database.
  <PROVIDER>    The provider to use. (E.g. Microsoft.EntityFrameworkCore.SqlServ
er)

Options:
  -d|--data-annotations                  Use attributes to configure the model (
where possible). If omitted, only the fluent API is used.
  -c|--context <NAME>                    The name of the DbContext.
  -f|--force                             Overwrite existing files.
  -o|--output-dir <PATH>                 The directory to put files in. Paths ar
e relative to the project directory.
  --schema <SCHEMA_NAME>...              The schemas of tables to generate entit
y types for.
  -t|--table >TABLE_NAME<...             The tables to generate entity types for.
  --use-database-names                   Use table and column names directly from the database.
  --json                                 Show JSON output.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
    
Add
> dotnet ef migrations add

Usage: dotnet ef migrations add [arguments] [options]

Arguments:
  <NAME>  The name of the migration.

Options:
  -o|--output-dir <PATH>                 The directory (and sub-namespace) to us
e. Paths are relative to the project directory. Defaults to "Migrations".
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
    
List
> dotnet ef migrations list

Usage: dotnet ef migrations list [options]

Options:
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
    
Remove
> dotnet ef migrations remove

Usage: dotnet ef migrations remove [options]

Options:
  -f|--force                             Don't check to see if the migration has
 been applied to the database.
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
    
Script
> dotnet ef migrations script

Usage: dotnet ef migrations script [arguments] [options]

Arguments:
  <FROM>  The starting migration. Defaults to '0' (the initial database).
  <TO>    The ending migration. Defaults to the last migration.

Options:
  -o|--output <FILE>                     The file to write the result to.
  -i|--idempotent                        Generate a script that can be used on a
 database at any migration.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  --no-build                             Don't build the project. Only use this 
                                         when the build is up-to-date.
