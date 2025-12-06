# FinancialChat - Complete Technical Documentation

## Table of Contents

1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [System Architecture](#system-architecture)
4. [Project Structure](#project-structure)
5. [Core Layer (Domain)](#core-layer-domain)
6. [Infrastructure Layer](#infrastructure-layer)
7. [API Layer](#api-layer)
8. [Bot Worker Service](#bot-worker-service)
9. [Real-Time Communication (SignalR)](#real-time-communication-signalr)
10. [Messaging System (RabbitMQ)](#messaging-system-rabbitmq)
11. [Authentication and Authorization](#authentication-and-authorization)
12. [Database](#database)
13. [Frontend (Razor Pages)](#frontend-razor-pages)
14. [Automated Testing](#automated-testing)
15. [Docker Configuration](#docker-configuration)
16. [Data Flow](#data-flow)
17. [API Endpoints](#api-endpoints)
18. [Security Considerations](#security-considerations)

---

## Project Overview

**FinancialChat** is a real-time chat application built with .NET 8 that allows users to communicate in chat rooms and obtain stock quotes through special commands.

### Key Features

- **Real-time chat** via SignalR
- **Multiple chat rooms** with pagination support
- **Stock quotes** via `/stock=CODE` command
- **Multi-quote support** (`/stock=AAPL.US,GOOGL.US,MSFT.US`)
- **Decoupled bot** through RabbitMQ
- **JWT Authentication** with ASP.NET Core Identity
- **Persistence** in PostgreSQL with Entity Framework Core
- **Soft Delete** for entities
- **Containerization** with Docker Compose

---

## Technology Stack

### Backend

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Main framework |
| ASP.NET Core | 8.0 | Web API + Razor Pages |
| SignalR | 8.0 | Real-time communication |
| Entity Framework Core | 8.0.11 | ORM |
| ASP.NET Core Identity | 8.0.11 | Authentication/Authorization |
| RabbitMQ.Client | 6.8.1 | Message Broker |
| Npgsql | 8.0.11 | PostgreSQL Driver |

### Database

| Technology | Version | Purpose |
|------------|---------|---------|
| PostgreSQL | 15 Alpine | Relational database |

### Message Broker

| Technology | Version | Purpose |
|------------|---------|---------|
| RabbitMQ | 3 Management Alpine | Message queue |

### Frontend

| Technology | Purpose |
|------------|---------|
| Razor Pages | Server-side rendering |
| Bootstrap 5.3 | CSS Framework |
| JavaScript (Vanilla) | Client-side logic |
| SignalR Client | WebSocket communication |

### Testing

| Technology | Version | Purpose |
|------------|---------|---------|
| xUnit | 2.4.2 | Testing framework |
| Moq | 4.20.72 | Mocking |
| FluentAssertions | 6.12.2 | Readable assertions |
| EF Core InMemory | 8.0.11 | Integration tests |

### Containerization

| Technology | Purpose |
|------------|---------|
| Docker | Containers |
| Docker Compose | Orchestration |

---

## Technology Descriptions

### .NET 8
**.NET 8** is Microsoft's latest Long-Term Support (LTS) cross-platform framework for building modern applications. It provides:
- **High Performance**: One of the fastest web frameworks in benchmarks
- **Cross-Platform**: Runs on Windows, Linux, and macOS
- **Unified Platform**: Single SDK for web, desktop, mobile, cloud, and IoT
- **C# 12**: Latest language features including primary constructors and collection expressions

**Why used**: Primary framework for building the API, SignalR hub, and background services with excellent performance and developer productivity.

### ASP.NET Core
**ASP.NET Core** is the web framework built on .NET for creating web APIs and web applications. Features include:
- **Minimal APIs**: Lightweight endpoint definitions
- **Razor Pages**: Server-side rendered pages with C#
- **Middleware Pipeline**: Flexible request/response processing
- **Dependency Injection**: Built-in IoC container

**Why used**: Powers the REST API endpoints, Razor Pages frontend, and hosts the SignalR hub.

### SignalR
**SignalR** is a real-time communication library that enables bi-directional communication between server and clients. It provides:
- **WebSocket Support**: Full-duplex communication over a single TCP connection
- **Automatic Fallback**: Falls back to Server-Sent Events or Long Polling if WebSockets unavailable
- **Hub Protocol**: Strongly-typed method invocation between client and server
- **Groups**: Broadcast messages to subsets of connected clients
- **Automatic Reconnection**: Built-in reconnection handling

**Why used**: Enables real-time chat functionality where messages appear instantly for all users in a chat room without page refresh.

### Entity Framework Core 8
**Entity Framework Core (EF Core)** is Microsoft's modern Object-Relational Mapper (ORM). It provides:
- **Code-First Approach**: Define database schema using C# classes
- **LINQ Queries**: Type-safe database queries using C# syntax
- **Migrations**: Version control for database schema changes
- **Change Tracking**: Automatic detection of entity modifications
- **Interceptors**: Hook into database operations (used for soft delete)
- **Global Query Filters**: Automatically apply filters to all queries

**Why used**: Simplifies database operations, provides type safety, and enables features like soft delete through interceptors.

### ASP.NET Core Identity
**ASP.NET Core Identity** is a membership system for authentication and authorization. Features include:
- **User Management**: Registration, login, password hashing
- **Role-Based Authorization**: Assign roles to users
- **Claims-Based Identity**: Fine-grained access control
- **Password Validation**: Configurable password requirements
- **Account Lockout**: Protection against brute-force attacks

**Why used**: Provides secure user authentication with password hashing, integrates seamlessly with EF Core for user storage.

### JWT (JSON Web Tokens)
**JWT** is an open standard for securely transmitting information between parties as a JSON object. Components:
- **Header**: Algorithm and token type
- **Payload**: Claims (user ID, name, expiration)
- **Signature**: Verification that the token hasn't been tampered with

**Why used**: Stateless authentication for the API and SignalR. Tokens are passed in HTTP headers or query strings, enabling secure communication without server-side session storage.

### RabbitMQ
**RabbitMQ** is an open-source message broker that implements the Advanced Message Queuing Protocol (AMQP). Features:
- **Message Queues**: Store messages until consumers process them
- **Publish/Subscribe**: Decouple message producers from consumers
- **Reliability**: Message persistence, acknowledgments, and confirmations
- **Management UI**: Web interface for monitoring queues and connections
- **High Availability**: Clustering and mirroring for fault tolerance

**Why used**: Decouples the chat application from the stock bot. When a user requests a stock quote, the API publishes a message to RabbitMQ. The bot consumes this message, fetches the quote, and publishes the response back. This architecture allows:
- Independent scaling of API and bot
- Bot can be restarted without affecting chat
- Messages are not lost if bot is temporarily unavailable

### PostgreSQL
**PostgreSQL** is a powerful, open-source relational database system. Features:
- **ACID Compliance**: Reliable transaction processing
- **JSON Support**: Store and query JSON data natively
- **Full-Text Search**: Built-in text search capabilities
- **Extensibility**: Custom functions, data types, and indexes
- **Performance**: Advanced query optimization and indexing

**Why used**: Robust, production-ready database for storing users, chat rooms, and messages. Excellent EF Core support with Npgsql provider.

### Docker
**Docker** is a platform for developing, shipping, and running applications in containers. Benefits:
- **Isolation**: Each service runs in its own container
- **Consistency**: Same environment in development and production
- **Portability**: Run anywhere Docker is installed
- **Resource Efficiency**: Lighter than virtual machines

**Why used**: Packages the application and all dependencies into portable containers, ensuring consistent behavior across different environments.

### Docker Compose
**Docker Compose** is a tool for defining and running multi-container Docker applications. Features:
- **YAML Configuration**: Define all services in a single file
- **Service Dependencies**: Control startup order with health checks
- **Networking**: Automatic DNS resolution between containers
- **Volume Management**: Persistent data storage

**Why used**: Orchestrates the four services (PostgreSQL, RabbitMQ, API, Bot) with proper startup order, networking, and health checks.

### Stooq API
**Stooq** is a financial data provider offering free stock quotes via CSV API. The endpoint:
```
https://stooq.com/q/l/?s={stock_code}&f=sd2t2ohlcv&h&e=csv
```
Returns CSV with: Symbol, Date, Time, Open, High, Low, Close, Volume

**Why used**: Free, reliable source for real-time stock quotes. No API key required, simple CSV response format.

### xUnit
**xUnit** is a modern testing framework for .NET. Features:
- **Fact/Theory**: Simple and parameterized tests
- **Parallel Execution**: Tests run concurrently by default
- **Extensibility**: Custom test discovery and execution
- **Assert Methods**: Rich assertion library

**Why used**: Industry-standard testing framework with excellent tooling support and clean syntax.

### Moq
**Moq** is a mocking library for .NET. It allows:
- **Mock Objects**: Create fake implementations of interfaces
- **Behavior Verification**: Verify methods were called with expected parameters
- **Return Values**: Configure mock responses
- **Callbacks**: Execute custom logic when mocked methods are called

**Why used**: Essential for unit testing - allows testing components in isolation by mocking their dependencies.

### FluentAssertions
**FluentAssertions** provides a fluent API for writing test assertions. Benefits:
- **Readable Syntax**: `result.Should().Be(expected)`
- **Rich Comparisons**: Deep object comparison, collection assertions
- **Clear Failure Messages**: Detailed error messages when tests fail

**Why used**: Makes tests more readable and provides better failure diagnostics than standard assertions.

---

## System Architecture

### Clean Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                        PRESENTATION                          │
│                   (FinancialChat.API)                        │
│         Controllers, Hubs, Razor Pages, Extensions           │
├─────────────────────────────────────────────────────────────┤
│                       APPLICATION                            │
│                   (FinancialChat.Core)                       │
│         Entities, DTOs, Interfaces, Settings, Helpers        │
├─────────────────────────────────────────────────────────────┤
│                      INFRASTRUCTURE                          │
│               (FinancialChat.Infrastructure)                 │
│     DbContext, Repositories, Services, Messaging, Seed       │
├─────────────────────────────────────────────────────────────┤
│                     EXTERNAL SERVICES                        │
│                   (FinancialChat.Bot)                        │
│              Worker Service, Stock API Client                │
└─────────────────────────────────────────────────────────────┘
```

### Component Diagram

```
┌──────────────┐     HTTP/WS      ┌──────────────┐
│    Client    │◄────────────────►│   API        │
│   (Browser)  │                  │   Server     │
└──────────────┘                  └──────┬───────┘
                                         │
                                         │ SignalR
                                         │
┌──────────────┐    RabbitMQ      ┌──────▼───────┐
│   StockBot   │◄────────────────►│   Message    │
│   Worker     │                  │   Broker     │
└──────┬───────┘                  └──────────────┘
       │
       │ HTTP
       ▼
┌──────────────┐
│  Stooq API   │
│  (External)  │
└──────────────┘

┌──────────────┐
│  PostgreSQL  │◄──── Entity Framework Core
│   Database   │
└──────────────┘
```

---

## Project Structure

```
FinancialChat/
├── src/
│   ├── FinancialChat.API/              # Presentation layer
│   │   ├── Controllers/                # API Controllers
│   │   │   ├── AuthController.cs       # Login/Register
│   │   │   └── ChatRoomController.cs   # CRUD ChatRooms
│   │   ├── Hubs/
│   │   │   └── ChatHub.cs              # SignalR Hub
│   │   ├── Services/
│   │   │   └── StockResponseConsumer.cs # RabbitMQ Consumer
│   │   ├── Extensions/
│   │   │   └── ApplicationServicesExtension.cs # DI Config
│   │   ├── Pages/                      # Razor Pages
│   │   │   ├── Auth/
│   │   │   │   ├── Login.cshtml
│   │   │   │   └── Register.cshtml
│   │   │   ├── Chat/
│   │   │   │   ├── Index.cshtml        # Room list
│   │   │   │   └── Room.cshtml         # Chat room
│   │   │   └── Shared/
│   │   │       └── _Layout.cshtml
│   │   ├── wwwroot/                    # Static files
│   │   ├── appsettings.json
│   │   ├── appsettings.Docker.json
│   │   ├── Program.cs
│   │   └── Dockerfile
│   │
│   ├── FinancialChat.Core/             # Domain layer
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── ISoftDeleteEntity.cs
│   │   │   ├── User.cs
│   │   │   ├── ChatRoom.cs
│   │   │   └── Message.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginDto.cs
│   │   │   │   ├── RegisterDto.cs
│   │   │   │   └── AuthResponseDto.cs
│   │   │   ├── Chat/
│   │   │   │   ├── MessageDto.cs
│   │   │   │   ├── ChatRoomDto.cs
│   │   │   │   ├── StockQuoteDto.cs
│   │   │   │   └── SendMessageDto.cs
│   │   │   └── Common/
│   │   │       ├── ApiResponse.cs
│   │   │       └── PagedResult.cs
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IRepositoryBase.cs
│   │   │   │   ├── IRepositoryManager.cs
│   │   │   │   ├── IChatRoomRepository.cs
│   │   │   │   └── IMessageRepository.cs
│   │   │   ├── Services/
│   │   │   │   ├── ITokenService.cs
│   │   │   │   ├── IStockService.cs
│   │   │   │   └── IMessageBrokerService.cs
│   │   │   └── Hubs/
│   │   │       └── IChatClient.cs
│   │   ├── Helpers/
│   │   │   └── StockCommandParser.cs
│   │   └── Settings/
│   │       ├── JwtSettings.cs
│   │       ├── RabbitMqSettings.cs
│   │       └── StooqApiSettings.cs
│   │
│   ├── FinancialChat.Infrastructure/   # Infrastructure layer
│   │   ├── Persistence/
│   │   │   ├── Context/
│   │   │   │   └── FinancialChatDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── ChatRoomConfiguration.cs
│   │   │   │   └── MessageConfiguration.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── RepositoryBase.cs
│   │   │   │   ├── RepositoryManager.cs
│   │   │   │   ├── ChatRoomRepository.cs
│   │   │   │   └── MessageRepository.cs
│   │   │   ├── Interceptors/
│   │   │   │   └── SoftDeleteInterceptor.cs
│   │   │   └── Seed/
│   │   │       └── DatabaseSeeder.cs
│   │   ├── Services/
│   │   │   ├── TokenService.cs
│   │   │   └── StockApiService.cs
│   │   ├── Messaging/
│   │   │   └── RabbitMqService.cs
│   │   └── Migrations/
│   │
│   ├── FinancialChat.Bot/              # Worker Service
│   │   ├── Workers/
│   │   │   └── StockQuoteWorker.cs
│   │   ├── appsettings.json
│   │   ├── Program.cs
│   │   └── Dockerfile
│   │
│   └── FinancialChat.Tests/            # Tests
│       ├── Unit/
│       │   ├── Controllers/
│       │   │   ├── AuthControllerTests.cs
│       │   │   └── ChatRoomControllerTests.cs
│       │   ├── Hubs/
│       │   │   └── ChatHubTests.cs
│       │   ├── Services/
│       │   │   ├── TokenServiceTests.cs
│       │   │   └── StockApiServiceTests.cs
│       │   ├── Repositories/
│       │   │   └── MessageRepositoryTests.cs
│       │   ├── Helpers/
│       │   │   └── StockCommandParserTests.cs
│       │   ├── Messaging/
│       │   │   └── MessageBrokerServiceTests.cs
│       │   └── Bot/
│       │       └── StockQuoteProcessorTests.cs
│       └── Integration/
│           └── Repositories/
│               └── ChatRoomRepositoryTests.cs
│
├── docker-compose.yml
├── dev.sh
└── FinancialChat.sln
```

---

## Core Layer (Domain)

### Entities

#### BaseEntity
```csharp
public abstract class BaseEntity : ISoftDeleteEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
```

#### User
```csharp
public class User : IdentityUser<Guid>, ISoftDeleteEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}
```
- Extends `IdentityUser<Guid>` to use GUIDs as IDs
- Implements `ISoftDeleteEntity` for logical deletion

#### ChatRoom
```csharp
public class ChatRoom : BaseEntity
{
    public string Name { get; set; }           // Max 100 characters
    public string? Description { get; set; }   // Max 500 characters
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}
```

#### Message
```csharp
public class Message : BaseEntity
{
    public string Content { get; set; }        // Max 500 characters
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; }       // Denormalized for display
    public bool IsBot { get; set; }            // True if from StockBot
    public Guid? UserId { get; set; }          // Nullable for bot
    public Guid ChatRoomId { get; set; }
    public virtual User? User { get; set; }
    public virtual ChatRoom ChatRoom { get; set; }
}
```

### DTOs

#### StockQuoteDto
```csharp
public record StockQuoteDto
{
    public string StockCode { get; init; }
    public decimal? Price { get; init; }
    public string Message { get; init; }       // "AAPL.US quote is $150.25 per share"
    public Guid ChatRoomId { get; init; }
    public Guid CorrelationId { get; init; }
    public bool Success { get; init; }
    public string? Error { get; init; }
}
```

#### StockRequestDto
```csharp
public record StockRequestDto
{
    public string StockCode { get; init; }
    public Guid ChatRoomId { get; init; }
    public string RequestedBy { get; init; }
    public Guid CorrelationId { get; init; }
}
```

#### PagedResult<T>
```csharp
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

### Interfaces

#### IChatClient (SignalR)
```csharp
public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveStockQuote(StockQuoteDto quote);
    Task UserJoined(string userName);
    Task UserLeft(string userName);
    Task Error(string message);
}
```

#### IMessageBrokerService
```csharp
public interface IMessageBrokerService
{
    Task PublishStockRequestAsync(StockRequestDto request, CancellationToken cancellationToken = default);
    Task PublishStockResponseAsync(StockQuoteDto response, CancellationToken cancellationToken = default);
}
```

### StockCommandParser

Helper for parsing stock commands:

```csharp
public static class StockCommandParser
{
    // Regex: ^/stock=(.+)$
    public const int MaxStockCodes = 5;

    // Parses "/stock=AAPL.US" or "/stock=AAPL.US,GOOGL.US,MSFT.US"
    public static bool TryParse(string message, out IReadOnlyList<string> stockCodes);
    public static IReadOnlyList<string> ParseStockCodes(string codesString);
    public static bool IsStockCommand(string message);
}
```

**Features:**
- Supports comma-separated codes
- Removes duplicates (case-insensitive)
- Limits to 5 codes maximum
- Automatic whitespace trimming

---

## Infrastructure Layer

### DbContext

```csharp
public class FinancialChatDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_softDeleteInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Prevent cascade deletes
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }
}
```

### SoftDeleteInterceptor

```csharp
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        foreach (var entry in context.ChangeTracker.Entries<ISoftDeleteEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = DateTime.UtcNow;
            }
        }
        return base.SavingChangesAsync(...);
    }
}
```

### Repository Pattern

#### RepositoryBase<T>
```csharp
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    protected readonly FinancialChatDbContext Context;
    protected readonly DbSet<T> DbSet;

    // Automatically filters soft-deleted entities
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await DbSet.Where(e => !e.IsDeleted).ToListAsync(ct);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken ct = default);
    public virtual Task DeleteAsync(T entity, CancellationToken ct = default);
}
```

#### MessageRepository
```csharp
public class MessageRepository : RepositoryBase<Message>, IMessageRepository
{
    // Gets latest N messages ordered chronologically
    public async Task<IEnumerable<Message>> GetLatestMessagesByChatRoomAsync(
        Guid chatRoomId, int count = 50, CancellationToken ct = default)
    {
        return await DbSet
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderByDescending(m => m.Timestamp)
            .Take(count)
            .OrderBy(m => m.Timestamp)  // Reverse for chronological order
            .ToListAsync(ct);
    }

    // Pagination for "Load More"
    public async Task<(IEnumerable<Message>, int)> GetMessagesByChatRoomPagedAsync(
        Guid chatRoomId, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        var query = DbSet.Where(m => m.ChatRoomId == chatRoomId);
        var totalCount = await query.CountAsync(ct);
        var messages = await query
            .OrderByDescending(m => m.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(ct);
        return (messages, totalCount);
    }
}
```

### RabbitMqService

```csharp
public class RabbitMqService : IMessageBrokerService, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqService(IOptions<RabbitMqSettings> settings, ILogger logger)
    {
        InitializeConnection();
        // Declares queues: stock_requests, stock_responses
    }

    public Task PublishStockRequestAsync(StockRequestDto request, CancellationToken ct = default)
    {
        return PublishMessageAsync(_settings.StockRequestsQueue, request);
    }

    private Task PublishMessageAsync<T>(string queueName, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
    }
}
```

### StockApiService (Stooq Client)

```csharp
public class StockApiService : IStockService
{
    public async Task<StockQuoteDto> GetStockQuoteAsync(string stockCode, CancellationToken ct = default)
    {
        // URL: https://stooq.com/q/l/?s={code}&f=sd2t2ohlcv&h&e=csv
        var response = await _httpClient.GetStringAsync(url, ct);
        return ParseCsvResponse(response, stockCode);
    }

    private StockQuoteDto ParseCsvResponse(string csvContent, string stockCode)
    {
        // CSV Format: Symbol,Date,Time,Open,High,Low,Close,Volume
        // Example: AAPL.US,2024-01-15,22:00:00,150.12,151.47,149.89,150.25,45678900
        var lines = csvContent.Split('\n');
        var values = lines[1].Split(',');
        var symbol = values[0].Trim().ToUpperInvariant();
        var closePrice = values[6].Trim();

        // Handle "N/D" (No Data)
        if (closePrice.Equals("N/D", StringComparison.OrdinalIgnoreCase))
            return CreateErrorResponse(stockCode, "Stock not found");

        decimal.TryParse(closePrice, out var price);
        return new StockQuoteDto
        {
            StockCode = symbol,
            Price = price,
            Success = true,
            Message = $"{symbol} quote is ${price:F2} per share"
        };
    }
}
```

---

## API Layer

### ChatHub (SignalR)

```csharp
[Authorize]
public class ChatHub : Hub<IChatClient>
{
    // Client → Server methods
    public async Task JoinRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).UserJoined(userName);
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).UserLeft(userName);
    }

    public async Task SendMessage(Guid roomId, string content)
    {
        // Detect /stock= command
        if (StockCommandParser.TryParse(content, out var stockCodes))
        {
            await HandleStockCommand(roomId, stockCodes, userName);
            return; // Does NOT save command to DB
        }

        // Regular message - save to DB
        var message = new Message { /* ... */ };
        await _repositoryManager.MessageRepository.CreateAsync(message);
        await Clients.Group(roomId.ToString()).ReceiveMessage(messageDto);
    }

    private async Task HandleStockCommand(Guid roomId, IReadOnlyList<string> stockCodes, string requestedBy)
    {
        foreach (var stockCode in stockCodes)
        {
            var request = new StockRequestDto { StockCode = stockCode, ChatRoomId = roomId, ... };
            await _messageBroker.PublishStockRequestAsync(request);
        }
    }
}
```

**Important:** The `/stock=` command is NOT saved to the database.

### StockResponseConsumer

```csharp
public class StockResponseConsumer : BackgroundService
{
    // Consumes messages from stock_responses queue
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeRabbitMq();
        StartConsuming();
    }

    private async Task ProcessStockResponse(StockQuoteDto quote)
    {
        // 1. Save bot message to DB
        var message = new Message
        {
            Content = quote.Message,
            UserName = "StockBot",
            IsBot = true,
            ChatRoomId = quote.ChatRoomId
        };
        await repositoryManager.MessageRepository.CreateAsync(message);

        // 2. Broadcast via SignalR
        await _hubContext.Clients.Group(quote.ChatRoomId.ToString()).ReceiveMessage(messageDto);
        await _hubContext.Clients.Group(quote.ChatRoomId.ToString()).ReceiveStockQuote(quote);
    }
}
```

### Controllers

#### AuthController
```
POST /api/auth/register    - User registration
POST /api/auth/login       - Login and JWT generation
```

#### ChatRoomController
```
GET    /api/chatroom                     - List all rooms
GET    /api/chatroom/{id}                - Get room by ID
GET    /api/chatroom/{id}/messages       - Paginated messages (page, pageSize)
POST   /api/chatroom                     - Create new room
DELETE /api/chatroom/{id}                - Delete room (soft delete)
```

### ApplicationServicesExtension

Centralized service configuration:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
{
    // Database
    services.AddDbContext<FinancialChatDbContext>(options =>
        options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

    // Identity
    services.AddIdentity<User, IdentityRole<Guid>>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<FinancialChatDbContext>();

    // JWT Authentication
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            // SignalR: extract token from query string
            options.Events = new JwtBearerEvents {
                OnMessageReceived = context => {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        context.Token = accessToken;
                    return Task.CompletedTask;
                }
            };
        });

    // Settings
    services.Configure<RabbitMqSettings>(config.GetSection("RabbitMq"));
    services.Configure<StooqApiSettings>(config.GetSection("StooqApi"));

    // Repositories & Services
    services.AddScoped<IRepositoryManager, RepositoryManager>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddSingleton<IMessageBrokerService, RabbitMqService>();
    services.AddHttpClient<IStockService, StockApiService>();

    // SignalR
    services.AddSignalR(options => {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    });

    // CORS
    services.AddCors(options => {
        options.AddPolicy("AllowFrontend", builder =>
            builder.WithOrigins("http://localhost:5000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials());
    });
}
```

---

## Bot Worker Service

### StockQuoteWorker

```csharp
public class StockQuoteWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken); // Wait for RabbitMQ
        InitializeRabbitMq();
        StartConsuming(stoppingToken);
    }

    private async Task ProcessStockRequest(StockRequestDto request)
    {
        try
        {
            var quote = await _stockService.GetStockQuoteAsync(request.StockCode);
            var response = new StockQuoteDto
            {
                StockCode = quote.StockCode,
                Price = quote.Price,
                Message = quote.Message,
                ChatRoomId = request.ChatRoomId,
                CorrelationId = request.CorrelationId,
                Success = quote.Success,
                Error = quote.Error
            };
            await PublishResponse(response);
        }
        catch (Exception ex)
        {
            // Error handling - respond with error message
            var errorResponse = new StockQuoteDto
            {
                Message = $"Error processing stock request for {request.StockCode}: {ex.Message}",
                Success = false,
                Error = ex.Message
            };
            await PublishResponse(errorResponse);
        }
    }
}
```

**Bot Flow:**
1. Consumes from `stock_requests`
2. Calls Stooq API
3. Parses CSV response
4. Publishes result to `stock_responses`

---

## Real-Time Communication (SignalR)

### Hub Configuration

```javascript
// SignalR Client
const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/chat?access_token=' + token)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Server → Client events
connection.on('ReceiveMessage', (message) => { /* ... */ });
connection.on('ReceiveStockQuote', (quote) => { /* ... */ });
connection.on('UserJoined', (user) => { /* ... */ });
connection.on('UserLeft', (user) => { /* ... */ });
connection.on('Error', (error) => { /* ... */ });

// Client → Server methods
await connection.invoke('JoinRoom', roomId);
await connection.invoke('LeaveRoom', roomId);
await connection.invoke('SendMessage', roomId, message);
```

### SignalR Authentication

The JWT token is passed via query string for WebSocket:
```
/hubs/chat?access_token=eyJhbGciOiJIUzI1NiIs...
```

The server extracts the token in `JwtBearerEvents.OnMessageReceived`.

---

## Messaging System (RabbitMQ)

### Queues

| Queue | Direction | Content |
|-------|-----------|---------|
| `stock_requests` | API → Bot | StockRequestDto (JSON) |
| `stock_responses` | Bot → API | StockQuoteDto (JSON) |

### Queue Configuration

```csharp
_channel.QueueDeclare(
    queue: "stock_requests",
    durable: true,          // Persists on restart
    exclusive: false,       // Multiple consumers
    autoDelete: false,      // Doesn't delete on disconnect
    arguments: null);

_channel.BasicQos(0, 1, false);  // Prefetch 1 message
```

### Message Format

**StockRequestDto:**
```json
{
  "stockCode": "aapl.us",
  "chatRoomId": "550e8400-e29b-41d4-a716-446655440000",
  "requestedBy": "john_doe",
  "correlationId": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
}
```

**StockQuoteDto:**
```json
{
  "stockCode": "AAPL.US",
  "price": 150.25,
  "message": "AAPL.US quote is $150.25 per share",
  "chatRoomId": "550e8400-e29b-41d4-a716-446655440000",
  "correlationId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "success": true,
  "error": null
}
```

---

## Authentication and Authorization

### JWT Configuration

```json
{
  "Jwt": {
    "Key": "SuperSecretKeyForJwtTokenGeneration123!",
    "Issuer": "FinancialChat",
    "Audience": "FinancialChatUsers",
    "ExpirationMinutes": 60
  }
}
```

### TokenService

```csharp
public class TokenService : ITokenService
{
    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Identity Configuration

```csharp
services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
});
```

---

## Database

### Schema

```
┌─────────────────────────────────────────────────────┐
│                    AspNetUsers                       │
├─────────────────────────────────────────────────────┤
│ Id (GUID, PK)                                       │
│ UserName (VARCHAR 256)                              │
│ Email (VARCHAR 256)                                 │
│ PasswordHash (TEXT)                                 │
│ CreatedAt (TIMESTAMP)                               │
│ IsDeleted (BOOLEAN)                                 │
│ DeletedAtUtc (TIMESTAMP NULL)                       │
└─────────────────────────────────────────────────────┘
            │
            │ 1:N
            ▼
┌─────────────────────────────────────────────────────┐
│                     Messages                         │
├─────────────────────────────────────────────────────┤
│ Id (GUID, PK)                                       │
│ Content (VARCHAR 500)                               │
│ Timestamp (TIMESTAMP)                               │
│ UserName (VARCHAR 100)                              │
│ IsBot (BOOLEAN)                                     │
│ UserId (GUID NULL, FK → AspNetUsers)                │
│ ChatRoomId (GUID, FK → ChatRooms)                   │
│ IsDeleted (BOOLEAN)                                 │
│ DeletedAtUtc (TIMESTAMP NULL)                       │
└─────────────────────────────────────────────────────┘
            │
            │ N:1
            ▼
┌─────────────────────────────────────────────────────┐
│                    ChatRooms                         │
├─────────────────────────────────────────────────────┤
│ Id (GUID, PK)                                       │
│ Name (VARCHAR 100, UNIQUE)                          │
│ Description (VARCHAR 500 NULL)                      │
│ CreatedAt (TIMESTAMP)                               │
│ IsDeleted (BOOLEAN)                                 │
│ DeletedAtUtc (TIMESTAMP NULL)                       │
└─────────────────────────────────────────────────────┘
```

### EF Core Configurations

```csharp
// MessageConfiguration.cs
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.Content).HasMaxLength(500).IsRequired();
        builder.Property(m => m.UserName).HasMaxLength(100).IsRequired();
        builder.HasIndex(m => m.ChatRoomId);
        builder.HasIndex(m => m.Timestamp);
        builder.HasQueryFilter(m => !m.IsDeleted);  // Global filter
    }
}
```

---

## Frontend (Razor Pages)

### Pages

| Route | File | Description |
|-------|------|-------------|
| `/auth/login` | Login.cshtml | Login form |
| `/auth/register` | Register.cshtml | Registration form |
| `/chat` | Chat/Index.cshtml | Room list |
| `/chat/room?id=` | Chat/Room.cshtml | Chat room |

### Room.cshtml - Features

1. **SignalR connection** with automatic reconnection
2. **Pagination** with "Load More" for older messages
3. **Visual distinction** for own messages, other users, and bot
4. **Connection indicator** (Connecting, Connected, Reconnecting, Disconnected)
5. **Message counter** showing total messages
6. **HTML escaping** to prevent XSS

```javascript
// Pagination
let currentPage = 1;
let hasMoreMessages = false;
const pageSize = 50;

async function loadMessages(page, replace = false) {
    const response = await fetch(`/api/chatroom/${roomId}/messages?page=${page}&pageSize=${pageSize}`, {
        headers: { 'Authorization': 'Bearer ' + token }
    });
    const result = await response.json();

    if (result.success) {
        currentPage = result.data.page;
        hasMoreMessages = result.data.hasNextPage;

        if (replace) renderMessages(result.data.items);
        else prependMessages(result.data.items);
    }
}

async function loadOlderMessages() {
    if (!hasMoreMessages) return;
    await loadMessages(currentPage + 1, false);
}
```

---

## Automated Testing

### Test Structure

```
FinancialChat.Tests/
├── Unit/
│   ├── Controllers/
│   │   ├── AuthControllerTests.cs      (14 tests)
│   │   └── ChatRoomControllerTests.cs  (21 tests)
│   ├── Hubs/
│   │   └── ChatHubTests.cs             (13 tests)
│   ├── Services/
│   │   ├── TokenServiceTests.cs
│   │   └── StockApiServiceTests.cs
│   ├── Repositories/
│   │   └── MessageRepositoryTests.cs   (10 tests)
│   ├── Helpers/
│   │   └── StockCommandParserTests.cs
│   ├── Messaging/
│   │   └── MessageBrokerServiceTests.cs (15 tests)
│   └── Bot/
│       └── StockQuoteProcessorTests.cs (13 tests)
└── Integration/
    └── Repositories/
        └── ChatRoomRepositoryTests.cs  (12 tests)
```

### Total: 138 tests

### Test Example

```csharp
[Fact]
public async Task SendMessage_WithStockCommand_DoesNotSaveToDatabase()
{
    // Arrange
    var roomId = Guid.NewGuid();
    var message = "/stock=AAPL.US";

    // Act
    await _chatHub.SendMessage(roomId, message);

    // Assert
    _messageRepositoryMock.Verify(
        x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()),
        Times.Never);  // Must NOT save /stock commands
}
```

### Running Tests

```bash
# All tests
dotnet test src/FinancialChat.Tests

# Specific tests
dotnet test src/FinancialChat.Tests --filter "FullyQualifiedName~ChatHubTests"

# With coverage
dotnet test src/FinancialChat.Tests --collect:"XPlat Code Coverage"
```

---

## Docker Configuration

### docker-compose.yml

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    container_name: financialchat-postgres
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: FinancialChatDb
      POSTGRES_USER: financialchat
      POSTGRES_PASSWORD: financialchat123
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U financialchat -d FinancialChatDb"]

  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: financialchat-rabbitmq
    ports:
      - "5672:5672"    # AMQP
      - "15672:15672"  # Management UI
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "check_running"]

  api:
    build:
      context: .
      dockerfile: src/FinancialChat.API/Dockerfile
    container_name: financialchat-api
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
    depends_on:
      postgres: { condition: service_healthy }
      rabbitmq: { condition: service_healthy }

  bot:
    build:
      context: .
      dockerfile: src/FinancialChat.Bot/Dockerfile
    container_name: financialchat-bot
    environment:
      - DOTNET_ENVIRONMENT=Docker
    depends_on:
      rabbitmq: { condition: service_healthy }

networks:
  financialchat-network:
    driver: bridge

volumes:
  postgres_data:
```

### Commands

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api
docker-compose logs -f bot

# Stop
docker-compose down

# Clean volumes
docker-compose down -v
```

---

## Data Flow

### Regular Message Flow

```
1. User types message
2. Client sends via SignalR: connection.invoke('SendMessage', roomId, content)
3. ChatHub.SendMessage() detects it's NOT a /stock command
4. Creates Message entity and saves to PostgreSQL
5. Broadcasts to group via SignalR: Clients.Group(roomId).ReceiveMessage(dto)
6. All clients in the room receive the message
```

### Stock Quote Flow

```
1. User types "/stock=AAPL.US"
2. Client sends via SignalR: connection.invoke('SendMessage', roomId, "/stock=AAPL.US")
3. ChatHub.SendMessage() detects /stock command via StockCommandParser
4. Does NOT save command to DB
5. Publishes StockRequestDto to "stock_requests" queue via RabbitMQ
6. StockQuoteWorker (Bot) consumes the message
7. Bot calls Stooq API: GET https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv
8. Bot parses CSV response
9. Bot publishes StockQuoteDto to "stock_responses" queue
10. StockResponseConsumer (API) consumes the message
11. Saves bot message to PostgreSQL
12. Broadcasts to group via SignalR: Clients.Group(roomId).ReceiveMessage(dto)
13. Users see: "🤖 StockBot: AAPL.US quote is $150.25 per share"
```

---

## API Endpoints

### Authentication

| Method | Endpoint | Body | Response |
|--------|----------|------|----------|
| POST | `/api/auth/register` | `{ userName, email, password }` | `{ success, token, userId, ... }` |
| POST | `/api/auth/login` | `{ email, password }` | `{ success, token, userId, ... }` |

### Chat Rooms

| Method | Endpoint | Query Params | Response |
|--------|----------|--------------|----------|
| GET | `/api/chatrooms` | - | List of ChatRoomDto |
| GET | `/api/chatrooms/{id}` | - | ChatRoomDto |
| GET | `/api/chatrooms/{id}/messages` | `page`, `pageSize` | PagedResult<MessageDto> |
| POST | `/api/chatrooms` | `{ name, description }` | ChatRoomDto |
| DELETE | `/api/chatrooms/{id}` | - | Success/Error |

### SignalR Hub

| Method | Parameters | Description |
|--------|------------|-------------|
| `JoinRoom` | `roomId: Guid` | Joins user to room |
| `LeaveRoom` | `roomId: Guid` | Removes user from room |
| `SendMessage` | `roomId: Guid, content: string` | Sends message or command |

---

## Security Considerations

### Implemented

1. **JWT Authentication** - Tokens signed with HMAC-SHA256
2. **Authorization** - Hub and endpoints protected with `[Authorize]`
3. **HTML Escaping** - XSS prevention in messages
4. **Input Validation** - Maximum message length (500 chars)
5. **Code Limit** - Maximum 5 stock codes per command
6. **Soft Delete** - Logical data deletion
7. **Query Filters** - Global filter for deleted entities
8. **CORS** - Restrictive origin configuration

### Password Configuration

```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequiredLength = 6;
options.User.RequireUniqueEmail = true;
```

### Resource Limits

| Resource | Limit |
|----------|-------|
| Messages per page | 100 maximum |
| Stock codes | 5 maximum |
| Message length | 500 characters |
| Room name | 100 characters |
| Room description | 500 characters |

---

## Automated Installer

The project includes automated installers for Windows and Linux/Mac.

### Windows Installation

```powershell
# Full installation
.\install.ps1

# Skip running tests
.\install.ps1 -SkipTests

# Skip building (use existing images)
.\install.ps1 -SkipBuild

# Uninstall everything
.\install.ps1 -Uninstall

# Show help
.\install.ps1 -Help
```

### Linux/Mac Installation

```bash
# Make executable (first time only)
chmod +x install.sh

# Full installation
./install.sh

# Skip running tests
./install.sh --skip-tests

# Skip building (use existing images)
./install.sh --skip-build

# Uninstall everything
./install.sh --uninstall

# Show help
./install.sh --help
```

### What the Installer Does

1. **Prerequisites Check**: Verifies Docker, Docker Compose, and optionally .NET SDK
2. **Stop Existing**: Stops any running containers from previous installations
3. **Run Tests**: Executes all unit tests (can be skipped)
4. **Build Images**: Builds Docker images for API and Bot
5. **Start Services**: Starts PostgreSQL, RabbitMQ, API, and Bot in correct order
6. **Health Checks**: Waits for each service to be healthy before proceeding
7. **Display Status**: Shows running services and access URLs

### Post-Installation

After successful installation:
- **Application**: http://localhost:5000
- **RabbitMQ Admin**: http://localhost:15672 (guest/guest)

---

## Appendix: Useful Commands

### Local Development

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run API
dotnet run --project src/FinancialChat.API

# Run Bot
dotnet run --project src/FinancialChat.Bot

# Run tests
dotnet test

# EF Migrations
dotnet ef migrations add MigrationName --project src/FinancialChat.Infrastructure --startup-project src/FinancialChat.API
dotnet ef database update --project src/FinancialChat.Infrastructure --startup-project src/FinancialChat.API
```

### Docker

```bash
# Build and start
docker-compose up --build -d

# Real-time logs
docker-compose logs -f

# Access PostgreSQL
docker exec -it financialchat-postgres psql -U financialchat -d FinancialChatDb

# RabbitMQ Management
http://localhost:15672 (guest/guest)
```

---

*Documentation generated for FinancialChat - .NET 8 Real-time Chat Application*
