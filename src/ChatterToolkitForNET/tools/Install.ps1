param($installPath, $toolsPath, $package, $project)

$readmeFile = "http://www.wadewegner.com/2014/01/announcing-the-salesforce-toolkits-for-net/"
$DTE.ItemOperations.Navigate($readmeFile, [EnvDTE.vsNavigateOptions]::vsNavigateOptionsNewWindow)