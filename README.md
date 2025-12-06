# FinancialChat 💹

A powerful real-time chat application built with .NET 8, featuring decoupled stock quote services via RabbitMQ.

## 🚀 Key Features

*   **Real-time Messaging**: Instant communication using SignalR WebSockets.
*   **Stock Quotes**: Get live quotes using `/stock=CODE` (e.g., `/stock=aapl.us`).
*   **Microservices Architecture**: Decoupled Bot service for processing financial data.
*   **Clean Architecture**: Separation of concerns with Domain, Infrastructure, and Presentation layers.
*   **Secure**: JWT Authentication and Identity management.
*   **Docker Ready**: Full containerization with Docker Compose.

## 🛠️ Tech Stack

*   **.NET 8** (API, Worker Service)
*   **PostgreSQL** (Database)
*   **RabbitMQ** (Message Broker)
*   **SignalR** (Real-time Engine)
*   **Razor Pages** (Frontend)

## ⚡ Quick Start

### Prerequisites
*   Docker & Docker Compose

### Installation

**Windows:**
```powershell
.\install.ps1
```

**Linux/Mac:**
```bash
chmod +x install.sh
./install.sh
```

The application will be available at `http://localhost:5000`.

## 📚 Documentation

Full technical documentation is available in:
*   [English Documentation](DOCUMENTATION.md)
*   [Documentación en Español](DOCUMENTATION_ES.md)

---
Developed by **Ramon Nolasco**

