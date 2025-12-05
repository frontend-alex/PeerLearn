$ErrorActionPreference = "Continue"

Write-Host "Running integration tests..." -ForegroundColor Cyan

$output = dotnet test --verbosity detailed 2>&1 | Out-String
Write-Host $output

# Save output to file
$output | Out-File "test-output.txt" -Encoding UTF8

Write-Host "`nTest output saved to test-output.txt" -ForegroundColor Green
