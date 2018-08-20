

mkdir "artifacts"

dotnet clean src/ForceToolkitForNET.sln

dotnet build src/ForceToolkitForNET.sln -c Release /p:DebugType=Full

dotnet build src/ForceToolkitForNET.sln -c Debug /p:DebugType=Full

dotnet pack "src/ForceToolkitForNET/ForceToolkitForNET.csproj" /p:NuspecFile="ForceToolkitForNET.nuspec" -o "../../artifacts"

dotnet pack "src/ForceToolkitForNET/ForceToolkitForNET.csproj" /p:NuspecFile="ForceToolkitForNET.Symbol.nuspec" -o "../../artifacts" --include-symbols  --include-source

dotnet pack "src/ChatterToolkitForNET/ChatterToolkitForNET.csproj" /p:NuspecFile="ChatterToolkitForNET.nuspec" -o "../../artifacts"

dotnet pack "src/ChatterToolkitForNET/ChatterToolkitForNET.csproj" /p:NuspecFile="ChatterToolkitForNET.Symbol.nuspec" -o "../../artifacts" --include-symbols  --include-source