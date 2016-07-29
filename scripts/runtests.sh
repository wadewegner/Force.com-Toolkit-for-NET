mono assets/nuget.exe restore "../src/ForceToolkitForNET.sln"

mono assets/nuget.exe install NUnit.Runners -Version 2.6.4 -OutputDirectory testrunner

mono assets/nuget.exe install Microsoft.Bcl -OutputDirectory testrunner
mono assets/nuget.exe install Microsoft.Bcl.Async -OutputDirectory testrunner
mono assets/nuget.exe install Microsoft.Bcl.Build -OutputDirectory testrunner
mono assets/nuget.exe install Microsoft.Bcl.Compression -OutputDirectory testrunner
mono assets/nuget.exe install Microsoft.Net.Http -OutputDirectory testrunner

xbuild /p:Configuration=Debug "../src/ForceToolkitForNET.sln"

mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe "../src/CommonLibrariesForNET.FunctionalTests/bin/Debug/Salesforce.Common.FunctionalTests.dll"
mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe "../src/CommonLibrariesForNET.UnitTests/bin/Debug/Salesforce.Common.UnitTests.dll"
mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe "../src/ForceToolkitForNET.FunctionalTests/bin/Debug/Salesforce.Force.FunctionalTests.dll"
mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe "../src/ForceToolkitForNET.FunctionalTests/bin/Debug/Salesforce.Force.UnitTests.dll"
mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe "../src/ChatterToolkitForNET.FunctionalTests/bin/Debug/Salesforce.Chatter.FunctionalTests.dll"