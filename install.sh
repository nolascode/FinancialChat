#!/bin/bash

#############################################################################
# FinancialChat Installer for Linux/Mac
#
# Automated installer that sets up the complete FinancialChat application
# including all dependencies, database, and services.
#
#############################################################################

set -e

# Configuration
APP_NAME="FinancialChat"
APP_VERSION="1.0.0"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/docker-compose.yml"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Flags
SKIP_BUILD=false
SKIP_TESTS=false
DEVELOPMENT=false
UNINSTALL=false
DOTNET_AVAILABLE=false

# Output functions
print_header() {
    echo ""
    echo -e "${CYAN}=============================================${NC}"
    echo -e "${CYAN}  $1${NC}"
    echo -e "${CYAN}=============================================${NC}"
    echo ""
}

print_step() {
    echo -e "${YELLOW}[*] $1${NC}"
}

print_success() {
    echo -e "${GREEN}[+] $1${NC}"
}

print_error() {
    echo -e "${RED}[!] $1${NC}"
}

print_info() {
    echo -e "${GRAY}[i] $1${NC}"
}

show_help() {
    print_header "$APP_NAME Installer v$APP_VERSION"
    echo "Usage: ./install.sh [options]"
    echo ""
    echo "Options:"
    echo "  --skip-build      Skip building Docker images (use existing)"
    echo "  --skip-tests      Skip running unit tests"
    echo "  --development     Install in development mode"
    echo "  --uninstall       Remove all containers, images, and volumes"
    echo "  --help            Show this help message"
    echo ""
    echo "Examples:"
    echo "  ./install.sh                    # Full installation"
    echo "  ./install.sh --skip-tests       # Install without running tests"
    echo "  ./install.sh --uninstall        # Remove everything"
    echo ""
}

check_prerequisites() {
    print_header "Checking Prerequisites"

    # Check Docker
    print_step "Checking Docker installation..."
    if command -v docker &> /dev/null; then
        DOCKER_VERSION=$(docker --version)
        print_success "Docker found: $DOCKER_VERSION"
    else
        print_error "Docker is not installed or not in PATH"
        print_info "Please install Docker from: https://docs.docker.com/get-docker/"
        exit 1
    fi

    # Check Docker is running
    print_step "Checking if Docker is running..."
    if docker info &> /dev/null; then
        print_success "Docker is running"
    else
        print_error "Docker is not running"
        print_info "Please start Docker and try again"
        exit 1
    fi

    # Check Docker Compose
    print_step "Checking Docker Compose..."
    if docker compose version &> /dev/null; then
        COMPOSE_VERSION=$(docker compose version)
        print_success "Docker Compose found: $COMPOSE_VERSION"
    elif command -v docker-compose &> /dev/null; then
        COMPOSE_VERSION=$(docker-compose --version)
        print_success "Docker Compose found: $COMPOSE_VERSION"
        # Use docker-compose instead of docker compose
        alias docker_compose="docker-compose"
    else
        print_error "Docker Compose is not available"
        print_info "Please install Docker Compose"
        exit 1
    fi

    # Check .NET SDK (optional, for development/tests)
    print_step "Checking .NET SDK..."
    if command -v dotnet &> /dev/null; then
        DOTNET_VERSION=$(dotnet --version)
        print_success ".NET SDK found: $DOTNET_VERSION"
        DOTNET_AVAILABLE=true
    else
        print_info ".NET SDK not found - skipping local build/tests"
        print_info "Install from: https://dotnet.microsoft.com/download"
        DOTNET_AVAILABLE=false
    fi

    # Check compose file exists
    print_step "Checking docker-compose.yml..."
    if [ -f "$COMPOSE_FILE" ]; then
        print_success "docker-compose.yml found"
    else
        print_error "docker-compose.yml not found at: $COMPOSE_FILE"
        exit 1
    fi

    print_success "All prerequisites satisfied!"
}

stop_existing_containers() {
    print_header "Stopping Existing Containers"

    print_step "Stopping any running containers..."
    cd "$SCRIPT_DIR"
    docker compose down 2>/dev/null || true
    print_success "Containers stopped"
}

run_tests() {
    if [ "$SKIP_TESTS" = true ]; then
        print_info "Skipping tests (--skip-tests flag)"
        return
    fi

    if [ "$DOTNET_AVAILABLE" = false ]; then
        print_info "Skipping tests (.NET SDK not available)"
        return
    fi

    print_header "Running Unit Tests"

    print_step "Restoring packages..."
    cd "$SCRIPT_DIR"
    dotnet restore --verbosity quiet

    print_step "Running tests..."
    if dotnet test src/FinancialChat.Tests --verbosity minimal --no-restore; then
        print_success "All tests passed!"
    else
        print_error "Some tests failed"
        print_info "You can skip tests with: ./install.sh --skip-tests"
        read -p "Continue anyway? (y/N) " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            exit 1
        fi
    fi
}

