* * *

Stock Portfolio Management Microservices
========================================

Overview
--------

This project is a microservices-based application designed for managing stock portfolios in real-time. It consists of three primary services: **Order Service**, **Portfolio Service**, and **Price Service**. Each service is responsible for a specific aspect of the application, and they communicate with each other using RabbitMQ for message passing.

### Key Features

*   **Order Management**: Handles the creation and management of stock orders, including buy and sell operations.
*   **Portfolio Management**: Manages users' portfolios, updating stock holdings and valuations based on executed orders and real-time price updates.
*   **Real-Time Price Updates**: Generates and broadcasts real-time stock prices to ensure accurate portfolio valuations.

Microservices Architecture
--------------------------

### 1\. **Order Service**

*   **Purpose**: Handles stock order creation, validation, and processing.
*   **Key Components**:
    *   **OrderController**: Provides endpoints for creating and managing orders.
    *   **RabbitMQ Integration**: Publishes order execution events to `order_exchange` for other services to consume.
    *   **Concurrency Handling**: Ensures data consistency during concurrent order processing using `ConcurrentDictionary`.

### 2\. **Portfolio Service**

*   **Purpose**: Manages the user's portfolio, including updating holdings based on executed orders and real-time price updates.
*   **Key Components**:
    *   **PortfolioService**: Processes orders and updates portfolio items accordingly.
    *   **RabbitMQ Consumers**: Listens for order execution and price update events from RabbitMQ queues.
    *   **Concurrency-Safe Methods**: Ensures accurate updates to portfolio data under concurrent operations.

### 3\. **Price Service**

*   **Purpose**: Generates and broadcasts real-time stock prices.
*   **Key Components**:
    *   **PriceGeneratorService**: Periodically generates random stock prices for a list of predefined tickers.
    *   **RabbitMQ Integration**: Publishes price updates to `price_exchange`, allowing the Portfolio Service to update valuations.

Communication Between Services
------------------------------

The services communicate using RabbitMQ as the messaging broker:

*   **Exchanges**:
    *   `order_exchange`: Used by the Order Service to publish order execution events.
    *   `price_exchange`: Used by the Price Service to broadcast real-time price updates.
*   **Queues**:
    *   `OrderUpdateQueue`: Subscribed to by the Portfolio Service to receive order execution events.
    *   `PriceUpdateQueue`: Subscribed to by the Portfolio Service to receive price updates.


Getting Started
---------------

### Prerequisites

Before running the projects, ensure that you have the following software installed:

1.  **.NET 6 SDK and Runtime**:
    
    *   You need the .NET 6 SDK installed on your machine to develop, build, and run the projects.
    *   Download and install .NET 6 from the official .NET website: [Download .NET 6](https://dotnet.microsoft.com/download/dotnet/6.0).
2.  **RabbitMQ**: The RabbitMQ server should be running on your machine. You can install it locally or run it using Docker.
    

### Setting Up RabbitMQ

1.  **Install RabbitMQ**:
    *   Download and install RabbitMQ from the official website: [Download RabbitMQ](https://www.rabbitmq.com/docs/download).
        
    *   **RabbitMQ Management Console**: Accessible at `http://localhost:15672` with default credentials (`guest`/`guest`).

### Running the Services

Each service can be run individually. Below are the steps to run each service:

#### 1\. Order Service

##### Step 1: Navigate to the Order Service Directory

Open a terminal or command prompt and navigate to the `Order` directory:

`cd /path/to/StockExchangeApp/Order`

##### Step 2: Restore Dependencies

Ensure that all necessary NuGet packages are restored:

`dotnet restore`

##### Step 3: Update `appsettings.json`

Edit the `appsettings.json` file in the `OrderApi`, to configure your local environment:

*   **Connection Strings**: Update the `DefaultConnection` to point to your local machine's SQL Server instance.

Example `appsettings.json`:

`  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_LOCAL_SERVER_NAME;Database=Order;Trusted_Connection=True;MultipleActiveResultSets=true"
  }`

*   Replace `YOUR_LOCAL_SERVER_NAME` with the actual name of your SQL Server instance.

##### Step 4: Run Database Migrations

After updating the `appsettings.json`, run the following command to apply database migrations:

`update-database` in the package manager console

This will ensure your database schema is up-to-date.

##### Step 5: Run the Order Service

`dotnet run`

This will start the Order Service, which will listen to incoming HTTP requests related to order management and publish events for RabbitMQ.

#### 2\. Portfolio Service

##### Step 1: Navigate to the Portfolio Service Directory

Open a terminal and navigate to the `Portfolio` directory:

`cd /path/to/StockExchangeApp/Portfolio`

##### Step 2: Restore Dependencies

Ensure that all necessary NuGet packages are restored:

`dotnet restore`

##### Step 3: Update `appsettings.json`

Edit the `appsettings.json` file in the `PortfolioApi`, to configure your local environment:

*   **Connection Strings**: Update the `DefaultConnection` to point to your local machine's SQL Server instance.

Example `appsettings.json`:

`  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_LOCAL_SERVER_NAME;Database=Order;Trusted_Connection=True;MultipleActiveResultSets=true"
  }`
  
*   Replace `YOUR_LOCAL_SERVER_NAME` with the actual name of your SQL Server instance.

##### Step 4: Run Database Migrations

After updating the `appsettings.json`, run the following command to apply database migrations:

`update-database` in the package manager console

This will ensure your database schema is up-to-date.

##### Step 5: Run the Portfolio Service

`dotnet run`

This will start the Portfolio Service, which will consume messages from RabbitMQ related to order executions and price updates, and update portfolio data accordingly.

#### 3\. Price Service

##### Step 1: Navigate to the Price Service Directory

Open a terminal and navigate to the `Price` directory:

`cd /path/to/StockExchangeApp/Price`

##### Step 2: Restore Dependencies

Ensure that all necessary NuGet packages are restored:

`dotnet restore`

**Note**: The Price Service does not use a database, so you need not update any database connection strings or run migrations for this service.

##### Step 4: Run the Price Service

`dotnet run`

This will start the Price Service, which will generate and broadcast stock price updates to RabbitMQ.

### Common Issues and Troubleshooting

*   **RabbitMQ Connection Issues**: Ensure RabbitMQ is running and accessible at the configured `HostName`. Check the RabbitMQ management console for any issues.
*   **Database Connection Issues**: Ensure that your database is up and running. Verify the connection strings in `appsettings.json`.
*   **Port Conflicts**: Ensure that each service is running on a different port, or configure them to avoid conflicts.
