This provides a REST API for managing:

- **Groups**
- **Charging Stations**
- **Connectors**

with all domain rules and validation enforced.

The solution also includes a complete **integration and unit test project**.

---

## Technologies Used
- .NET 10 (or .NET 8 depending on local SDK)
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- xUnit test framework
- Swagger/OpenAPI for API documentation

---

# Build and Run

Make sure you are located in the EVChargingRepo and can see the .sln for the project.
Then run these...

### 1. Restore packages
```sh
dotnet restore
```

### 2. Build the solution
```sh
dotnet build
```

### 3. Run the API
```sh
dotnet run --project EVCharging
```

The application will start on the ports defined in `launchSettings.json`.
https://localhost:7295;http://localhost:5247

---

# Swagger / OpenAPI Documentation

Once the API is running, open:

```
http://localhost:5247/swagger

or

https://localhost:7295/swagger

```

Swagger shows:

- All API endpoints
- Request/response models
- Example payloads
- Validation rules

---

# Running the Tests

The full test suite is located in the **EVChargingTests** project.

Run tests using:

```sh
dotnet test
```

Tests include:

- Unit tests for all services and validation logic  
- Integration tests for all controllers  
- Test data builders for clean test setup  

The tests run against an EF Core **InMemory database** for isolation.

---

# Project Structure

```
EVChargingRepo/
│   EVCharging.sln
│   README.md
│   .gitignore
│
├── EVCharging/          # Main API project
│     Controllers/
│     Services/
│     Models/
│     Dtos/
│     Data/
│     Program.cs
│
└── EVChargingTests/     # Full test suite
      Builders/
      Controllers/
      Services/
      Utility/ 
```

---

# Notes

- No external setup is required—database is in-memory.  
- All domain constraints (connector limits, load validation, group capacity, etc.) are enforced.
- Swagger UI is enabled only in **Development** environment.

--- 
