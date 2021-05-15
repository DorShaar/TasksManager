@ECHO OFF

echo Testing TaskerManagerCore
dotnet test --no-build TaskManager/Tests/Domain/ObjectSerializer.JsonService.Tests/ObjectSerializer.JsonService.Tests.csproj
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet test --no-build TaskManager/Tests/Domain/TaskData.Tests/TaskData.Tests.csproj
if %errorlevel% neq 0 exit /b %errorlevel%

echo Packing TaskData
set /p packageVersion="Enter package version: "
dotnet pack --no-build -p:PackageVersion=%packageVersion% TaskManager/Domain/TaskData/TaskData.csproj

echo Pushing package to github
echo Pushing TaskManager/Domain/TaskData/bin/Debug/TaskData.%packageVersion%.nupkg
dotnet nuget push TaskManager/Domain/TaskData/bin/Debug/Tasker.TaskData.%packageVersion%.nupkg --source "github"