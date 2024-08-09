# How to create a nuget package for XrmToolbox

## Important!
Make sure that you update the project version and the assembly version. They need to be the same for the plugin to be accepted by XrmToolbox.
Update the assembly version in the AssemblyInfo.cs file and the project version in the .csproj file.
It is not possible to update a nuget package once it has been published to nuget.org. You will need to create a new package with a higher version number.

## Obtain nuget.exe
Download nuget.exe either from https://www.nuget.org/downloads or obtain it from a Visual Studio installation. The nuget.exe file is usually located in the following folder: `C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\4.0.0\tools\NuGet.exe`.

## run nuget pack using .nuspec file
copy nuget.exe inside the plugin project folder or add it to the PATH environment variable. 
Then navigate to the plugin project folder and run the following command:

```
.\nuget.exe pack .\FlowOwnershipAudit.nuspec
```

This will create a nuget package in the same folder as the plugin project. You can then upload this package to nuget.org.
Since this plugin has already been published on the xrmtoolbox store, you do not need to do anything else, it will be updated automatically.
