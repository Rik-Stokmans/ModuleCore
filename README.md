# Project Overview

## Introduction

This project is a C# application that leverages Azure Functions to create serverless APIs. The project includes a logic layer, a mock data layer, and an API layer that generates Azure Functions dynamically based on annotated methods in the logic layer.

## Project Structure

### Logic Layer

**Functions:**
- Contains core business logic and interfaces.
- Includes methods annotated for HTTP triggers, which are used to generate Azure Functions.

**Dependencies:**
- Interfaces and models that define the structure of the data and the contracts for the services.

**Folder Structure:**
```
LogicLayer/
├── Core/
│   ├── Core.cs
│   └── OtherCoreFiles.cs
├── Interfaces/
│   ├── ILogService.cs
│   └── OtherInterfaces.cs
└── Models/
    ├── OperationResult.cs
    └── OtherModels.cs
```

### Mock Data Layer

**Functions:**
- Provides mock implementations of the services defined in the logic layer.
- Useful for testing and development purposes.

**Dependencies:**
- Implements the interfaces defined in the logic layer.

**Folder Structure:**
```
MockDataLayer/
└── Services/
    ├── LogMockService.cs
    └── OtherMockServices.cs
```

### API Layer

**Functions:**
- Includes an `AzureFunctionGenerator` class that dynamically generates Azure Functions based on the methods in the logic layer.
- The generated functions handle HTTP requests, validate input parameters, and call the corresponding service methods.

**Dependencies:**
- Depends on the logic layer for the core business logic and interfaces.
- Uses Azure Functions SDK for creating serverless functions.
- Uses ASP.NET Core for handling HTTP requests and responses.

**Folder Structure:**
```
FunctionApi/
├── AzureFunctionGenerator.cs
├── bin/
│   └── Debug/
│       └── net8.0/
│           └── Generated/
│               ├── LogServiceCreateLog.cs
│               └── OtherGeneratedFunctions.cs
└── Generated/
    ├── LogServiceCreateLog.cs
    └── OtherGeneratedFunctions.cs
```

## Conclusion

This project demonstrates a dynamic approach to generating Azure Functions based on annotated methods in the logic layer. It includes a mock data layer for testing and a robust API layer for handling HTTP requests and responses. The use of Azure Functions allows for scalable and efficient serverless API development.

## License

This project is licensed under the Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International (CC BY-NC-ND 4.0). You may share it as long as you provide proper attribution, but you cannot modify or sell the code.
