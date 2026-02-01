# Order Processing Task

## Project Overview

A .NET-based order processing system demonstrating layered architecture, dependency injection, async/await patterns, and concurrent order processing.

## How to Run

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/dawidbak/order-processing-task.git
   cd order-processing-task
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/OrderProcessingTask/OrderProcessingTask.csproj
   ```


5. **Run tests**
   ```bash
   dotnet test
   ```
You can also perform the above steps in your favorite IDE.
## Architecture Diagram
<img width="1569" height="251" alt="image" src="https://github.com/user-attachments/assets/db0da932-48a4-4211-aeb7-572842afd293" />
<img width="1478" height="262" alt="image" src="https://github.com/user-attachments/assets/6729c39e-40c4-43c7-88a0-2fd390437f24" />
Code for mermaid

 ```bash
   flowchart TD
    subgraph AddOrder["AddOrderAsync"]
        A_Start([Start: AddOrderAsync]) --> A_Log[Log: Adding new order]
        A_Log --> A_Val{Valid ID?}
        A_Val -->|No| A_Err[Log Error]
        A_Err --> A_End1([End])
        A_Val -->|Yes| A_Store[Store in Dictionary]
        A_Store --> A_Success{Success?}
        A_Success -->|No| A_Dup[OrderAlreadyExistsException]
        A_Dup --> A_End2([End with Error])
        A_Success -->|Yes| A_Log2[Log Success]
        A_Log2 --> A_Notify[Notify: New Order]
        A_Notify --> A_End3([End Success])
    end
    
    subgraph ProcessOrder["ProcessOrderAsync"]
        P_Start([Start: ProcessOrderAsync]) --> P_Log[Log: Starting processing]
        P_Log --> P_Val{Valid ID?}
        P_Val -->|No| P_Err[Log Error]
        P_Err --> P_End1([End])
        P_Val -->|Yes| P_Get[Get Order]
        P_Get --> P_Exists{Exists?}
        P_Exists -->|No| P_NotFound[KeyNotFoundException]
        P_NotFound --> P_End2([End with Error])
        P_Exists -->|Yes| P_Process[Process Order]
        P_Process --> P_Log2[Log Success]
        P_Log2 --> P_Notify[Notify: Processed]
        P_Notify --> P_End3([End Success])
    end
    
    style A_Start fill:#4CAF50
    style P_Start fill:#4CAF50
    style A_End3 fill:#4CAF50
    style P_End3 fill:#4CAF50
    style A_Val fill:#FFC107
    style P_Val fill:#FFC107
    style A_Success fill:#FFC107
    style P_Exists fill:#FFC107
    style A_Err fill:#f44336
    style P_Err fill:#f44336
    style A_Dup fill:#f44336
    style P_NotFound fill:#f44336
    style A_Notify fill:#9C27B0
    style P_Notify fill:#9C27B0t
   ```

## Completed bonus tasks

- Asynchronous Processing
- Add Order (CRUD)
- IOrderValidator
- Unit Tests
- Configuration via appsettings.json
- Notification Service

