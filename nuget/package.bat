@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=buildsupport\nuget.exe
 
FOR %%G IN (packaging\nuget\*.nuspec) DO (
  %NUGET% pack %%G -Version %VERSION% -Symbols -o artifacts
)