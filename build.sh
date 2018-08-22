rm -rf src/CommonLibrariesForNET/bin
rm -rf src/CommonLibrariesForNET/obj
rm -rf src/ChatterToolkitForNET/bin
rm -rf src/ChatterToolkitForNET/obj
rm -rf src/ForceToolkitForNET/bin
rm -rf src/ForceToolkitForNET/obj

dotnet clean src/ForceToolkitForNET.sln
dotnet build src/ForceToolkitForNET.sln