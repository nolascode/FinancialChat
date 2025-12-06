<#
.SYNOPSIS
    FinancialChat Installer for Windows
.DESCRIPTION
    Automated installer that sets up the complete FinancialChat application
    including all dependencies, database, and services.
#>

param(
    [switch]$SkipBuild,
    [switch]$SkipTests,
    [switch]$Development,
    [switch]$Uninstall,
    [switch]$Help
)

$ErrorActionPreference = "Stop"

# Configuration
$AppName = "FinancialChat"
$AppVersion = "1.0.0"
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$ComposeFile = Join-Path $ScriptPath "docker-compose.yml"

# Colors for output
function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Write-Header {
    param([string]$Title)
    Write-Host ""
    Write-ColorOutput "=============================================" "Cyan"
    Write-ColorOutput "  $Title" "Cyan"
    Write-ColorOutput "=============================================" "Cyan"
    Write-Host ""
}

function Write-Step {
    param([string]$Message)
    Write-ColorOutput "[*] $Message" "Yellow"
}

function Write-Success {
    param([string]$Message)
    Write-ColorOutput "[+] $Message" "Green"
}

function Write-Error {
    param([string]$Message)
    Write-ColorOutput "[!] $Message" "Red"
}

function Write-Info {
    param([string]$Message)
    Write-ColorOutput "[i] $Message" "Gray"
}

function Show-Help {
    Write-Header "$AppName Installer v$AppVersion"
    Write-Host "Usage: .\install.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -SkipBuild      Skip building Docker images (use existing)"
    Write-Host "  -SkipTests      Skip running unit tests"
    Write-Host "  -Development    Install in development mode (with hot reload)"
    Write-Host "  -Uninstall      Remove all containers, images, and volumes"
    Write-Host "  -Help           Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\install.ps1                    # Full installation"
    Write-Host "  .\install.ps1 -SkipTests         # Install without running tests"
    Write-Host "  .\install.ps1 -Uninstall         # Remove everything"
    Write-Host ""
}

function Test-Prerequisites {
    Write-Header "Checking Prerequisites"

    # Check Docker
    Write-Step "Checking Docker installation..."
    try {
        $dockerVersion = docker --version
        Write-Success "Docker found: $dockerVersion"
    }
    catch {
        Write-Error "Docker is not installed or not in PATH"
        Write-Info "Please install Docker Desktop from: https://www.docker.com/products/docker-desktop"
        exit 1
    }

    # Check Docker is running
    Write-Step "Checking if Docker is running..."
    try {
        docker info | Out-Null
        Write-Success "Docker is running"
    }
    catch {
        Write-Error "Docker is not running"
        Write-Info "Please start Docker Desktop and try again"
        exit 1
    }

    # Check Docker Compose
    Write-Step "Checking Docker Compose..."
    try {
        $composeVersion = docker compose version
        Write-Success "Docker Compose found: $composeVersion"
    }
    catch {
        Write-Error "Docker Compose is not available"
        Write-Info "Please ensure Docker Desktop is up to date"
        exit 1
    }

    # Check .NET SDK (optional, for development/tests)
    Write-Step "Checking .NET SDK..."
    try {
        $dotnetVersion = dotnet --version
        Write-Success ".NET SDK found: $dotnetVersion"
        $script:DotNetAvailable = $true
    }
    catch {
        Write-Info ".NET SDK not found - skipping local build/tests"
        Write-Info "Install from: https://dotnet.microsoft.com/download"
        $script:DotNetAvailable = $false
    }

    # Check compose file exists
    Write-Step "Checking docker-compose.yml..."
    if (Test-Path $ComposeFile) {
        Write-Success "docker-compose.yml found"
    }
    else {
        Write-Error "docker-compose.yml not found at: $ComposeFile"
        exit 1
    }

    Write-Success "All prerequisites satisfied!"
}

function Stop-ExistingContainers {
    Write-Header "Stopping Existing Containers"

    Write-Step "Stopping any running containers..."
    Set-Location $ScriptPath
    docker compose down 2>$null
    Write-Success "Containers stopped"
}

function Run-Tests {
    if ($SkipTests) {
        Write-Info "Skipping tests (--SkipTests flag)"
        return
    }

    if (-not $script:DotNetAvailable) {
        Write-Info "Skipping tests (.NET SDK not available)"
        return
    }

    Write-Header "Running Unit Tests"

    Write-Step "Restoring packages..."
    Set-Location $ScriptPath
    dotnet restore --verbosity quiet

    Write-Step "Running tests..."
    $testResult = dotnet test src/FinancialChat.Tests --verbosity minimal --no-restore

    if ($LASTEXITCODE -eq 0) {
        Write-Success "All tests passed!"
    }
    else {
        Write-Error "Some tests failed"
        Write-Info "You can skip tests with: .\install.ps1 -SkipTests"
        $continue = Read-Host "Continue anyway? (y/N)"
        if ($continue -ne "y" -and $continue -ne "Y") {
            exit 1
        }
    }
}

