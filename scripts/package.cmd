mkdir "../artifacts"

"assets/NuGet.exe" pack "assets/ChatterToolkitForNET.nuspec" -Symbols -o "../artifacts"

"assets/NuGet.exe" pack "assets/ForceToolkitForNET.nuspec" -Symbols -o "../artifacts" 

"assets/NuGet.exe" pack "assets/ChatterToolkitForNET.Symbol.nuspec" -Symbols -o "../artifacts"

"assets/NuGet.exe" pack "assets/ForceToolkitForNET.Symbol.nuspec" -Symbols -o "../artifacts"


