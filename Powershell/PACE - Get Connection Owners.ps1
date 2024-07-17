# set up params
param (
    [string]$owner = "admin@MngEnvMCAP275987.onmicrosoft.com",
    [bool]$outputAsCSV = $false,
    [string]$csvPath
 )

# ask for parameters interactively if they are not provided
 if ($owner -eq "") {
    $owner = Read-Host "Please enter the owners email address"
 }
if ($outputAsCSV -eq $true) {
    if ($csvPath -eq "") {
        $csvPath = Read-Host "Please enter the path to save the CSV file"
    }
    Write-Verbose "Will output as csv to $csvPath"
}
else {
    Write-Verbose "Will output as table to console"
}



$job = Start-Job -FilePath .\Get-ConnectionOwners.ps1 -ArgumentList $owner

$Label = "Working"
$symbols = @("⣾⣿", "⣽⣿", "⣻⣿", "⢿⣿", "⡿⣿", "⣟⣿", "⣯⣿", "⣷⣿",
    "⣿⣾", "⣿⣽", "⣿⣻", "⣿⢿", "⣿⡿", "⣿⣟", "⣿⣯", "⣿⣷")
$i = 0;

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

# format output according to user preference
if ($outputAsCSV -eq $true) {
    $out | Export-Csv -Path $csvPath -NoTypeInfo -UseCulture
}
else {
    $out | Sort-Object -Property DisplayName | Format-Table -Property DisplayName, ConnectorName, ConnectionName, Status, CreatedTime, Environment_DisplayName -Wrap
}