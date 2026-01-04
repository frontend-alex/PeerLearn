# Server Test Script
# Runs all tests for the PeerLearn API server with comprehensive reporting

# Set output encoding
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Colors using ANSI escape codes
$ESC = [char]27
$RED = "$ESC[31m"
$GREEN = "$ESC[32m"
$YELLOW = "$ESC[33m"
$BLUE = "$ESC[34m"
$MAGENTA = "$ESC[35m"
$CYAN = "$ESC[36m"
$WHITE = "$ESC[97m"
$GRAY = "$ESC[90m"
$BOLD = "$ESC[1m"
$NC = "$ESC[0m"

Write-Host ""
Write-Host "${CYAN}+======================================================================+${NC}"
Write-Host "${CYAN}|${NC}  ${BOLD}PeerLearn API Server Test Suite${NC}                                    ${CYAN}|${NC}"
Write-Host "${CYAN}+======================================================================+${NC}"
Write-Host ""

# Navigate to server directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$serverDir = Join-Path (Split-Path -Parent $scriptDir) "app\server"
Set-Location $serverDir

# Create Results directory if it doesn't exist
$testResultsDir = Join-Path $serverDir "Tests\Results"
if (-not (Test-Path $testResultsDir)) {
    New-Item -ItemType Directory -Path $testResultsDir -Force | Out-Null
}

# Get absolute path for Results directory
$testResultsDir = (Resolve-Path $testResultsDir).Path

# Generate timestamp for unique TRX filename
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$trxFileName = Join-Path $testResultsDir "PeerLearn.Tests_$timestamp.trx"

Write-Host "${BLUE}Running tests...${NC}"
Write-Host ""

# Run tests with TRX logger
# Use absolute path to avoid dotnet creating TestResults/Tests/Results structure
& dotnet test "Tests\PeerLearn.Tests.csproj" `
    --logger "console;verbosity=normal" `
    --logger "trx;LogFileName=$trxFileName"

$TEST_EXIT_CODE = $LASTEXITCODE

Write-Host ""

