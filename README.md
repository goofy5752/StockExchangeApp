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

*   **Purpose**: Handles the creation, validation, and processing of stock orders.
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
