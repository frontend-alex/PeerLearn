# Server Test Script (PowerShell)
# Runs all integration tests for the PeerLearn API server

Write-Host "Running PeerLearn API Tests..." -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$serverPath = Join-Path $scriptPath "..\app\server"

Set-Location $serverPath

# Run tests
dotnet test Tests/PeerLearn.Tests.csproj --logger "console;verbosity=normal"

$testExitCode = $LASTEXITCODE

Write-Host ""
if ($testExitCode -eq 0) {
    Write-Host "All tests passed!" -ForegroundColor Green
} else {
    Write-Host "Tests failed with exit code: $testExitCode" -ForegroundColor Red
}

exit $testExitCode
