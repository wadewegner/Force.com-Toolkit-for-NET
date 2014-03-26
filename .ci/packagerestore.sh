#!/bin/sh -x

mono --runtime=v4.0 nuget/buildsupport/NuGet.exe restore $@ -ConfigFile src/NuGet.Config -Source nuget -Verbosity detailed -NoCache

exit 0