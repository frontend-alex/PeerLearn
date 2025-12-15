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
Write-Host "==============================" -ForegroundColor Cyan
if ($testExitCode -eq 0) {
    Write-Host "SUCCESS: All tests passed!" -ForegroundColor Green
    Write-Host "Exit Code: $testExitCode" -ForegroundColor Green
} else {
    Write-Host "FAILED: Tests failed!" -ForegroundColor Red
    Write-Host "Exit Code: $testExitCode" -ForegroundColor Red
}
Write-Host "==============================" -ForegroundColor Cyan

exit $testExitCode
