#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell
#https://learn.microsoft.com/en-us/powershell/module/microsoft.powerapps.administration.powershell/get-adminpowerappconnection?view=pa-ps-latest

$owner=$args[0]

if (-not (Get-Module Microsoft.PowerApps.Administration.PowerShell -ListAvailable)) {
    Install-Module -Name Microsoft.PowerApps.Administration.PowerShell -Scope CurrentUser
}

if (-not (Get-Module Microsoft.PowerApps.PowerShell -ListAvailable)) {
    Install-Module -Name Microsoft.PowerApps.PowerShell -AllowClobber -Scope CurrentUser
}

if (-not (Get-Module Join-Object -ListAvailable)) {
    Install-Module -Name Join-Object
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