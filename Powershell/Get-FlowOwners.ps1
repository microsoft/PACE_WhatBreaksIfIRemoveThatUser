#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell
#https://learn.microsoft.com/en-us/powershell/module/microsoft.powerapps.administration.powershell/get-adminpowerappconnection?view=pa-ps-latest

$owner = $args[0]

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

$ownerId = Get-UsersOrGroupsFromGraph -SearchString $owner | Select-Object -Property ObjectId

#ParameterSet is not correctly choosen
#Blanks check is present in source, passing in blanks will make the function ignore the values 
$empty = " "
$flowOwnerRole = Get-AdminFlowOwnerRole -Owner $ownerId.ObjectId -Environment $empty -FlowName $empty | Where-Object { $_.RoleType -eq 'Owner' }

$environments = @(Get-AdminPowerAppEnvironment | Select-Object  EnvironmentName, DisplayName)

if ($flowOwnerRole.Count -eq 0) {
    Write-Host "No Flow Owner Role found for $owner"
    exit
}

$allFlows = @(Get-AdminFlow | ForEach-Object {
        $flow = $_
        $userId = $flow.CreatedBy.userId
        [PSCustomObject]@{
            DisplayName = $flow.DisplayName
            CreatedTime = $flow.CreatedTime
            #Enabled = $flow.Enabled
            #EnvironmentName = $flow.EnvironmentName
            UserId      = [string]$userId
            FlowId      = [string]$flow.FlowName
        }
    } | Sort-Object -Property FlowId)

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
