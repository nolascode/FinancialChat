# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY FinancialChat.sln .
COPY src/FinancialChat.Core/FinancialChat.Core.csproj src/FinancialChat.Core/
COPY src/FinancialChat.Infrastructure/FinancialChat.Infrastructure.csproj src/FinancialChat.Infrastructure/
COPY src/FinancialChat.API/FinancialChat.API.csproj src/FinancialChat.API/
COPY src/FinancialChat.Bot/FinancialChat.Bot.csproj src/FinancialChat.Bot/
COPY src/FinancialChat.Tests/FinancialChat.Tests.csproj src/FinancialChat.Tests/

# Restore dependencies
RUN dotnet restore src/FinancialChat.Bot/FinancialChat.Bot.csproj

# Copy everything else
COPY src/FinancialChat.Core/ src/FinancialChat.Core/
COPY src/FinancialChat.Infrastructure/ src/FinancialChat.Infrastructure/
COPY src/FinancialChat.Bot/ src/FinancialChat.Bot/

# Build
WORKDIR /src/src/FinancialChat.Bot
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinancialChat.Bot.dll"]
