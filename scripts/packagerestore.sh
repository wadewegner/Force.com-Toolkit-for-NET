#!/bin/sh -x

mono --runtime=v4.0 scripts/assets/NuGet.exe restore $@ -ConfigFile scripts/assets/NuGet.Config -Source nuget -Verbosity detailed -NoCache -DisableParallelProcessing

if [ $? -ne 0 ]
then   
 exit 1
fi

exit $?