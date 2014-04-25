param($installPath, $toolsPath, $package, $project)

$readmeFile = "http://www.wadewegner.com/2014/04/Announcing-the-Windows-Phone-8-Accelerators-for-Salesforce/"
$DTE.ItemOperations.Navigate($readmeFile, [EnvDTE.vsNavigateOptions]::vsNavigateOptionsNewWindow)