function Build-Application {
    Write-Header "Building Application"

    Set-Location $ScriptPath

    if ($SkipBuild) {
        Write-Info "Skipping build (--SkipBuild flag)"
        Write-Step "Pulling existing images..."
        docker compose pull 2>$null
        return
    }

    Write-Step "Building Docker images..."
    Write-Info "This may take several minutes on first run..."

    docker compose build --progress=plain

    if ($LASTEXITCODE -eq 0) {
        Write-Success "Build completed successfully!"
    }
    else {
        Write-Error "Build failed"
        exit 1
    }
}

function Start-Services {
    Write-Header "Starting Services"

    Set-Location $ScriptPath

    Write-Step "Starting PostgreSQL..."
    docker compose up -d postgres

    Write-Step "Waiting for PostgreSQL to be healthy..."
    $attempts = 0
    $maxAttempts = 30
    while ($attempts -lt $maxAttempts) {
        $health = docker inspect --format='{{.State.Health.Status}}' financialchat-postgres 2>$null
        if ($health -eq "healthy") {
            Write-Success "PostgreSQL is ready"
            break
        }
        Start-Sleep -Seconds 2
        $attempts++
        Write-Host "." -NoNewline
    }
    if ($attempts -eq $maxAttempts) {
        Write-Error "PostgreSQL failed to start"
        exit 1
    }
    Write-Host ""

    Write-Step "Starting RabbitMQ..."
    docker compose up -d rabbitmq

    Write-Step "Waiting for RabbitMQ to be healthy..."
    $attempts = 0
    while ($attempts -lt $maxAttempts) {
        $health = docker inspect --format='{{.State.Health.Status}}' financialchat-rabbitmq 2>$null
        if ($health -eq "healthy") {
            Write-Success "RabbitMQ is ready"
            break
        }
        Start-Sleep -Seconds 2
        $attempts++
        Write-Host "." -NoNewline
    }
    if ($attempts -eq $maxAttempts) {
        Write-Error "RabbitMQ failed to start"
        exit 1
    }
    Write-Host ""

    Write-Step "Starting API server..."
    docker compose up -d api
    Start-Sleep -Seconds 5
    Write-Success "API server started"

    Write-Step "Starting Stock Bot..."
    docker compose up -d bot
    Start-Sleep -Seconds 3
    Write-Success "Stock Bot started"

    Write-Success "All services started!"
}

function Show-Status {
    Write-Header "Installation Complete!"

    Write-Host ""
    Write-ColorOutput "  Services Status:" "White"
    Write-Host ""
    docker compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"

    Write-Host ""
    Write-ColorOutput "  Access Points:" "White"
    Write-Host ""
    Write-ColorOutput "  Application:      http://localhost:5000" "Green"
    Write-ColorOutput "  RabbitMQ Admin:   http://localhost:15672 (guest/guest)" "Green"
    Write-Host ""

    Write-ColorOutput "  Quick Start:" "White"
    Write-Host ""
    Write-Host "  1. Open http://localhost:5000 in your browser"
    Write-Host "  2. Register a new account"
    Write-Host "  3. Create or join a chat room"
    Write-Host "  4. Try the stock command: /stock=AAPL.US"
    Write-Host ""

    Write-ColorOutput "  Useful Commands:" "White"
    Write-Host ""
    Write-Host "  View logs:        docker compose logs -f"
    Write-Host "  Stop services:    docker compose stop"
    Write-Host "  Start services:   docker compose start"
    Write-Host "  Restart:          docker compose restart"
    Write-Host "  Uninstall:        .\install.ps1 -Uninstall"
    Write-Host ""
}

function Uninstall-Application {
    Write-Header "Uninstalling $AppName"

    Set-Location $ScriptPath

    Write-Step "Stopping containers..."
    docker compose down

    Write-Step "Removing volumes..."
    docker compose down -v

    Write-Step "Removing images..."
    docker compose down --rmi local 2>$null

    # Remove specific images
    $images = @(
        "financialchat-api",
        "financialchat-bot"
    )
    foreach ($image in $images) {
        docker rmi $image 2>$null
    }

    Write-Success "Uninstallation complete!"
    Write-Info "Note: PostgreSQL and RabbitMQ base images were preserved"
    Write-Info "To remove all images: docker system prune -a"
}

function Install-Application {
    Write-Header "$AppName Installer v$AppVersion"

    Write-Info "Installation directory: $ScriptPath"
    Write-Info "Mode: $(if ($Development) { 'Development' } else { 'Production' })"
    Write-Host ""

    Test-Prerequisites
    Stop-ExistingContainers
    Run-Tests
    Build-Application
    Start-Services
    Show-Status
}

# Main execution
if ($Help) {
    Show-Help
    exit 0
}

if ($Uninstall) {
    Uninstall-Application
    exit 0
}

Install-Application
