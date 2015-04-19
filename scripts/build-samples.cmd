"assets\NuGet.exe" restore "..\samples\Samples.sln" -ConfigFile "assets\NuGet.Config" -Source nuget -Verbosity detailed -NoCache -DisableParallelProcessing

"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "..\samples\Samples.sln" /verbosity:minimal /target:Clean

"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "..\samples\Samples.sln" /verbosity:minimal /target:Rebuild /property:Configuration=Debug

"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "..\samples\Samples.sln" /verbosity:minimal /target:Rebuild /property:Configuration=Release