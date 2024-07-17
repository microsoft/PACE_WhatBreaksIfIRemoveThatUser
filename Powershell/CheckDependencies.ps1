if (-not (Get-Module Microsoft.PowerApps.Administration.PowerShell -ListAvailable)) {
    Install-Module -Name Microsoft.PowerApps.Administration.PowerShell -AllowClobber -Scope CurrentUser -Force
}
else {
    #check the version of the installed module and compare it to the latest version
    $installedVersion = (Get-Module -Name Microsoft.PowerApps.Administration.PowerShell -ListAvailable).Version
    $latestVersion = New-Object System.Version((Find-Module -Name Microsoft.PowerApps.Administration.PowerShell).Version)

    if ($installedVersion -lt $latestVersion) {
        Uninstall-Module -Name Microsoft.PowerApps.Administration.PowerShell -Force
        Install-Module -Name Microsoft.PowerApps.Administration.PowerShell -AllowClobber -Scope CurrentUser -Force 
    }
    else {
        Write-Host "Microsoft.PowerApps.Administration.PowerShell is up to date."
    }
}

if (-not (Get-Module Microsoft.PowerApps.PowerShell -ListAvailable)) {
    Install-Module -Name Microsoft.PowerApps.PowerShell -AllowClobber -Scope CurrentUser -Force
}
else {
    #check the version of the installed module and compare it to the latest version
    $installedVersion = (Get-Module -Name Microsoft.PowerApps.PowerShell -ListAvailable).Version
    $latestVersion = New-Object System.Version((Find-Module -Name Microsoft.PowerApps.PowerShell).Version)

    if ($installedVersion -lt $latestVersion) {
        Uninstall-Module -Name Microsoft.PowerApps.PowerShell -Force
        Install-Module -Name Microsoft.PowerApps.PowerShell -AllowClobber -Scope CurrentUser -Force 
    }
    else {
        Write-Host "Microsoft.PowerApps.PowerShell is up to date."
    }
}

if (-not (Get-Module Join-Object -ListAvailable)) {
    Install-Module -Name Join-Object
}
else {
    #check the version of the installed module and compare it to the latest version
    $installedVersion = (Get-Module -Name Join-Object -ListAvailable).Version
    $latestVersion = New-Object System.Version((Find-Module -Name Join-Object).Version)

    if ($installedVersion -lt $latestVersion) {
        Uninstall-Module -Name Join-Object -Force
        Install-Module -Name Join-Object -AllowClobber -Scope CurrentUser -Force
        #Write-Host "A newer version of the module is available."
    }
    else {
        Write-Host "Join-Object is up to date."
    }
}
Write-Host "All dependencies are met."