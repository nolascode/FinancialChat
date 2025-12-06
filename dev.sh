#!/bin/bash

# FinancialChat Development Helper Script
# Similar to LoanAppPlatform dev.sh

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Print colored message
print_message() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Print header
print_header() {
    echo ""
    print_message "$BLUE" "=================================="
    print_message "$BLUE" "$1"
    print_message "$BLUE" "=================================="
    echo ""
}

# Check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_message "$RED" "Error: Docker is not running. Please start Docker and try again."
        exit 1
    fi
}

# Start all services
start() {
    print_header "Starting FinancialChat Services"
    check_docker
    docker-compose up -d
    print_message "$GREEN" "Services started successfully!"
    print_message "$YELLOW" "API: http://localhost:5000"
    print_message "$YELLOW" "RabbitMQ Management: http://localhost:15672 (guest/guest)"
    print_message "$YELLOW" "PostgreSQL: localhost:5432"
}

# Stop all services
stop() {
    print_header "Stopping FinancialChat Services"
    check_docker
    docker-compose down
    print_message "$GREEN" "Services stopped successfully!"
}

# Restart all services
restart() {
    print_header "Restarting FinancialChat Services"
    stop
    start
}

# Rebuild and restart services
rebuild() {
    print_header "Rebuilding FinancialChat Services"
    check_docker
    docker-compose down
    docker-compose build --no-cache
    docker-compose up -d
    print_message "$GREEN" "Services rebuilt and started successfully!"
}

# View logs for all services
logs() {
    print_header "Viewing All Logs"
    check_docker
    docker-compose logs -f
}

# View API logs
logs_api() {
    print_header "Viewing API Logs"
    check_docker
    docker-compose logs -f api
}

# View Bot logs
logs_bot() {
    print_header "Viewing Bot Logs"
    check_docker
    docker-compose logs -f bot
}

# View PostgreSQL logs
logs_db() {
    print_header "Viewing PostgreSQL Logs"
    check_docker
    docker-compose logs -f postgres
}

# View RabbitMQ logs
logs_rabbit() {
    print_header "Viewing RabbitMQ Logs"
    check_docker
    docker-compose logs -f rabbitmq
}

# Run EF migrations
migrate() {
    print_header "Running EF Core Migrations"
    cd src/FinancialChat.API
    dotnet ef database update
    cd ../..
    print_message "$GREEN" "Migrations applied successfully!"
}

# Create a new migration
migration_add() {
    if [ -z "$1" ]; then
        print_message "$RED" "Error: Please provide a migration name"
        print_message "$YELLOW" "Usage: ./dev.sh migration-add <MigrationName>"
        exit 1
    fi
    print_header "Creating New Migration: $1"
    cd src/FinancialChat.API
    dotnet ef migrations add "$1" --project ../FinancialChat.Infrastructure/FinancialChat.Infrastructure.csproj
    cd ../..
    print_message "$GREEN" "Migration '$1' created successfully!"
}

# Run all tests
test() {
    print_header "Running All Tests"
    dotnet test src/FinancialChat.Tests/FinancialChat.Tests.csproj --verbosity normal
    print_message "$GREEN" "Tests completed!"
}

# Run tests with coverage
test_coverage() {
    print_header "Running Tests with Coverage"
    dotnet test src/FinancialChat.Tests/FinancialChat.Tests.csproj --collect:"XPlat Code Coverage" --verbosity normal
    print_message "$GREEN" "Tests with coverage completed!"
}

# Build the solution
build() {
    print_header "Building Solution"
    dotnet build FinancialChat.sln
    print_message "$GREEN" "Build completed successfully!"
}

# Clean build artifacts
clean() {
    print_header "Cleaning Build Artifacts"
    dotnet clean FinancialChat.sln
    find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true
    find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true
    print_message "$GREEN" "Clean completed!"
}

# Clean Docker resources
docker_clean() {
    print_header "Cleaning Docker Resources"
    check_docker
    docker-compose down -v --rmi local
    print_message "$GREEN" "Docker resources cleaned!"
}

# Full clean (build artifacts + Docker)
full_clean() {
    print_header "Full Clean"
    clean
    docker_clean
    print_message "$GREEN" "Full clean completed!"
}

# Shell into API container
shell_api() {
    print_header "Opening Shell in API Container"
    check_docker
    docker-compose exec api /bin/sh
}

# Shell into PostgreSQL container
shell_db() {
    print_header "Opening PostgreSQL Shell"
    check_docker
    docker-compose exec postgres psql -U financialchat -d FinancialChatDb
}

# Shell into RabbitMQ container
shell_rabbit() {
    print_header "Opening Shell in RabbitMQ Container"
    check_docker
    docker-compose exec rabbitmq /bin/sh
}

# Open RabbitMQ Management UI
rabbit() {
    print_header "Opening RabbitMQ Management UI"
    if command -v xdg-open > /dev/null; then
        xdg-open http://localhost:15672
    elif command -v open > /dev/null; then
        open http://localhost:15672
    else
        print_message "$YELLOW" "Please open http://localhost:15672 in your browser"
    fi
}

