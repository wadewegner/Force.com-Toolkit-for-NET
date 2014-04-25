param($installPath, $toolsPath, $package, $project)

$readmeFile = "http://www.wadewegner.com/2014/04/announcing-the-salesforce-accelerators-for-windows-store-apps/"
$DTE.ItemOperations.Navigate($readmeFile, [EnvDTE.vsNavigateOptions]::vsNavigateOptionsNewWindow)