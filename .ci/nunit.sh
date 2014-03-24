#!/bin/sh -x

#mono --runtime=v4.0 nuget/buildsupport/NuGet.exe install NUnit.Runners -Version 2.6.1 -o packages

runTest(){
    #mono --runtime=v4.0 packages/NUnit.Runners.2.6.1/tools/nunit-console.exe -noxml -nodots -labels -stoponerror $@
    mono /Library/Frameworks/Mono.framework/Versions/3.2.6/lib/mono/4.5/nunit-console.exe -labels -noshadow $@

   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

#This is the call that runs the tests and adds tweakable arguments.
#In this case I'm excluding tests I categorized for performance.
runTest $1 -exclude=Performance

exit $?