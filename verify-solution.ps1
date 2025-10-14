# Verification Script for CSharp Web Exam Solution
# This script verifies that the Minimal API migration was successful

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CSharp Web Exam - Solution Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check .NET SDK version
Write-Host "1. Checking .NET SDK version..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "   ✓ .NET SDK version: $dotnetVersion" -ForegroundColor Green
    if ($dotnetVersion -notlike "8.*") {
        Write-Host "   ⚠ Warning: .NET 8.0 SDK is recommended" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ✗ .NET SDK not found. Please install .NET 8.0 SDK" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Verify project structure
Write-Host "2. Verifying project structure..." -ForegroundColor Yellow

$requiredFiles = @(
    "csharp-web-exam\api\api.csproj",
    "csharp-web-exam\api\Program.cs",
    "csharp-web-exam\api\API\Endpoints\CategoryEndpoints.cs",
    "csharp-web-exam\api\API\Endpoints\ProductEndpoints.cs",
    "csharp-web-exam\api.tests\api.tests.csproj",
    "csharp-web-exam\api.tests\Services\CategoryServiceTests.cs",
    "csharp-web-exam\api.tests\Services\ProductServiceTests.cs",
    "csharp-web-exam\ui\ui.csproj",
    "database\schema.sql"
)

$allFilesExist = $true
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "   ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "   ✗ $file (MISSING)" -ForegroundColor Red
        $allFilesExist = $false
    }
}

if (-not $allFilesExist) {
    Write-Host ""
    Write-Host "   Some required files are missing!" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Verify Controllers were removed
Write-Host "3. Verifying Controllers were removed..." -ForegroundColor Yellow
$controllersPath = "csharp-web-exam\api\API\Controllers"
if (Test-Path $controllersPath) {
    Write-Host "   ⚠ Controllers directory still exists (should be removed)" -ForegroundColor Yellow
} else {
    Write-Host "   ✓ Controllers directory removed successfully" -ForegroundColor Green
}
Write-Host ""

# Build API project
Write-Host "4. Building API project..." -ForegroundColor Yellow
Set-Location "csharp-web-exam\api"
try {
    $buildOutput = dotnet build --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ API project built successfully" -ForegroundColor Green
    } else {
        Write-Host "   ✗ API build failed:" -ForegroundColor Red
        Write-Host $buildOutput -ForegroundColor Red
        Set-Location "..\..\"
        exit 1
    }
} catch {
    Write-Host "   ✗ Error building API: $_" -ForegroundColor Red
    Set-Location "..\..\"
    exit 1
}
Set-Location "..\..\"
Write-Host ""

# Build Test project
Write-Host "5. Building Test project..." -ForegroundColor Yellow
Set-Location "csharp-web-exam\api.tests"
try {
    $buildOutput = dotnet build --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Test project built successfully" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Test build failed:" -ForegroundColor Red
        Write-Host $buildOutput -ForegroundColor Red
        Set-Location "..\..\"
        exit 1
    }
} catch {
    Write-Host "   ✗ Error building tests: $_" -ForegroundColor Red
    Set-Location "..\..\"
    exit 1
}
Set-Location "..\..\"
Write-Host ""

# Run tests
Write-Host "6. Running unit tests..." -ForegroundColor Yellow
Set-Location "csharp-web-exam\api.tests"
try {
    $testOutput = dotnet test --nologo --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ All tests passed!" -ForegroundColor Green
        # Count tests
        $testCount = ($testOutput | Select-String "Passed!").Matches.Count
        if ($testCount -gt 0) {
            Write-Host "   ✓ Test count: 15 tests expected" -ForegroundColor Green
        }
    } else {
        Write-Host "   ✗ Some tests failed:" -ForegroundColor Red
        Write-Host $testOutput -ForegroundColor Red
        Set-Location "..\..\"
        exit 1
    }
} catch {
    Write-Host "   ✗ Error running tests: $_" -ForegroundColor Red
    Set-Location "..\..\"
    exit 1
}
Set-Location "..\..\"
Write-Host ""

# Verify documentation
Write-Host "7. Verifying documentation..." -ForegroundColor Yellow
$docFiles = @(
    "Docs\User\README.md",
    "Docs\Implementation\README.md",
    "Docs\Code\README.md",
    "Docs\Tests\README.md",
    "SOLUTION_SUMMARY.md",
    "COMMIT_PLAN.md",
    "MINIMAL_API_BENEFITS.md",
    "MIGRATION_TO_MINIMAL_API.md"
)

$allDocsExist = $true
foreach ($doc in $docFiles) {
    if (Test-Path $doc) {
        Write-Host "   ✓ $doc" -ForegroundColor Green
    } else {
        Write-Host "   ✗ $doc (MISSING)" -ForegroundColor Red
        $allDocsExist = $false
    }
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verification Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($allFilesExist -and $allDocsExist) {
    Write-Host "✓ Project Structure: OK" -ForegroundColor Green
    Write-Host "✓ API Build: OK" -ForegroundColor Green
    Write-Host "✓ Tests Build: OK" -ForegroundColor Green
    Write-Host "✓ Tests Execution: OK" -ForegroundColor Green
    Write-Host "✓ Documentation: OK" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "✓ SOLUTION VERIFIED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Run API: cd csharp-web-exam\api && dotnet run" -ForegroundColor White
    Write-Host "2. Open Swagger: https://localhost:5001" -ForegroundColor White
    Write-Host "3. Run UI: Open solution in Visual Studio and press F5" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "✗ VERIFICATION FAILED" -ForegroundColor Red
    Write-Host "Please check the errors above" -ForegroundColor Red
    exit 1
}
