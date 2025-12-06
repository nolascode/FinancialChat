# FinancialChat - Documentación Técnica Completa

## Tabla de Contenidos

1. [Descripción del Proyecto](#descripción-del-proyecto)
2. [Stack Tecnológico](#stack-tecnológico)
3. [Arquitectura del Sistema](#arquitectura-del-sistema)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Capa Core (Dominio)](#capa-core-dominio)
6. [Capa de Infraestructura](#capa-de-infraestructura)
7. [Capa API](#capa-api)
8. [Bot Worker Service](#bot-worker-service)
9. [Comunicación en Tiempo Real (SignalR)](#comunicación-en-tiempo-real-signalr)
10. [Sistema de Mensajería (RabbitMQ)](#sistema-de-mensajería-rabbitmq)
11. [Autenticación y Autorización](#autenticación-y-autorización)
12. [Base de Datos](#base-de-datos)
13. [Frontend (Razor Pages)](#frontend-razor-pages)
14. [Pruebas Automatizadas](#pruebas-automatizadas)
15. [Configuración Docker](#configuración-docker)
16. [Flujo de Datos](#flujo-de-datos)
17. [Endpoints de la API](#endpoints-de-la-api)
18. [Consideraciones de Seguridad](#consideraciones-de-seguridad)

---

## Descripción del Proyecto

**FinancialChat** es una aplicación de chat en tiempo real construida con .NET 8 que permite a los usuarios comunicarse en salas de chat y obtener cotizaciones de acciones a través de comandos especiales.

### Características Principales

- **Chat en tiempo real** mediante SignalR
- **Múltiples salas de chat** con soporte de paginación
- **Cotizaciones de acciones** mediante comando `/stock=CODIGO`
- **Soporte multi-cotización** (`/stock=AAPL.US,GOOGL.US,MSFT.US`)
- **Bot desacoplado** a través de RabbitMQ
- **Autenticación JWT** con ASP.NET Core Identity
- **Persistencia** en PostgreSQL con Entity Framework Core
- **Soft Delete** para entidades
- **Contenedorización** con Docker Compose

---

## Stack Tecnológico

### Backend

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | Web API + Razor Pages |
| SignalR | 8.0 | Comunicación en tiempo real |
| Entity Framework Core | 8.0.11 | ORM |
| ASP.NET Core Identity | 8.0.11 | Autenticación/Autorización |
| RabbitMQ.Client | 6.8.1 | Message Broker |
| Npgsql | 8.0.11 | Driver PostgreSQL |

### Base de Datos

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| PostgreSQL | 15 Alpine | Base de datos relacional |

### Message Broker

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| RabbitMQ | 3 Management Alpine | Cola de mensajes |

### Frontend

| Tecnología | Propósito |
|------------|-----------|
| Razor Pages | Renderizado del lado del servidor |
| Bootstrap 5.3 | Framework CSS |
| JavaScript (Vanilla) | Lógica del cliente |
| SignalR Client | Comunicación WebSocket |

### Pruebas

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| xUnit | 2.4.2 | Framework de pruebas |
| Moq | 4.20.72 | Mocking |
| FluentAssertions | 6.12.2 | Aserciones legibles |
| EF Core InMemory | 8.0.11 | Pruebas de integración |

### Contenedorización

| Tecnología | Propósito |
|------------|-----------|
| Docker | Contenedores |
| Docker Compose | Orquestación |

---

## Descripción de Tecnologías

### .NET 8
**.NET 8** es el framework multiplataforma más reciente de Microsoft con soporte a largo plazo (LTS) para construir aplicaciones modernas. Proporciona:
- **Alto Rendimiento**: Uno de los frameworks web más rápidos en benchmarks
- **Multiplataforma**: Ejecuta en Windows, Linux y macOS
- **Plataforma Unificada**: Un solo SDK para web, escritorio, móvil, nube e IoT
- **C# 12**: Últimas características del lenguaje incluyendo constructores primarios y expresiones de colección

**Por qué se usa**: Framework principal para construir la API, el hub SignalR y los servicios en segundo plano con excelente rendimiento y productividad del desarrollador.

### ASP.NET Core
**ASP.NET Core** es el framework web construido sobre .NET para crear APIs web y aplicaciones web. Sus características incluyen:
- **Minimal APIs**: Definiciones de endpoints ligeras
- **Razor Pages**: Páginas renderizadas del lado del servidor con C#
- **Pipeline de Middleware**: Procesamiento flexible de peticiones/respuestas
- **Inyección de Dependencias**: Contenedor IoC integrado

**Por qué se usa**: Potencia los endpoints de la API REST, el frontend con Razor Pages y hospeda el hub SignalR.

### SignalR
**SignalR** es una biblioteca de comunicación en tiempo real que permite comunicación bidireccional entre servidor y clientes. Proporciona:
- **Soporte WebSocket**: Comunicación full-duplex sobre una única conexión TCP
- **Fallback Automático**: Recurre a Server-Sent Events o Long Polling si WebSockets no está disponible
- **Protocolo de Hub**: Invocación de métodos fuertemente tipada entre cliente y servidor
- **Grupos**: Transmitir mensajes a subconjuntos de clientes conectados
- **Reconexión Automática**: Manejo de reconexión integrado

**Por qué se usa**: Habilita la funcionalidad de chat en tiempo real donde los mensajes aparecen instantáneamente para todos los usuarios en una sala sin necesidad de recargar la página.

### Entity Framework Core 8
**Entity Framework Core (EF Core)** es el ORM (Object-Relational Mapper) moderno de Microsoft. Proporciona:
- **Enfoque Code-First**: Definir esquema de base de datos usando clases C#
- **Consultas LINQ**: Consultas a base de datos type-safe usando sintaxis C#
- **Migraciones**: Control de versiones para cambios en el esquema
- **Change Tracking**: Detección automática de modificaciones en entidades
- **Interceptores**: Engancharse a operaciones de base de datos (usado para soft delete)
- **Filtros de Consulta Globales**: Aplicar filtros automáticamente a todas las consultas

**Por qué se usa**: Simplifica las operaciones de base de datos, proporciona type safety y habilita características como soft delete mediante interceptores.

### ASP.NET Core Identity
**ASP.NET Core Identity** es un sistema de membresía para autenticación y autorización. Sus características incluyen:
- **Gestión de Usuarios**: Registro, login, hash de contraseñas
- **Autorización Basada en Roles**: Asignar roles a usuarios
- **Identidad Basada en Claims**: Control de acceso granular
- **Validación de Contraseñas**: Requisitos de contraseña configurables
- **Bloqueo de Cuenta**: Protección contra ataques de fuerza bruta

**Por qué se usa**: Proporciona autenticación segura de usuarios con hash de contraseñas, se integra perfectamente con EF Core para almacenamiento de usuarios.

### JWT (JSON Web Tokens)
**JWT** es un estándar abierto para transmitir información de forma segura entre partes como un objeto JSON. Componentes:
- **Header**: Algoritmo y tipo de token
- **Payload**: Claims (ID de usuario, nombre, expiración)
- **Signature**: Verificación de que el token no ha sido alterado

**Por qué se usa**: Autenticación sin estado para la API y SignalR. Los tokens se pasan en headers HTTP o query strings, permitiendo comunicación segura sin almacenamiento de sesión del lado del servidor.

### RabbitMQ
**RabbitMQ** es un message broker de código abierto que implementa el Protocolo Avanzado de Colas de Mensajes (AMQP). Características:
- **Colas de Mensajes**: Almacenan mensajes hasta que los consumidores los procesan
- **Publicar/Suscribir**: Desacopla productores de mensajes de consumidores
- **Confiabilidad**: Persistencia de mensajes, acknowledgments y confirmaciones
- **UI de Administración**: Interfaz web para monitorear colas y conexiones
- **Alta Disponibilidad**: Clustering y mirroring para tolerancia a fallos

**Por qué se usa**: Desacopla la aplicación de chat del bot de acciones. Cuando un usuario solicita una cotización, la API publica un mensaje a RabbitMQ. El bot consume este mensaje, obtiene la cotización y publica la respuesta de vuelta. Esta arquitectura permite:
- Escalado independiente de API y bot
- El bot puede reiniciarse sin afectar el chat
- Los mensajes no se pierden si el bot está temporalmente no disponible

### PostgreSQL
**PostgreSQL** es un potente sistema de base de datos relacional de código abierto. Características:
- **Cumplimiento ACID**: Procesamiento confiable de transacciones
- **Soporte JSON**: Almacenar y consultar datos JSON nativamente
- **Búsqueda de Texto Completo**: Capacidades de búsqueda de texto integradas
- **Extensibilidad**: Funciones personalizadas, tipos de datos e índices
- **Rendimiento**: Optimización avanzada de consultas e indexación

**Por qué se usa**: Base de datos robusta y lista para producción para almacenar usuarios, salas de chat y mensajes. Excelente soporte de EF Core con el proveedor Npgsql.

### Docker
**Docker** es una plataforma para desarrollar, enviar y ejecutar aplicaciones en contenedores. Beneficios:
- **Aislamiento**: Cada servicio ejecuta en su propio contenedor
- **Consistencia**: Mismo entorno en desarrollo y producción
- **Portabilidad**: Ejecuta donde sea que Docker esté instalado
- **Eficiencia de Recursos**: Más ligero que máquinas virtuales

**Por qué se usa**: Empaqueta la aplicación y todas las dependencias en contenedores portables, asegurando comportamiento consistente en diferentes entornos.

### Docker Compose
**Docker Compose** es una herramienta para definir y ejecutar aplicaciones Docker multi-contenedor. Características:
- **Configuración YAML**: Define todos los servicios en un solo archivo
- **Dependencias de Servicios**: Controla el orden de inicio con health checks
- **Networking**: Resolución DNS automática entre contenedores
- **Gestión de Volúmenes**: Almacenamiento de datos persistente

**Por qué se usa**: Orquesta los cuatro servicios (PostgreSQL, RabbitMQ, API, Bot) con el orden de inicio correcto, networking y health checks.

### Stooq API
**Stooq** es un proveedor de datos financieros que ofrece cotizaciones de acciones gratuitas via API CSV. El endpoint:
```
https://stooq.com/q/l/?s={stock_code}&f=sd2t2ohlcv&h&e=csv
```
Retorna CSV con: Symbol, Date, Time, Open, High, Low, Close, Volume

**Por qué se usa**: Fuente gratuita y confiable para cotizaciones de acciones en tiempo real. No requiere API key, formato de respuesta CSV simple.

### xUnit
**xUnit** es un framework de pruebas moderno para .NET. Características:
- **Fact/Theory**: Pruebas simples y parametrizadas
- **Ejecución Paralela**: Las pruebas ejecutan concurrentemente por defecto
- **Extensibilidad**: Descubrimiento y ejecución de pruebas personalizado
- **Métodos Assert**: Rica biblioteca de aserciones

**Por qué se usa**: Framework de pruebas estándar de la industria con excelente soporte de herramientas y sintaxis limpia.

### Moq
**Moq** es una biblioteca de mocking para .NET. Permite:
- **Objetos Mock**: Crear implementaciones falsas de interfaces
- **Verificación de Comportamiento**: Verificar que métodos fueron llamados con parámetros esperados
- **Valores de Retorno**: Configurar respuestas mock
- **Callbacks**: Ejecutar lógica personalizada cuando métodos mockeados son llamados

**Por qué se usa**: Esencial para pruebas unitarias - permite probar componentes en aislamiento mockeando sus dependencias.

### FluentAssertions
**FluentAssertions** proporciona una API fluida para escribir aserciones de prueba. Beneficios:
- **Sintaxis Legible**: `result.Should().Be(expected)`
- **Comparaciones Ricas**: Comparación profunda de objetos, aserciones de colecciones
- **Mensajes de Fallo Claros**: Mensajes de error detallados cuando las pruebas fallan

**Por qué se usa**: Hace las pruebas más legibles y proporciona mejor diagnóstico de fallos que las aserciones estándar.

---

## Arquitectura del Sistema

### Clean Architecture

El proyecto sigue los principios de **Clean Architecture** con clara separación de responsabilidades:

```
┌─────────────────────────────────────────────────────────────┐
│                       PRESENTACIÓN                           │
│                   (FinancialChat.API)                        │
│         Controllers, Hubs, Razor Pages, Extensions           │
├─────────────────────────────────────────────────────────────┤
│                       APLICACIÓN                             │
│                   (FinancialChat.Core)                       │
│         Entidades, DTOs, Interfaces, Settings, Helpers       │
├─────────────────────────────────────────────────────────────┤
│                     INFRAESTRUCTURA                          │
│               (FinancialChat.Infrastructure)                 │
│     DbContext, Repositorios, Servicios, Mensajería, Seed     │
├─────────────────────────────────────────────────────────────┤
│                   SERVICIOS EXTERNOS                         │
│                   (FinancialChat.Bot)                        │
│              Worker Service, Cliente API Stock               │
└─────────────────────────────────────────────────────────────┘
```

### Diagrama de Componentes

```
┌──────────────┐     HTTP/WS      ┌──────────────┐
│    Cliente   │◄────────────────►│   Servidor   │
│   (Browser)  │                  │     API      │
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
│  (Externo)   │
└──────────────┘

┌──────────────┐
│  PostgreSQL  │◄──── Entity Framework Core
│   Database   │
└──────────────┘
```

---

## Estructura del Proyecto

```
FinancialChat/
├── src/
│   ├── FinancialChat.API/              # Capa de presentación
│   │   ├── Controllers/                # Controladores API
│   │   │   ├── AuthController.cs       # Login/Register
│   │   │   └── ChatRoomController.cs   # CRUD ChatRooms
│   │   ├── Hubs/
│   │   │   └── ChatHub.cs              # Hub SignalR
│   │   ├── Services/
│   │   │   └── StockResponseConsumer.cs # Consumidor RabbitMQ
│   │   ├── Extensions/
│   │   │   └── ApplicationServicesExtension.cs # Config DI
│   │   ├── Pages/                      # Razor Pages
│   │   │   ├── Auth/
│   │   │   │   ├── Login.cshtml
│   │   │   │   └── Register.cshtml
│   │   │   ├── Chat/
│   │   │   │   ├── Index.cshtml        # Lista de salas
│   │   │   │   └── Room.cshtml         # Sala de chat
│   │   │   └── Shared/
│   │   │       └── _Layout.cshtml
│   │   ├── wwwroot/                    # Archivos estáticos
│   │   ├── appsettings.json
│   │   ├── appsettings.Docker.json
│   │   ├── Program.cs
│   │   └── Dockerfile
│   │
│   ├── FinancialChat.Core/             # Capa de dominio
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
│   ├── FinancialChat.Infrastructure/   # Capa de infraestructura
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
│   └── FinancialChat.Tests/            # Pruebas
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

## Capa Core (Dominio)

### Entidades

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
- Extiende `IdentityUser<Guid>` para usar GUIDs como IDs
- Implementa `ISoftDeleteEntity` para eliminación lógica

#### ChatRoom
```csharp
public class ChatRoom : BaseEntity
{
    public string Name { get; set; }           // Máximo 100 caracteres
    public string? Description { get; set; }   // Máximo 500 caracteres
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}
```

#### Message
```csharp
public class Message : BaseEntity
{
    public string Content { get; set; }        // Máximo 500 caracteres
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; }       // Desnormalizado para mostrar
    public bool IsBot { get; set; }            // True si es del StockBot
    public Guid? UserId { get; set; }          // Nullable para el bot
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

Helper para parsear comandos de acciones:

```csharp
public static class StockCommandParser
{
    // Regex: ^/stock=(.+)$
    public const int MaxStockCodes = 5;

    // Parsea "/stock=AAPL.US" o "/stock=AAPL.US,GOOGL.US,MSFT.US"
    public static bool TryParse(string message, out IReadOnlyList<string> stockCodes);
    public static IReadOnlyList<string> ParseStockCodes(string codesString);
    public static bool IsStockCommand(string message);
}
```

**Características:**
- Soporta códigos separados por coma
- Elimina duplicados (insensible a mayúsculas)
- Limita a 5 códigos máximo
- Recorte automático de espacios

---

## Capa de Infraestructura

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
        // Aplica configuraciones del assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Previene eliminaciones en cascada
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

### Patrón Repository

#### RepositoryBase<T>
```csharp
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    protected readonly FinancialChatDbContext Context;
    protected readonly DbSet<T> DbSet;

    // Filtra automáticamente entidades eliminadas lógicamente
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
    // Obtiene los últimos N mensajes ordenados cronológicamente
    public async Task<IEnumerable<Message>> GetLatestMessagesByChatRoomAsync(
        Guid chatRoomId, int count = 50, CancellationToken ct = default)
    {
        return await DbSet
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderByDescending(m => m.Timestamp)
            .Take(count)
            .OrderBy(m => m.Timestamp)  // Invierte para orden cronológico
            .ToListAsync(ct);
    }

    // Paginación para "Cargar más"
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
        // Declara colas: stock_requests, stock_responses
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

### StockApiService (Cliente Stooq)

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
        // Formato CSV: Symbol,Date,Time,Open,High,Low,Close,Volume
        // Ejemplo: AAPL.US,2024-01-15,22:00:00,150.12,151.47,149.89,150.25,45678900
        var lines = csvContent.Split('\n');
        var values = lines[1].Split(',');
        var symbol = values[0].Trim().ToUpperInvariant();
        var closePrice = values[6].Trim();

        // Maneja "N/D" (Sin Datos)
        if (closePrice.Equals("N/D", StringComparison.OrdinalIgnoreCase))
            return CreateErrorResponse(stockCode, "Acción no encontrada");

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

## Capa API

### ChatHub (SignalR)

```csharp
[Authorize]
public class ChatHub : Hub<IChatClient>
{
    // Métodos Cliente → Servidor
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
        // Detecta comando /stock=
        if (StockCommandParser.TryParse(content, out var stockCodes))
        {
            await HandleStockCommand(roomId, stockCodes, userName);
            return; // NO guarda el comando en DB
        }

        // Mensaje regular - guarda en DB
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

**Importante:** El comando `/stock=` NO se guarda en la base de datos.

### StockResponseConsumer

```csharp
public class StockResponseConsumer : BackgroundService
{
    // Consume mensajes de la cola stock_responses
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeRabbitMq();
        StartConsuming();
    }

    private async Task ProcessStockResponse(StockQuoteDto quote)
    {
        // 1. Guarda mensaje del bot en DB
        var message = new Message
        {
            Content = quote.Message,
            UserName = "StockBot",
            IsBot = true,
            ChatRoomId = quote.ChatRoomId
        };
        await repositoryManager.MessageRepository.CreateAsync(message);

        // 2. Transmite via SignalR
        await _hubContext.Clients.Group(quote.ChatRoomId.ToString()).ReceiveMessage(messageDto);
        await _hubContext.Clients.Group(quote.ChatRoomId.ToString()).ReceiveStockQuote(quote);
    }
}
```

### Controladores

#### AuthController
```
POST /api/auth/register    - Registro de usuario
POST /api/auth/login       - Login y generación de JWT
```

#### ChatRoomController
```
GET    /api/chatroom                     - Lista todas las salas
GET    /api/chatroom/{id}                - Obtiene sala por ID
GET    /api/chatroom/{id}/messages       - Mensajes paginados (page, pageSize)
POST   /api/chatroom                     - Crea nueva sala
DELETE /api/chatroom/{id}                - Elimina sala (soft delete)
```

### ApplicationServicesExtension

Configuración centralizada de servicios:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
{
    // Base de datos
    services.AddDbContext<FinancialChatDbContext>(options =>
        options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

    // Identity
    services.AddIdentity<User, IdentityRole<Guid>>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<FinancialChatDbContext>();

    // Autenticación JWT
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            // SignalR: extrae token del query string
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

    // Repositorios y Servicios
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
        await Task.Delay(5000, stoppingToken); // Espera a RabbitMQ
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
            // Manejo de errores - responde con mensaje de error
            var errorResponse = new StockQuoteDto
            {
                Message = $"Error procesando solicitud de acción {request.StockCode}: {ex.Message}",
                Success = false,
                Error = ex.Message
            };
            await PublishResponse(errorResponse);
        }
    }
}
```

**Flujo del Bot:**
1. Consume de `stock_requests`
2. Llama a la API de Stooq
3. Parsea respuesta CSV
4. Publica resultado a `stock_responses`

---

## Comunicación en Tiempo Real (SignalR)

### Configuración del Hub

```javascript
// Cliente SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/chat?access_token=' + token)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Eventos Servidor → Cliente
connection.on('ReceiveMessage', (message) => { /* ... */ });
connection.on('ReceiveStockQuote', (quote) => { /* ... */ });
connection.on('UserJoined', (user) => { /* ... */ });
connection.on('UserLeft', (user) => { /* ... */ });
connection.on('Error', (error) => { /* ... */ });

// Métodos Cliente → Servidor
await connection.invoke('JoinRoom', roomId);
await connection.invoke('LeaveRoom', roomId);
await connection.invoke('SendMessage', roomId, message);
```

### Autenticación SignalR

El token JWT se pasa via query string para WebSocket:
```
/hubs/chat?access_token=eyJhbGciOiJIUzI1NiIs...
```

El servidor extrae el token en `JwtBearerEvents.OnMessageReceived`.

---

## Sistema de Mensajería (RabbitMQ)

### Colas

| Cola | Dirección | Contenido |
|------|-----------|-----------|
| `stock_requests` | API → Bot | StockRequestDto (JSON) |
| `stock_responses` | Bot → API | StockQuoteDto (JSON) |

### Configuración de Colas

```csharp
_channel.QueueDeclare(
    queue: "stock_requests",
    durable: true,          // Persiste en reinicio
    exclusive: false,       // Múltiples consumidores
    autoDelete: false,      // No se elimina al desconectar
    arguments: null);

_channel.BasicQos(0, 1, false);  // Prefetch 1 mensaje
```

### Formato de Mensajes

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

## Autenticación y Autorización

### Configuración JWT

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

### Configuración de Identity

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

## Base de Datos

### Esquema

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

### Configuraciones EF Core

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
        builder.HasQueryFilter(m => !m.IsDeleted);  // Filtro global
    }
}
```

---

## Frontend (Razor Pages)

### Páginas

| Ruta | Archivo | Descripción |
|------|---------|-------------|
| `/auth/login` | Login.cshtml | Formulario de login |
| `/auth/register` | Register.cshtml | Formulario de registro |
| `/chat` | Chat/Index.cshtml | Lista de salas |
| `/chat/room?id=` | Chat/Room.cshtml | Sala de chat |

### Room.cshtml - Características

1. **Conexión SignalR** con reconexión automática
2. **Paginación** con "Cargar más" para mensajes antiguos
3. **Distinción visual** para mensajes propios, de otros usuarios y del bot
4. **Indicador de conexión** (Conectando, Conectado, Reconectando, Desconectado)
5. **Contador de mensajes** mostrando total de mensajes
6. **Escape HTML** para prevenir XSS

```javascript
// Paginación
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

## Pruebas Automatizadas

### Estructura de Pruebas

```
FinancialChat.Tests/
├── Unit/
│   ├── Controllers/
│   │   ├── AuthControllerTests.cs      (14 pruebas)
│   │   └── ChatRoomControllerTests.cs  (21 pruebas)
│   ├── Hubs/
│   │   └── ChatHubTests.cs             (13 pruebas)
│   ├── Services/
│   │   ├── TokenServiceTests.cs
│   │   └── StockApiServiceTests.cs
│   ├── Repositories/
│   │   └── MessageRepositoryTests.cs   (10 pruebas)
│   ├── Helpers/
│   │   └── StockCommandParserTests.cs
│   ├── Messaging/
│   │   └── MessageBrokerServiceTests.cs (15 pruebas)
│   └── Bot/
│       └── StockQuoteProcessorTests.cs (13 pruebas)
└── Integration/
    └── Repositories/
        └── ChatRoomRepositoryTests.cs  (12 pruebas)
```

### Total: 138 pruebas

### Ejemplo de Prueba

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
        Times.Never);  // NO debe guardar comandos /stock
}
```

### Ejecutar Pruebas

```bash
# Todas las pruebas
dotnet test src/FinancialChat.Tests

# Pruebas específicas
dotnet test src/FinancialChat.Tests --filter "FullyQualifiedName~ChatHubTests"

# Con cobertura
dotnet test src/FinancialChat.Tests --collect:"XPlat Code Coverage"
```

---

## Configuración Docker

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
      - "15672:15672"  # UI de Administración
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

### Comandos

```bash
# Iniciar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f api
docker-compose logs -f bot

# Detener
docker-compose down

# Limpiar volúmenes
docker-compose down -v
```

---

## Flujo de Datos

### Flujo de Mensaje Regular

```
1. Usuario escribe mensaje
2. Cliente envía via SignalR: connection.invoke('SendMessage', roomId, content)
3. ChatHub.SendMessage() detecta que NO es comando /stock
4. Crea entidad Message y guarda en PostgreSQL
5. Transmite al grupo via SignalR: Clients.Group(roomId).ReceiveMessage(dto)
6. Todos los clientes en la sala reciben el mensaje
```

### Flujo de Cotización de Acciones

```
1. Usuario escribe "/stock=AAPL.US"
2. Cliente envía via SignalR: connection.invoke('SendMessage', roomId, "/stock=AAPL.US")
3. ChatHub.SendMessage() detecta comando /stock via StockCommandParser
4. NO guarda comando en DB
5. Publica StockRequestDto a cola "stock_requests" via RabbitMQ
6. StockQuoteWorker (Bot) consume el mensaje
7. Bot llama a API de Stooq: GET https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv
8. Bot parsea respuesta CSV
9. Bot publica StockQuoteDto a cola "stock_responses"
10. StockResponseConsumer (API) consume el mensaje
11. Guarda mensaje del bot en PostgreSQL
12. Transmite al grupo via SignalR: Clients.Group(roomId).ReceiveMessage(dto)
13. Usuarios ven: "🤖 StockBot: AAPL.US quote is $150.25 per share"
```

---

## Endpoints de la API

### Autenticación

| Método | Endpoint | Body | Respuesta |
|--------|----------|------|-----------|
| POST | `/api/auth/register` | `{ userName, email, password }` | `{ success, token, userId, ... }` |
| POST | `/api/auth/login` | `{ email, password }` | `{ success, token, userId, ... }` |

### Salas de Chat

| Método | Endpoint | Query Params | Respuesta |
|--------|----------|--------------|-----------|
| GET | `/api/chatrooms` | - | Lista de ChatRoomDto |
| GET | `/api/chatrooms/{id}` | - | ChatRoomDto |
| GET | `/api/chatrooms/{id}/messages` | `page`, `pageSize` | PagedResult<MessageDto> |
| POST | `/api/chatrooms` | `{ name, description }` | ChatRoomDto |
| DELETE | `/api/chatrooms/{id}` | - | Success/Error |

### Hub SignalR

| Método | Parámetros | Descripción |
|--------|------------|-------------|
| `JoinRoom` | `roomId: Guid` | Une usuario a sala |
| `LeaveRoom` | `roomId: Guid` | Remueve usuario de sala |
| `SendMessage` | `roomId: Guid, content: string` | Envía mensaje o comando |

---

## Consideraciones de Seguridad

### Implementadas

1. **Autenticación JWT** - Tokens firmados con HMAC-SHA256
2. **Autorización** - Hub y endpoints protegidos con `[Authorize]`
3. **Escape HTML** - Prevención de XSS en mensajes
4. **Validación de entrada** - Longitud máxima de mensajes (500 chars)
5. **Límite de códigos** - Máximo 5 códigos de acciones por comando
6. **Soft Delete** - Eliminación lógica de datos
7. **Query Filters** - Filtro global para entidades eliminadas
8. **CORS** - Configuración restrictiva de orígenes

### Configuración de Contraseñas

```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequiredLength = 6;
options.User.RequireUniqueEmail = true;
```

### Límites de Recursos

| Recurso | Límite |
|---------|--------|
| Mensajes por página | 100 máximo |
| Códigos de acciones | 5 máximo |
| Longitud de mensaje | 500 caracteres |
| Nombre de sala | 100 caracteres |
| Descripción de sala | 500 caracteres |

---

## Instalador Automatizado

El proyecto incluye instaladores automatizados para Windows y Linux/Mac.

### Instalación en Windows

```powershell
# Instalación completa
.\install.ps1

# Omitir ejecución de pruebas
.\install.ps1 -SkipTests

# Omitir construcción (usar imágenes existentes)
.\install.ps1 -SkipBuild

# Desinstalar todo
.\install.ps1 -Uninstall

# Mostrar ayuda
.\install.ps1 -Help
```

### Instalación en Linux/Mac

```bash
# Hacer ejecutable (solo la primera vez)
chmod +x install.sh

# Instalación completa
./install.sh

# Omitir ejecución de pruebas
./install.sh --skip-tests

# Omitir construcción (usar imágenes existentes)
./install.sh --skip-build

# Desinstalar todo
./install.sh --uninstall

# Mostrar ayuda
./install.sh --help
```

### Qué Hace el Instalador

1. **Verificación de Requisitos**: Verifica Docker, Docker Compose y opcionalmente .NET SDK
2. **Detener Existentes**: Detiene cualquier contenedor de instalaciones previas
3. **Ejecutar Pruebas**: Ejecuta todas las pruebas unitarias (puede omitirse)
4. **Construir Imágenes**: Construye imágenes Docker para API y Bot
5. **Iniciar Servicios**: Inicia PostgreSQL, RabbitMQ, API y Bot en orden correcto
6. **Health Checks**: Espera a que cada servicio esté saludable antes de continuar
7. **Mostrar Estado**: Muestra servicios en ejecución y URLs de acceso

### Post-Instalación

Después de una instalación exitosa:
- **Aplicación**: http://localhost:5000
- **Admin RabbitMQ**: http://localhost:15672 (guest/guest)

---

## Apéndice: Comandos Útiles

### Desarrollo Local

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar API
dotnet run --project src/FinancialChat.API

# Ejecutar Bot
dotnet run --project src/FinancialChat.Bot

# Ejecutar pruebas
dotnet test

# Migraciones EF
dotnet ef migrations add NombreMigracion --project src/FinancialChat.Infrastructure --startup-project src/FinancialChat.API
dotnet ef database update --project src/FinancialChat.Infrastructure --startup-project src/FinancialChat.API
```

### Docker

```bash
# Construir e iniciar
docker-compose up --build -d

# Logs en tiempo real
docker-compose logs -f

# Acceder a PostgreSQL
docker exec -it financialchat-postgres psql -U financialchat -d FinancialChatDb

# Administración RabbitMQ
http://localhost:15672 (guest/guest)
```

---

*Documentación generada para FinancialChat - Aplicación de Chat en Tiempo Real con .NET 8*
