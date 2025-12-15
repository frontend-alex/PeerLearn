# Server Test Script (PowerShell)
# Runs all tests for the PeerLearn API server, with teacher-friendly output.

Write-Host "Running PeerLearn API Tests..." -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptPath "..")
$serverPath = Resolve-Path (Join-Path $repoRoot "app\server")
$resultsDir = Join-Path $serverPath "Tests\TestResults"

if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir | Out-Null
}

Set-Location $serverPath

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$trxFileName = "PeerLearn.Tests_$timestamp.trx"

# Run tests (quiet console, plus a TRX report for visualization)
dotnet test Tests/PeerLearn.Tests.csproj `
    --nologo `
    --logger "console;verbosity=minimal" `
    --logger "trx;LogFileName=$trxFileName" `
    --results-directory "$resultsDir"

$testExitCode = $LASTEXITCODE
$trxPath = Join-Path $resultsDir $trxFileName

Write-Host ""
Write-Host "==============================" -ForegroundColor Cyan
Write-Host "Report (TRX): $trxPath" -ForegroundColor DarkGray
Write-Host "Open it (Windows): start `"$trxPath`"" -ForegroundColor DarkGray
Write-Host ""

# Optional: auto-open the TRX report after the run
# Usage (PowerShell):
#   $env:OPEN_TRX = "1"; ./scripts/test
if ($env:OPEN_TRX -eq "1" -and (Test-Path $trxPath)) {
    try { Start-Process -FilePath $trxPath | Out-Null } catch { }
}

function Show-TrxSummary([string] $path) {
    if (-not (Test-Path $path)) {
        Write-Host "No TRX found to summarize." -ForegroundColor Yellow
        return
    }

    try {
        [xml]$trx = Get-Content -Path $path
        $ns = New-Object System.Xml.XmlNamespaceManager($trx.NameTable)
        $ns.AddNamespace("t", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")

        $results = $trx.SelectNodes("//t:UnitTestResult", $ns)
        if ($null -eq $results) { $results = @() }

        $total = $results.Count
        $passed = ($results | Where-Object { $_.outcome -eq "Passed" }).Count
        $failed = ($results | Where-Object { $_.outcome -eq "Failed" }).Count
        $skipped = ($results | Where-Object { $_.outcome -eq "NotExecuted" }).Count

        Write-Host "Summary: total=$total passed=$passed failed=$failed skipped=$skipped" -ForegroundColor Cyan

        if ($failed -gt 0) {
            Write-Host ""
            Write-Host "Failed tests:" -ForegroundColor Red
            ($results | Where-Object { $_.outcome -eq "Failed" }) | ForEach-Object {
                Write-Host ("- " + $_.testName) -ForegroundColor Red
            }
        }
    }
    catch {
        Write-Host "Could not parse TRX summary: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Show-TrxSummary -path $trxPath

Write-Host ""
if ($testExitCode -eq 0) {
    Write-Host "SUCCESS: All tests passed!" -ForegroundColor Green
    Write-Host "Exit Code: $testExitCode" -ForegroundColor Green
} else {
    Write-Host "FAILED: Tests failed!" -ForegroundColor Red
    Write-Host "Exit Code: $testExitCode" -ForegroundColor Red
}
Write-Host "==============================" -ForegroundColor Cyan

Write-Host ""
Write-Host "How to visualize:" -ForegroundColor Cyan
Write-Host "- Visual Studio: Test Explorer can open TRX (double-click file)." -ForegroundColor DarkGray
Write-Host "- Rider: Open TRX / Unit Tests tool window." -ForegroundColor DarkGray

exit $testExitCode
