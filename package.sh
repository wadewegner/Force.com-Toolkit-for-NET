mkdir "artifacts"

dotnet clean src/ForceToolkitForNET.sln

dotnet build src/ForceToolkitForNET.sln -c Release /p:DebugType=Full

dotnet build src/ForceToolkitForNET.sln -c Debug /p:DebugType=Full

dotnet pack src/ForceToolkitForNET/ForceToolkitForNET.csproj -o "..\..\artifacts" --include-symbols --include-source

dotnet pack src/ChatterToolkitForNET/ChatterToolkitForNET.csproj -o "..\..\artifacts" --include-symbols --include-source