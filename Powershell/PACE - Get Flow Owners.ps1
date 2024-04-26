$owner = "laurenva@partner.eursc.eu"
$job = Start-Job -FilePath .\Get-FlowOwners.ps1 -ArgumentList $owner

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
$out | Format-Table -Property DisplayName, ConnectorName, ConnectionName, Status, CreatedTime, Environment_DisplayName -Wrap