build_application() {
    print_header "Building Application"

    cd "$SCRIPT_DIR"

    if [ "$SKIP_BUILD" = true ]; then
        print_info "Skipping build (--skip-build flag)"
        print_step "Pulling existing images..."
        docker compose pull 2>/dev/null || true
        return
    fi

    print_step "Building Docker images..."
    print_info "This may take several minutes on first run..."

    if docker compose build --progress=plain; then
        print_success "Build completed successfully!"
    else
        print_error "Build failed"
        exit 1
    fi
}

wait_for_healthy() {
    local container=$1
    local max_attempts=$2
    local attempts=0

    while [ $attempts -lt $max_attempts ]; do
        health=$(docker inspect --format='{{.State.Health.Status}}' "$container" 2>/dev/null || echo "unknown")
        if [ "$health" = "healthy" ]; then
            return 0
        fi
        sleep 2
        attempts=$((attempts + 1))
        printf "."
    done
    echo ""
    return 1
}

start_services() {
    print_header "Starting Services"

    cd "$SCRIPT_DIR"

    print_step "Starting PostgreSQL..."
    docker compose up -d postgres

    print_step "Waiting for PostgreSQL to be healthy..."
    if wait_for_healthy "financialchat-postgres" 30; then
        echo ""
        print_success "PostgreSQL is ready"
    else
        print_error "PostgreSQL failed to start"
        exit 1
    fi

    print_step "Starting RabbitMQ..."
    docker compose up -d rabbitmq

    print_step "Waiting for RabbitMQ to be healthy..."
    if wait_for_healthy "financialchat-rabbitmq" 30; then
        echo ""
        print_success "RabbitMQ is ready"
    else
        print_error "RabbitMQ failed to start"
        exit 1
    fi

    print_step "Starting API server..."
    docker compose up -d api
    sleep 5
    print_success "API server started"

    print_step "Starting Stock Bot..."
    docker compose up -d bot
    sleep 3
    print_success "Stock Bot started"

    print_success "All services started!"
}

show_status() {
    print_header "Installation Complete!"

    echo ""
    echo -e "  ${NC}Services Status:${NC}"
    echo ""
    docker compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"

    echo ""
    echo -e "  ${NC}Access Points:${NC}"
    echo ""
    echo -e "  ${GREEN}Application:      http://localhost:5000${NC}"
    echo -e "  ${GREEN}RabbitMQ Admin:   http://localhost:15672 (guest/guest)${NC}"
    echo ""

    echo -e "  ${NC}Quick Start:${NC}"
    echo ""
    echo "  1. Open http://localhost:5000 in your browser"
    echo "  2. Register a new account"
    echo "  3. Create or join a chat room"
    echo "  4. Try the stock command: /stock=AAPL.US"
    echo ""

    echo -e "  ${NC}Useful Commands:${NC}"
    echo ""
    echo "  View logs:        docker compose logs -f"
    echo "  Stop services:    docker compose stop"
    echo "  Start services:   docker compose start"
    echo "  Restart:          docker compose restart"
    echo "  Uninstall:        ./install.sh --uninstall"
    echo ""
}

uninstall_application() {
    print_header "Uninstalling $APP_NAME"

    cd "$SCRIPT_DIR"

    print_step "Stopping containers..."
    docker compose down

    print_step "Removing volumes..."
    docker compose down -v

    print_step "Removing images..."
    docker compose down --rmi local 2>/dev/null || true

    # Remove specific images
    docker rmi financialchat-api 2>/dev/null || true
    docker rmi financialchat-bot 2>/dev/null || true

    print_success "Uninstallation complete!"
    print_info "Note: PostgreSQL and RabbitMQ base images were preserved"
    print_info "To remove all images: docker system prune -a"
}

install_application() {
    print_header "$APP_NAME Installer v$APP_VERSION"

    print_info "Installation directory: $SCRIPT_DIR"
    if [ "$DEVELOPMENT" = true ]; then
        print_info "Mode: Development"
    else
        print_info "Mode: Production"
    fi
    echo ""

    check_prerequisites
    stop_existing_containers
    run_tests
    build_application
    start_services
    show_status
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --development)
            DEVELOPMENT=true
            shift
            ;;
        --uninstall)
            UNINSTALL=true
            shift
            ;;
        --help|-h)
            show_help
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Main execution
if [ "$UNINSTALL" = true ]; then
    uninstall_application
    exit 0
fi

install_application
