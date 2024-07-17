param (
    [string]$owner = "admin@MngEnvMCAP275987.onmicrosoft.com",
    [bool]$queryAllEnvironments = $false,
    [string]$targetEnvironment,
    [bool]$outputAsCSV = $true,
    [bool]$skipDependencyCheck = $true,
    [string]$csvPath = (Join-Path -Path (Get-Location) -ChildPath "\output\")
 )

#region check parameters
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

$job = Start-Job -FilePath .\Get-FlowOwners.ps1 -ArgumentList $owner, $targetEnvironment, $skipDependencyCheck

$Label = "Working"
$symbols = @("⣾⣿", "⣽⣿", "⣻⣿", "⢿⣿", "⡿⣿", "⣟⣿", "⣯⣿", "⣷⣿",
    "⣿⣾", "⣿⣽", "⣿⣻", "⣿⢿", "⣿⡿", "⣿⣟", "⣿⣯", "⣿⣷")
$i = 0;

Write-Host $PSVersionTable.PSVersion

while ($job.State -eq "Running") {
    $symbol = $symbols[$i]
    Write-Host -NoNewLine "`r$symbol $Label" -ForegroundColor Green
    Start-Sleep -Milliseconds 100
    $i++
    if ($i -eq $symbols.Count) {
        $i = 0;
    }   
}

$out = $job | Receive-Job -Keep
$out | Sort-Object -Property Flow_DisplayName | Format-Table -Property FlowName, Flow_DisplayName, Flow_CreatedTime, Environment_DisplayName -Wrap