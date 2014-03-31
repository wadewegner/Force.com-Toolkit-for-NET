 #!/bin/sh -x

# package restore
.ci/packagerestore.sh src/CommonLibrariesForNET.sln
.ci/packagerestore.sh src/ChatterToolkitForNET.sln
.ci/packagerestore.sh src/ForceToolkitForNET.sln

# build projects
xbuild src/CommonLibrariesForNET.sln
xbuild src/ChatterToolkitForNET.sln
xbuild src/ForceToolkitForNET.sln

# unit tests
.ci/nunit.sh src/CommonLibrariesForNET.FunctionalTests/bin/Debug/Salesforce.Common.FunctionalTests.dll
.ci/nunit.sh src/CommonLibrariesForNET.UnitTests/bin/Debug/Salesforce.Common.UnitTests.dll
.ci/nunit.sh src/ForceToolkitForNET.FunctionalTests/bin/Debug/Salesforce.Force.FunctionalTests.dll
.ci/nunit.sh src/ForceToolkitForNET.UnitTests/bin/Debug/Salesforce.Force.UnitTests.dll
.ci/nunit.sh src/ChatterToolkitForNET.FunctionalTests/bin/Debug/Salesforce.Chatter.FunctionalTests.dll