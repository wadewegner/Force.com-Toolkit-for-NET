@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=buildsupport\nuget.exe
 
FOR %%G IN (packaging\nuget\Package-DeveloperForce.%2.nuspec) DO (
  %NUGET% pack %%G -Version %VERSION% -Symbols -o artifacts
)