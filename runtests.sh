dotnet clean tests/ForceToolkitForNET.Tests.sln
dotnet test tests/ForceToolkitForNET.Tests.sln --list-tests
dotnet test tests/ForceToolkitForNET.Tests.sln /p:CollectCoverage=true