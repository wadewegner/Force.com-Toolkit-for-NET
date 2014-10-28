"assets\NuGet.exe" restore "..\src\ForceToolkitForNET.sln" -ConfigFile "assets\NuGet.Config" -Source nuget -Verbosity detailed -NoCache -DisableParallelProcessing

"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "..\src\ForceToolkitForNET.sln" /verbosity:detailed /target:Clean

"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "..\src\ForceToolkitForNET.sln" /verbosity:detailed /target:Rebuild /property:Configuration=Debug

"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "..\src\ForceToolkitForNET.sln" /verbosity:detailed /target:Rebuild /property:Configuration=Release