# Show service status
status() {
    print_header "Service Status"
    check_docker
    docker-compose ps
}

# Restore NuGet packages
restore() {
    print_header "Restoring NuGet Packages"
    dotnet restore FinancialChat.sln
    print_message "$GREEN" "Packages restored successfully!"
}

# Run API locally (for development)
run_api() {
    print_header "Running API Locally"
    cd src/FinancialChat.API
    dotnet run
}

# Run Bot locally (for development)
run_bot() {
    print_header "Running Bot Locally"
    cd src/FinancialChat.Bot
    dotnet run
}

# Start infrastructure only (DB + RabbitMQ)
infra() {
    print_header "Starting Infrastructure Only"
    check_docker
    docker-compose up -d postgres rabbitmq
    print_message "$GREEN" "Infrastructure started!"
    print_message "$YELLOW" "PostgreSQL: localhost:5432"
    print_message "$YELLOW" "RabbitMQ: localhost:5672"
    print_message "$YELLOW" "RabbitMQ Management: http://localhost:15672"
}

# Wait for services to be healthy
wait_healthy() {
    print_header "Waiting for Services to be Healthy"
    check_docker

    print_message "$YELLOW" "Waiting for PostgreSQL..."
    until docker-compose exec -T postgres pg_isready -U financialchat -d FinancialChatDb > /dev/null 2>&1; do
        sleep 1
    done
    print_message "$GREEN" "PostgreSQL is ready!"

    print_message "$YELLOW" "Waiting for RabbitMQ..."
    until docker-compose exec -T rabbitmq rabbitmq-diagnostics check_running > /dev/null 2>&1; do
        sleep 1
    done
    print_message "$GREEN" "RabbitMQ is ready!"
}

# Show help
help() {
    print_header "FinancialChat Dev Helper"
    echo "Usage: ./dev.sh <command> [options]"
    echo ""
    print_message "$GREEN" "Docker Commands:"
    echo "  start           Start all services"
    echo "  stop            Stop all services"
    echo "  restart         Restart all services"
    echo "  rebuild         Rebuild and restart services"
    echo "  status          Show service status"
    echo "  infra           Start infrastructure only (DB + RabbitMQ)"
    echo ""
    print_message "$GREEN" "Logs:"
    echo "  logs            View all logs"
    echo "  logs-api        View API logs"
    echo "  logs-bot        View Bot logs"
    echo "  logs-db         View PostgreSQL logs"
    echo "  logs-rabbit     View RabbitMQ logs"
    echo ""
    print_message "$GREEN" "Development:"
    echo "  build           Build solution"
    echo "  test            Run all tests"
    echo "  test-coverage   Run tests with coverage"
    echo "  run-api         Run API locally"
    echo "  run-bot         Run Bot locally"
    echo "  restore         Restore NuGet packages"
    echo ""
    print_message "$GREEN" "Database:"
    echo "  migrate         Run EF migrations"
    echo "  migration-add   Create new migration (requires name)"
    echo ""
    print_message "$GREEN" "Shell Access:"
    echo "  shell-api       Open shell in API container"
    echo "  shell-db        Open PostgreSQL shell"
    echo "  shell-rabbit    Open shell in RabbitMQ container"
    echo ""
    print_message "$GREEN" "Utilities:"
    echo "  rabbit          Open RabbitMQ Management UI"
    echo "  wait-healthy    Wait for services to be healthy"
    echo "  clean           Clean build artifacts"
    echo "  docker-clean    Clean Docker resources"
    echo "  full-clean      Clean everything"
    echo "  help            Show this help message"
}

# Parse command
case "$1" in
    start)
        start
        ;;
    stop)
        stop
        ;;
    restart)
        restart
        ;;
    rebuild)
        rebuild
        ;;
    logs)
        logs
        ;;
    logs-api)
        logs_api
        ;;
    logs-bot)
        logs_bot
        ;;
    logs-db)
        logs_db
        ;;
    logs-rabbit)
        logs_rabbit
        ;;
    migrate)
        migrate
        ;;
    migration-add)
        migration_add "$2"
        ;;
    test)
        test
        ;;
    test-coverage)
        test_coverage
        ;;
    build)
        build
        ;;
    clean)
        clean
        ;;
    docker-clean)
        docker_clean
        ;;
    full-clean)
        full_clean
        ;;
    shell-api)
        shell_api
        ;;
    shell-db)
        shell_db
        ;;
    shell-rabbit)
        shell_rabbit
        ;;
    rabbit)
        rabbit
        ;;
    status)
        status
        ;;
    restore)
        restore
        ;;
    run-api)
        run_api
        ;;
    run-bot)
        run_bot
        ;;
    infra)
        infra
        ;;
    wait-healthy)
        wait_healthy
        ;;
    help|--help|-h|"")
        help
        ;;
    *)
        print_message "$RED" "Unknown command: $1"
        echo ""
        help
        exit 1
        ;;
esac
