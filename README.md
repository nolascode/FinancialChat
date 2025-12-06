# FinancialChat 💹

A powerful real-time chat application built with .NET 8, featuring decoupled stock quote services via RabbitMQ.

![Financial Chat Application](images/Screenshot%202025-12-06%20091736.png)

## 🚀 Key Features

*   **Real-time Messaging**: Instant communication using SignalR WebSockets.
*   **Stock Quotes**: Get live quotes using `/stock=CODE` (e.g., `/stock=aapl.us`).
*   **Microservices Architecture**: Decoupled Bot service for processing financial data.
*   **Clean Architecture**: Separation of concerns with Domain, Infrastructure, and Presentation layers.
*   **Secure**: JWT Authentication and Identity management.
*   **Docker Ready**: Full containerization with Docker Compose.

### Application Screenshots

#### Login & Registration
<p align="center">
  <img src="images/Screenshot%202025-12-06%20091012.png" alt="Login Screen" width="45%">
  <img src="images/Screenshot%202025-12-06%20091021.png" alt="Register Screen" width="45%">
</p>

#### Chat Room & Stock Bot
![Chat Room with Bot](images/Screenshot%202025-12-06%20091831.png)

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
