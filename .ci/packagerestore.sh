#!/bin/sh -x

mono --runtime=v4.0 nuget/buildsupport/NuGet.exe restore -source "https://www.nuget.org/api/v2" $@

exit 0