#https://learn.microsoft.com/en-us/power-platform/admin/powerapps-powershell


$owner = $args[0]
$environment=$args[1]
$skipDependencyCheck = $args[2]

if ($skipDependencyCheck -eq $false) {
    # run checkDependencies.ps1 to ensure all dependencies are met
    .\checkDependencies.ps1
}

$ownerId = Get-UsersOrGroupsFromGraph -SearchString $owner | Select-Object -Property ObjectId

if ($environment -eq "") {
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

}
else {
   # run only for the specified environment
   $flowOwnerRole = Get-AdminFlowOwnerRole -Owner $ownerId.ObjectId -EnvironmentName $targetEnvironment -FlowName $empty | Where-Object { $_.RoleType -eq 'Owner' }

   if ($flowOwnerRole.Count -eq 0) {
    Write-Host "No Flow Owner Role found for $owner"
    exit
    }
    
    $allFlows = @(Get-AdminFlow -EnvironmentName $targetEnvironment | ForEach-Object {
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

    $environments = @(Get-AdminPowerAppEnvironment -EnvironmentName $targetEnvironment | Select-Object  EnvironmentName, DisplayName)
    
    #Need to merge the flow information with the environment information
    $allFlowsWithEnvironment = Join-Object -Left $flowOwnerRole -Right $environments -LeftJoinProperty "EnvironmentName" -RightJoinProperty "EnvironmentName" -ExcludeRightProperties 'EnvironmentName' -Prefix 'Environment_'

    Join-Object -Left $allFlowsWithEnvironment -Right $allFlows -LeftJoinProperty "FlowName" -RightJoinProperty "FlowId" -Prefix 'Flow_'
}