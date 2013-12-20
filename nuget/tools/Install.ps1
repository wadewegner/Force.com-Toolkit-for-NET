param($installPath, $toolsPath, $package, $project)

$readmeFile = "https://github.com/developerforce/Force.com-Toolkit-for-NET/wiki/Getting-Started"
$DTE.ItemOperations.Navigate($readmeFile, [EnvDTE.vsNavigateOptions]::vsNavigateOptionsNewWindow)