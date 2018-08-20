dotnet nuget push artifacts/DeveloperForce.Force.2.0.0.nupkg $1 -k $1 -s https://api.nuget.org/v3/index.json

# dotnet nuget push artifacts/DeveloperForce.Force.2.0.0.symbols.nupkg $1 -k $1 -s https://api.nuget.org/v3/index.json

dotnet nuget push artifacts/DeveloperForce.Chatter.2.0.0.nupkg -k $1 -s https://api.nuget.org/v3/index.json

# dotnet nuget push artifacts/DeveloperForce.Chatter.2.0.0.symbols.nupkg -k $1 -s https://api.nuget.org/v3/index.json