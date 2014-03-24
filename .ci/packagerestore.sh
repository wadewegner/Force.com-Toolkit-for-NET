#!/bin/sh -x

mono --runtime=v4.0 nuget/buildsupport/NuGet.exe restore $@

exit 0