#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell
#https://learn.microsoft.com/en-us/powershell/module/microsoft.powerapps.administration.powershell/get-adminpowerappconnection?view=pa-ps-latest

$owner=$args[0]
#$owner="manchesa@eursc.eu"
#$owner="laurenva@partner.eursc.eu"

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

#ParameterSet is not correctly choosen
#Blanks check is present in source, passing in blanks will make the function ignore the values 
$empty = " "
$flowOwnerRole = Get-AdminFlowOwnerRole -Owner $ownerId.ObjectId -Environment $empty  -FlowName $empty  | Where-Object { $_.RoleType -eq 'Owner' }

if ($flowOwnerRole.Count -eq 0) {
    Write-Host "No Flow Owner Role found for $owner"
    exit
}

$environments = @(Get-AdminPowerAppEnvironment | Select-Object  EnvironmentName, DisplayName)

$allFlows = @(Get-AdminFlow | ForEach-Object {
    $flow = $_
    $userId = $flow.CreatedBy.userId
    [PSCustomObject]@{
        DisplayName = $flow.DisplayName
        CreatedTime = $flow.CreatedTime
        #Enabled = $flow.Enabled
        #EnvironmentName = $flow.EnvironmentName
        UserId = [string]$userId
        FlowId = [string]$flow.FlowName
    }
}| Sort-Object -Property FlowId)

#Need to merge the flow information with the environment information
$allFlowsWithEnvironment = Join-Object -Left $flowOwnerRole -Right $environments -LeftJoinProperty "EnvironmentName" -RightJoinProperty "EnvironmentName" -ExcludeRightProperties 'EnvironmentName' -Prefix 'Environment_'

#Need to merge the flow owner role with the flow information to get the display name of the flow
#$allFlowsWithEnvironmentAndFlowName = Join-Object -Left $allFlowsWithEnvironment -Right $allFlows -LeftJoinProperty "FlowName" -RightJoinProperty "FlowId" -Prefix 'Flow_'
Join-Object -Left $allFlowsWithEnvironment -Right $allFlows -LeftJoinProperty "FlowName" -RightJoinProperty "FlowId" -Prefix 'Flow_'

#Need to filter the list to only show the flows that the user is the owner of
#$finalList = $allFlowsWithEnvironmentAndFlowName | Where-Object { $flowOwnerRole.FlowName -contains $_.'FlowName' }
#$allFlowsWithEnvironmentAndFlowName | Format-Table

#Write-Host "Merging Connection information with Environment information"
#$joined = Join-Object -Left $connections -Right $environments -LeftJoinProperty "EnvironmentName" -RightJoinProperty "EnvironmentName" -ExcludeRightProperties 'EnvironmentName' -Prefix 'Environment_'
#$joined | Format-Table

