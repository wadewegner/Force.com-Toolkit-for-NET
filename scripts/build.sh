mono assets/nuget.exe restore "../src/ForceToolkitForNET.sln"

mono assets/nuget.exe install NUnit.Runners -Version 2.6.4 -OutputDirectory testrunner

mono assets/nuget.exe install Microsoft.Bcl.Compression -OutputDirectory testrunner
mono assets/nuget.exe install Microsoft.Net.Http -OutputDirectory testrunner

xbuild /t:Clean "../src/ForceToolkitForNET.sln"

xbuild /p:Configuration=Debug "../src/ForceToolkitForNET.sln"

xbuild /p:Configuration=Release "../src/ForceToolkitForNET.sln"