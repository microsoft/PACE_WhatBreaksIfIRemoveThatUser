#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell
#https://learn.microsoft.com/en-us/powershell/module/microsoft.powerapps.administration.powershell/get-adminpowerappconnection?view=pa-ps-latest

#$owner=$args[0]
$owner="laurenva@partner.eursc.eu"

if (-not (Get-Module Microsoft.PowerApps.Administration.PowerShell -ListAvailable)) {
    Install-Module -Name Microsoft.PowerApps.Administration.PowerShell -Scope CurrentUser
}

if (-not (Get-Module Microsoft.PowerApps.PowerShell -ListAvailable)) {
    Install-Module -Name Microsoft.PowerApps.PowerShell -AllowClobber -Scope CurrentUser
}

if (-not (Get-Module Join-Object -ListAvailable)) {
    Install-Module -Name Join-Object
}

$ownerId = Get-UsersOrGroupsFromGraph -SearchString $owner | Select-Object -Property ObjectId

$environments = @(Get-AdminPowerAppEnvironment | Select-Object  EnvironmentName, DisplayName)

$allFlows = @(Get-AdminFlow | ForEach-Object {
    $flow = $_
    $userId = $flow.CreatedBy.userId
    [PSCustomObject]@{
        DisplayName = $flow.DisplayName
        CreatedTime = $flow.CreatedTime
        Enabled = $flow.Enabled
        EnvironmentName = $flow.EnvironmentName
        UserId = [string]$userId
    }
})

#ParameterSet is not correctly choosen
#Blanks check is present in source, passing in blanks will make the function ignore the values 
$empty = " "
$flowOwnerRole = Get-AdminFlowOwnerRole -Owner "f673861a-b501-46e5-af77-abdfe213ece3" -Environment $empty -FlowName $empty

Write-Host "Merging Connection information with Environment information"
#$joined = Join-Object -Left $connections -Right $environments -LeftJoinProperty "EnvironmentName" -RightJoinProperty "EnvironmentName" -ExcludeRightProperties 'EnvironmentName' -Prefix 'Environment_'
#$joined | Format-Table

