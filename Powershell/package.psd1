@{
    Root = 'd:\Code\microsoft\PACE_WhatBreaksIfIRemoveThatUser\Powershell\PACE - Wrapper.ps1'
    OutputPath = 'd:\Code\microsoft\PACE_WhatBreaksIfIRemoveThatUser\Powershell\out'
    Package = @{
        Enabled = $true
        Obfuscate = $false
        HideConsoleWindow = $false
        DotNetVersion = 'v4.6.2'
        FileVersion = '1.0.0'
        FileDescription = ''
        ProductName = ''
        ProductVersion = ''
        Copyright = ''
        RequireElevation = $false
        ApplicationIconPath = ''
        PackageType = 'Console'
    }
    Bundle = @{
        Enabled = $true
        Modules = $true
        # IgnoredModules = @()
    }
}
        