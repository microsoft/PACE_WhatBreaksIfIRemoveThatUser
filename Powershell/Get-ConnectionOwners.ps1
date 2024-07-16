#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell
#https://learn.microsoft.com/en-us/powershell/module/microsoft.powerapps.administration.powershell/get-adminpowerappconnection?view=pa-ps-latest

$owner=$args[0]

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

$environments = @(Get-AdminPowerAppEnvironment | Select-Object  EnvironmentName, DisplayName)

$connections = @(Get-AdminPowerAppConnection -CreatedBy $owner | ForEach-Object {
        $connection = $_
        $status = if ($connection.statuses -match 'Error') { 'Error' } else { $connection.statuses.status }
        [PSCustomObject]@{
            DisplayName     = $connection.DisplayName
            ConnectorName   = $connection.ConnectorName
            ConnectionName  = $connection.ConnectionName
            Status          = [string]$status
            CreatedTime     = $connection.CreatedTime
            EnvironmentName = $connection.EnvironmentName
        }
    })

Join-Object -Left $connections -Right $environments -LeftJoinProperty "EnvironmentName" -RightJoinProperty "EnvironmentName" -ExcludeRightProperties 'EnvironmentName' -Prefix 'Environment_'
#$joined | Format-Table