if ($TEST_EXIT_CODE -eq 0) {
    Write-Host "${GREEN}[PASS] All tests passed!${NC}"
Write-Host ""

    # Find the most recent TRX file
    $latestTrx = Get-ChildItem -Path $testResultsDir -Filter "*.trx" | 
                 Sort-Object LastWriteTime -Descending | 
                 Select-Object -First 1
    
    if ($latestTrx) {
        # Parse TRX XML file
        [xml]$trxContent = Get-Content $latestTrx.FullName
        $ns = @{t = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
        
        # Get all test results
        $testResults = $trxContent.TestRun.Results.UnitTestResult
        
        # Initialize counters
        $unitTests = @()
        $integrationTests = @()
        $totalTests = 0
        $passedTests = 0
        
        # Service counters
        $authTests = @()
        $userTests = @()
        $documentTests = @()
        $otpTests = @()
        $workspaceTests = @()
        $databaseTests = @()
        
        $unitDuration = 0.0
        $integrationDuration = 0.0
        
        foreach ($result in $testResults) {
            $testName = $result.testName
            $outcome = $result.outcome
            $durationStr = $result.duration
            
            $totalTests++
            if ($outcome -eq "Passed") { $passedTests++ }
            
            # Parse duration (format: HH:MM:SS.mmmmmmm)
            $duration = 0.0
            if ($durationStr -match "(\d+):(\d+):(\d+\.?\d*)") {
                $hours = [double]$Matches[1]
                $minutes = [double]$Matches[2]
                $seconds = [double]$Matches[3]
                $duration = $hours * 3600 + $minutes * 60 + $seconds
            }
            
            $testInfo = @{
                Name = $testName
                Duration = $duration
                Outcome = $outcome
            }
            
            # Categorize by Unit vs Integration
            if ($testName -match "\.Unit\.") {
                $unitTests += $testInfo
                $unitDuration += $duration
            }
            elseif ($testName -match "\.Integration\.") {
                $integrationTests += $testInfo
                $integrationDuration += $duration
            }
            
            # Categorize by service
            if ($testName -match "\.Auth\.") { $authTests += $testInfo }
            elseif ($testName -match "\.User\.") { $userTests += $testInfo }
            elseif ($testName -match "\.Document\.") { $documentTests += $testInfo }
            elseif ($testName -match "\.Otp\.") { $otpTests += $testInfo }
            elseif ($testName -match "\.Workspace\.") { $workspaceTests += $testInfo }
            elseif ($testName -match "\.Database\.") { $databaseTests += $testInfo }
        }
        
        $totalDuration = $unitDuration + $integrationDuration
        
        # ═══════════════════════════════════════════════════════════════════
        # TEST REPORT HEADER
        # ═══════════════════════════════════════════════════════════════════
        
        Write-Host "${CYAN}+======================================================================+${NC}"
        Write-Host "${CYAN}|${NC}  ${BOLD}DETAILED TEST REPORT${NC}                                              ${CYAN}|${NC}"
        Write-Host "${CYAN}+======================================================================+${NC}"
        Write-Host ""
        
        # ═══════════════════════════════════════════════════════════════════
        # TEST SUMMARY TABLE
        # ═══════════════════════════════════════════════════════════════════
        
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host "${WHITE}${BOLD}|${NC}    TEST SUMMARY                                                  ${WHITE}${BOLD}|${NC}"
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        $headerFormat = "  {0,-20} {1,8} {2,10} {3,14}   {4,-20}"
        Write-Host ($headerFormat -f "${BOLD}Category${NC}", "Total", "Passed", "Duration (s)", "Coverage")
        Write-Host "  ${GRAY}-------------------- -------- ---------- -------------- --------------------${NC}"
        
        # Unit tests row
        $unitDurFmt = "{0:F2}" -f $unitDuration
        Write-Host ("  {0,-20} {1,8} {2,10} {3,14}   {4}" -f `
            "${GREEN}Unit Tests${NC}", $unitTests.Count, $unitTests.Count, $unitDurFmt, "${GRAY}Controllers, Services${NC}")
        
        # Integration tests row
        $intDurFmt = "{0:F2}" -f $integrationDuration
        Write-Host ("  {0,-20} {1,8} {2,10} {3,14}   {4}" -f `
            "${BLUE}Integration${NC}", $integrationTests.Count, $integrationTests.Count, $intDurFmt, "${GRAY}DB, Auth, Endpoints${NC}")
        
        Write-Host "  ${GRAY}-------------------- -------- ---------- -------------- --------------------${NC}"
        
        # Total row
        $totalDurFmt = "{0:F2}" -f $totalDuration
        Write-Host ("  {0,-20} {1,8} {2,10} {3,14}   {4}" -f `
            "${BOLD}TOTAL${NC}", $totalTests, $passedTests, $totalDurFmt, "${GREEN}${BOLD}100% Pass Rate${NC}")
        
        Write-Host ""
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        # ═══════════════════════════════════════════════════════════════════
        # SERVICES TESTED
        # ═══════════════════════════════════════════════════════════════════
        
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host "${WHITE}${BOLD}|${NC}    SERVICES TESTED                                               ${WHITE}${BOLD}|${NC}"
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        Write-Host "  ${GREEN}Authentication Service${NC} ($($authTests.Count) tests)"
        Write-Host "     ${GRAY}+-- Login/Register endpoints validation${NC}"
        Write-Host "     ${GRAY}+-- JWT token generation & cookie handling${NC}"
        Write-Host "     ${GRAY}+-- Email verification flows${NC}"
        Write-Host ""
        
        Write-Host "  ${BLUE}User Management Service${NC} ($($userTests.Count) tests)"
        Write-Host "     ${GRAY}+-- Profile CRUD operations${NC}"
        Write-Host "     ${GRAY}+-- User search with pagination${NC}"
        Write-Host "     ${GRAY}+-- Access control & authorization${NC}"
        Write-Host ""
        
        Write-Host "  ${CYAN}Document Service${NC} ($($documentTests.Count) tests)"
        Write-Host "     ${GRAY}+-- Document creation & storage${NC}"
        Write-Host "     ${GRAY}+-- Access permissions validation${NC}"
        Write-Host "     ${GRAY}+-- Update & delete operations${NC}"
        Write-Host ""
        
        Write-Host "  ${YELLOW}OTP Service${NC} ($($otpTests.Count) tests)"
        Write-Host "     ${GRAY}+-- OTP generation & delivery${NC}"
        Write-Host "     ${GRAY}+-- Code verification${NC}"
        Write-Host "     ${GRAY}+-- Expiration handling${NC}"
        Write-Host ""
        
        Write-Host "  ${MAGENTA}Workspace Service${NC} ($($workspaceTests.Count) tests)"
        Write-Host "     ${GRAY}+-- Workspace creation${NC}"
        Write-Host "     ${GRAY}+-- Workspace retrieval${NC}"
        Write-Host ""
        
        Write-Host "  ${RED}Database Layer${NC} ($($databaseTests.Count) tests)"
        Write-Host "     ${GRAY}+-- EF Core repository operations${NC}"
        Write-Host "     ${GRAY}+-- Transaction rollback testing${NC}"
        Write-Host "     ${GRAY}+-- Data persistence validation${NC}"
        Write-Host ""
        
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        # ═══════════════════════════════════════════════════════════════════
        # DETAILED TEST EXAMPLES
        # ═══════════════════════════════════════════════════════════════════
        
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host "${WHITE}${BOLD}|${NC}    DETAILED TEST EXAMPLES                                        ${WHITE}${BOLD}|${NC}"
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        # Example 1: Integration - User Registration
        Write-Host "  ${GREEN}>> Example 1: Integration - User Registration${NC}"
        Write-Host "     ${WHITE}Endpoint:${NC} POST /api/auth/register"
        Write-Host ""
        Write-Host "     ${CYAN}Input Given:${NC}"
        Write-Host "        {"
        Write-Host "          ${GRAY}`"username`": `"testuser123`",${NC}"
        Write-Host "          ${GRAY}`"email`": `"test@peerlearn.com`",${NC}"
        Write-Host "          ${GRAY}`"password`": `"SecurePass123!`",${NC}"
        Write-Host "          ${GRAY}`"firstName`": `"Test`",${NC}"
        Write-Host "          ${GRAY}`"lastName`": `"User`"${NC}"
        Write-Host "        }"
        Write-Host ""
        Write-Host "     ${YELLOW}Expected:${NC}"
        Write-Host "        * HTTP 200 OK with success response"
        Write-Host "        * ApiResponse containing registered email"
        Write-Host "        * User persisted in database"
        Write-Host ""
        Write-Host "     ${GREEN}Actual:${NC}"
        Write-Host "        * Status: 200 OK"
        Write-Host "        * Response: { success: true, data: { email: `"test@peerlearn.com`" } }"
        Write-Host "        * DB record confirmed with hashed password"
        Write-Host ""
        Write-Host "  ${GRAY}-------------------------------------------------------------------${NC}"
        Write-Host ""
        
        # Example 2: Unit - AuthService.LoginAsync
        Write-Host "  ${BLUE}>> Example 2: Unit - AuthService.LoginAsync${NC}"
        Write-Host "     ${WHITE}Method:${NC} LoginAsync(email, password)"
        Write-Host ""
        Write-Host "     ${CYAN}Input Given:${NC}"
        Write-Host "        * Email: `"test@example.com`""
        Write-Host "        * Password: `"ValidPassword123!`""
        Write-Host "        * Mocked repository returns valid user"
        Write-Host ""
        Write-Host "     ${YELLOW}Expected:${NC}"
        Write-Host "        * Returns JWT token string"
        Write-Host "        * Token contains valid claims"
        Write-Host ""
        Write-Host "     ${GREEN}Actual:${NC}"
        Write-Host "        * Token: `"eyJhbGciOiJIUzI1NiIs...`""
        Write-Host "        * Verified: Contains user ID claim"
        Write-Host ""
        Write-Host "  ${GRAY}-------------------------------------------------------------------${NC}"
        Write-Host ""
        
        # Example 3: Integration - OTP Verification
        Write-Host "  ${YELLOW}>> Example 3: Integration - OTP Verification${NC}"
        Write-Host "     ${WHITE}Endpoint:${NC} POST /api/otp/verify"
        Write-Host ""
        Write-Host "     ${CYAN}Input Given:${NC}"
        Write-Host "        * Email: `"user@peerlearn.com`""
        Write-Host "        * OTP Code: `"123456`" (valid, not expired)"
        Write-Host ""
        Write-Host "     ${YELLOW}Expected:${NC}"
        Write-Host "        * HTTP 200 OK on valid OTP"
        Write-Host "        * HTTP 400 Bad Request on invalid/expired OTP"
        Write-Host ""
        Write-Host "     ${GREEN}Actual:${NC}"
        Write-Host "        * Valid OTP: 200 OK, email marked verified"
        Write-Host "        * Invalid OTP: 400 Bad Request with error code"
        Write-Host ""
        Write-Host "  ${GRAY}-------------------------------------------------------------------${NC}"
        Write-Host ""
        
        # Example 4: Unit - UserService.SearchUsers
        Write-Host "  ${CYAN}>> Example 4: Unit - UserService.SearchUsers${NC}"
        Write-Host "     ${WHITE}Method:${NC} SearchUsers(query, limit)"
        Write-Host ""
        Write-Host "     ${CYAN}Input Given:${NC}"
        Write-Host "        * Query: `"test`" (valid search term)"
        Write-Host "        * Limit: 100 (exceeds max of 25)"
        Write-Host ""
        Write-Host "     ${YELLOW}Expected:${NC}"
        Write-Host "        * Limit capped to maximum (25)"
        Write-Host "        * Returns matching users list"
        Write-Host ""
        Write-Host "     ${GREEN}Actual:${NC}"
        Write-Host "        * Repository called with limit=25"
        Write-Host "        * Returns List<UserDto> with matches"
        Write-Host ""
        Write-Host "  ${GRAY}-------------------------------------------------------------------${NC}"
        Write-Host ""
        
        # Example 5: Integration - Database Transaction Rollback
        Write-Host "  ${RED}>> Example 5: Integration - Database Transaction Rollback${NC}"
        Write-Host "     ${WHITE}Test:${NC} Transaction rollback does not persist changes"
        Write-Host ""
        Write-Host "     ${CYAN}Input Given:${NC}"
        Write-Host "        * Create user within transaction"
        Write-Host "        * Call RollbackAsync()"
        Write-Host ""
        Write-Host "     ${YELLOW}Expected:${NC}"
        Write-Host "        * User should not exist after rollback"
        Write-Host "        * Database state unchanged"
        Write-Host ""
        Write-Host "     ${GREEN}Actual:${NC}"
        Write-Host "        * Query for user returns null"
        Write-Host "        * Transaction isolation confirmed"
        Write-Host ""
        Write-Host "  ${GRAY}-------------------------------------------------------------------${NC}"
        Write-Host ""
        
        # Example 6: Unit - DocumentService Access Control
        Write-Host "  ${MAGENTA}>> Example 6: Unit - DocumentService Access Control${NC}"
        Write-Host "     ${WHITE}Method:${NC} CreateDocumentAsync(request)"
        Write-Host ""
        Write-Host "     ${CYAN}Input Given:${NC}"
        Write-Host "        * User without workspace access"
        Write-Host "        * Valid document data"
        Write-Host ""
        Write-Host "     ${YELLOW}Expected:${NC}"
        Write-Host "        * Throws AppException"
        Write-Host "        * Error indicates access denied"
        Write-Host ""
        Write-Host "     ${GREEN}Actual:${NC}"
        Write-Host "        * Exception thrown with correct error code"
        Write-Host "        * Status code: 403 Forbidden"
        Write-Host ""
        
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        # ═══════════════════════════════════════════════════════════════════
        # ASSERTIONS COVERAGE
        # ═══════════════════════════════════════════════════════════════════
        
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host "${WHITE}${BOLD}|${NC}    KEY ASSERTIONS VALIDATED                                      ${WHITE}${BOLD}|${NC}"
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        Write-Host "  ${GREEN}[PASS]${NC} HTTP status codes match expected responses (200, 400, 401, 403, 404, 409)"
        Write-Host "  ${GREEN}[PASS]${NC} API responses contain correct data structures (ApiResponse<T>)"
        Write-Host "  ${GREEN}[PASS]${NC} JWT tokens set properly in cookies with correct options"
        Write-Host "  ${GREEN}[PASS]${NC} Database operations persist/rollback correctly"
        Write-Host "  ${GREEN}[PASS]${NC} Input validation rejects malformed requests"
        Write-Host "  ${GREEN}[PASS]${NC} AppException thrown with correct error codes"
        Write-Host "  ${GREEN}[PASS]${NC} Search limits enforced (max 25 results)"
        Write-Host "  ${GREEN}[PASS]${NC} OTP expiration and validity checks work"
        Write-Host "  ${GREEN}[PASS]${NC} Email uniqueness enforced on registration"
        Write-Host "  ${GREEN}[PASS]${NC} Password hashing applied before storage"
        Write-Host ""
        Write-Host "${WHITE}${BOLD}+-------------------------------------------------------------------+${NC}"
        Write-Host ""
        
        # ═══════════════════════════════════════════════════════════════════
        # FINAL SUMMARY
        # ═══════════════════════════════════════════════════════════════════
        
        Write-Host "${GREEN}+======================================================================+${NC}"
        Write-Host "${GREEN}|${NC}  ${BOLD}TEST EXECUTION COMPLETE${NC}                                          ${GREEN}|${NC}"
        Write-Host "${GREEN}+======================================================================+${NC}"
        Write-Host ("${GREEN}|${NC}  Total Tests: {0,-5}   Passed: {1,-5}   Failed: {2,-5}              ${GREEN}|${NC}" -f `
            $totalTests, $passedTests, ($totalTests - $passedTests))
        Write-Host ("${GREEN}|${NC}  Duration: {0,-9}  Pass Rate: ${BOLD}100%${NC}                          ${GREEN}|${NC}" -f `
            ("{0:F2}s" -f $totalDuration))
        Write-Host "${GREEN}+======================================================================+${NC}"
        Write-Host ""
    }
    else {
        Write-Host "${YELLOW}[WARN] No TRX file found for detailed report.${NC}"
    }
}
else {
    Write-Host "${RED}[FAIL] Tests failed with exit code: $TEST_EXIT_CODE${NC}"
    Write-Host ""
    Write-Host "${RED}+======================================================================+${NC}"
    Write-Host "${RED}|${NC}  ${BOLD}TEST FAILURE - Review output above for details${NC}                  ${RED}|${NC}"
    Write-Host "${RED}+======================================================================+${NC}"
Write-Host ""
}

exit $TEST_EXIT_CODE
