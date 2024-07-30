
# this is the main entry point that wraps all subordinate scripts. Use this script to call all other functionality unless you want to work with a specific script directly.

#region set up params
param (
    [string]$owner = "admin@MngEnvMCAP275987.onmicrosoft.com",
    [bool]$queryAllEnvironments = $false,
    [string]$targetEnvironment,
    [bool]$outputAsCSV = $true,
    [bool]$skipDependencyCheck = $true,
    [string]$csvPath = (Join-Path -Path (Get-Location) -ChildPath "\output\")
 )

# send welcome message
Write-Host "Welcome to the 'What breaks if I disable this user?' program. This program will help you identify all the connections and flows that will break if you disable a user."

# check dependencies
Write-Host "Checking dependencies..."
.\checkDependencies.ps1

# check target owner parameter
 if ($owner -eq "") {
    $owner = Read-Host "Please enter the owners email address"
 }

 # check parameters for output preferences
if ($outputAsCSV -eq $true) {
    if ($csvPath -eq "") {
        $csvPath = Read-Host "Please enter the path to save the CSV file"
    }
    Write-Verbose "Will output as csv to $csvPath"

    # create the output folder if it doesn't exist
    if (-not (Test-Path -Path $csvPath)) {
        New-Item -Path $csvPath -ItemType Directory | Out-Null   
    }
}
else {
    Write-Verbose "Will output as table to console"
}

# check parameter for target environment
if ($queryAllEnvironments -eq $false -and $targetEnvironment -eq "") {
    Write-Debug "No target environment specified. Will list all environments so the user can choose one."
    # get a list of Powerplatform environments that this user can access
    $environments = @(Get-AdminPowerAppEnvironment | Select-Object  EnvironmentName, DisplayName)

    # ask the user to choose one of the environments
    $targetEnvironment = $environments | Out-GridView -OutputMode Single -Title "Choose an environment to work with" | Select-Object -ExpandProperty EnvironmentName
    Write-Verbose "User selected environment $targetEnvironment"
}
#endregion

## --------------------- process connections
# always skip dependency check, we did that already
$jobConnections = Start-Job -FilePath .\Get-ConnectionOwners.ps1 -ArgumentList $owner, $targetEnvironment, $true

## --------------------- process flows
# always skip dependency check, we did that already
$jobFlows = Start-Job -FilePath .\Get-FlowOwners.ps1 -ArgumentList $owner, $targetEnvironment, $true

## --------------------- show awesome progress animation
$Label = "Working"
$symbols = @("⣾⣿", "⣽⣿", "⣻⣿", "⢿⣿", "⡿⣿", "⣟⣿", "⣯⣿", "⣷⣿",
    "⣿⣾", "⣿⣽", "⣿⣻", "⣿⢿", "⣿⡿", "⣿⣟", "⣿⣯", "⣿⣷")
$i = 0;

while (Get-Job -State Running) { # means any of the jobs is still running
    $symbol = $symbols[$i]
    Write-Host -NoNewLine "`r$symbol $Label" -ForegroundColor Green
    Start-Sleep -Milliseconds 100
    $i++
    if ($i -eq $symbols.Count) {
        $i = 0;
    }   
}

## --------------------- process job results
# connections
$outConnections = $jobConnections | Receive-Job -Keep

# format output according to user preference
if ($outputAsCSV -eq $true) {
    $outConnections | Export-Csv -Path "$csvPath\connections_$( Get-Date -format "ddMMyy_HHmm").csv" -NoTypeInfo -UseCulture
}
else {
    $outConnections | Sort-Object -Property DisplayName | Format-Table -Property DisplayName, ConnectorName, ConnectionName, Status, CreatedTime, Environment_DisplayName -Wrap
}

# flows
$outFlows = $jobFlows | Receive-Job -Keep

if ($outputAsCSV -eq $true) {
    $outFlows | Export-Csv -Path "$csvPath\flows_$( Get-Date -format "ddMMyy_HHmm").csv" -NoTypeInfo -UseCulture
}
else {
    $outFlows | Sort-Object -Property Flow_DisplayName | Format-Table -Property FlowName, Flow_DisplayName, Flow_CreatedTime, Environment_DisplayName -Wrap
}

Write-Host "All done."

# wait for confirmation before closing the window
Read-Host "Press Enter to exit"