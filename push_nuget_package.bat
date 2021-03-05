@ECHO OFF

echo Testing TaskerManagerCore
dotnet test TaskManager/Tests/Domain/ObjectSerializer.JsonService.Tests/ObjectSerializer.JsonService.Tests.csproj
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet test TaskManager/Tests/Domain/TaskData.Tests/TaskData.Tests.csproj
if %errorlevel% neq 0 exit /b %errorlevel%

echo Packing ObjectSerializer.JsonService
set /p packageVersion="Enter package version: "
dotnet pack -p:PackageVersion=%packageVersion% TaskManager/Domain/ObjectSerializer.JsonService/ObjectSerializer.JsonService.csproj

echo Pushing package to github
echo Pushing TaskManager/Domain/ObjectSerializer.JsonService/bin/Debug/Tasker.ObjectSerializer.JsonService.%packageVersion%.nupkg
dotnet nuget push TaskManager/Domain/ObjectSerializer.JsonService/bin/Debug/Tasker.ObjectSerializer.JsonService.%packageVersion%.nupkg --source "github"

echo Packing TaskData
set /p packageVersion="Enter package version: "
dotnet pack -p:PackageVersion=%packageVersion% TaskManager/Domain/TaskData/TaskData.csproj

echo Pushing package to github
echo Pushing TaskManager/Domain/TaskData/bin/Debug/TaskData.%packageVersion%.nupkg
dotnet nuget push TaskManager/Domain/TaskData/bin/Debug/Tasker.TaskData.%packageVersion%.nupkg --source "github"