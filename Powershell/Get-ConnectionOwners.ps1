#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell
#https://learn.microsoft.com/en-us/powershell/module/microsoft.powerapps.administration.powershell/get-adminpowerappconnection?view=pa-ps-latest

$owner=$args[0]
$environment=$args[1]
$skipDependencyCheck = $args[2]

if ($skipDependencyCheck -eq $false) {
    # run checkDependencies.ps1 to ensure all dependencies are met
    .\checkDependencies.ps1
}

# if environment is empty, work with all environments
if ($environment -eq "") {

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
}
# target environment was specified, query only connections inside that environment
else {
    $connections = @(Get-AdminPowerAppConnection -EnvironmentName $environment -CreatedBy $owner | ForEach-Object {
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

    $connections
}