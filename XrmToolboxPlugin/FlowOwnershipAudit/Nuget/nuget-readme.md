# How to create a nuget package for XrmToolbox

copy nuget.exe inside the plugin project folder or add it to the PATH environment variable. Then navigate to the plugin project folder and run the following command:

```
.\nuget.exe pack .\FlowOwnershipAudit.nuspec
```

This will create a nuget package in the same folder as the plugin project. You can then upload this package to nuget.org.
Since this plugin has already been published on the xrmtoolbox store, you do not need to do anything else, it will be updated automatically.
