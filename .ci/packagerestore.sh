#!/bin/sh -x

mono --runtime=v4.0 nuget/buildsupport/NuGet.exe restore $@ -Verbosity detailed -Source https://www.nuget.org/api/v2/ -NoCache

exit 0
