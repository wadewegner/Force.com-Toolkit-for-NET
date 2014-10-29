mkdir "../artifacts"

"assets/NuGet.exe" pack "../src/ChatterToolkitForNET/ChatterToolkitForNET.nuspec" -Symbols -o "../artifacts"

"assets/NuGet.exe" pack "../src/ForceToolkitForNET/ForceToolkitForNET.nuspec" -Symbols -o "../artifacts"

"assets/NuGet.exe" pack "../src/ChatterToolkitForNET/ChatterToolkitForNET.Symbol.nuspec" -Symbols -o "../artifacts"

"assets/NuGet.exe" pack "../src/ForceToolkitForNET/ForceToolkitForNET.Symbol.nuspec" -Symbols -o "../artifacts"