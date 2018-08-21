rm -rf "artifacts"
mkdir "artifacts"

dotnet clean src/ForceToolkitForNET.sln

dotnet build src/ForceToolkitForNET.sln -c Release /p:DebugType=Full

dotnet build src/ForceToolkitForNET.sln -c Debug /p:DebugType=Full

dotnet pack src/ForceToolkitForNET/ForceToolkitForNET.csproj -o "..\..\artifacts" --include-symbols --include-source

dotnet pack src/ChatterToolkitForNET/ChatterToolkitForNET.csproj -o "..\..\artifacts" --include-symbols --include-source

cp artifacts/DeveloperForce.Chatter.$1.nupkg artifacts/DeveloperForce.Chatter.$1.nupkg.zip

cp artifacts/DeveloperForce.Force.$1.nupkg artifacts/DeveloperForce.Force.$1.nupkg.zip

unzip artifacts/DeveloperForce.Chatter.$1.nupkg.zip -d artifacts/DeveloperForce.Chatter

unzip artifacts/DeveloperForce.Force.$1.nupkg.zip -d artifacts/DeveloperForce.Force

chmod +r artifacts/DeveloperForce.Chatter/DeveloperForce.Chatter.nuspec

chmod +r artifacts/DeveloperForce.Force/DeveloperForce.Force.nuspec