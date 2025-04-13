# Software Engineering Coding Test Implementation

This repository contains two C# projects that solve the given coding exercises:

1. **ATM Denomination Calculator** - A console application that calculates all possible combinations of Euro denominations.
2. **Customer API with Simulator** - A REST API for managing customers with a parallel request simulator.

## 1. ATM Denomination Calculator

### Problem Overview
An ATM has three cartridges for different denominations:
- 10 EUR cartridge
- 50 EUR cartridge
- 100 EUR cartridge

The program calculates all possible combinations for various payout amounts (30 EUR, 50 EUR, 60 EUR, 80 EUR, 140 EUR, 230 EUR, 370 EUR, 610 EUR, 980 EUR).

### Implementation Highlights
The solution offers two different algorithms to solve this problem:

1. **Backtracking Algorithm** - A recursive approach that explores all possible combinations by trying different counts of each denomination.
   - Time Complexity: O(n^m) where n is the maximum number of notes per denomination and m is the number of denominations.
   - Better for smaller amounts and fewer denominations.

2. **Dynamic Programming Algorithm** - Builds solutions iteratively by storing and reusing intermediate results.
   - Time Complexity: O(target_amount * num_denominations * num_combinations)
   - More efficient for larger amounts and more denominations.

### Key Features
- Both algorithms are implemented to compare performance for different use cases
- Detailed output of all possible combinations for each requested amount
- Performance measurement to compare the efficiency of both approaches
- User-friendly console interface to choose the algorithm

### How to Run
```
cd src/ATMDenominations
dotnet run
```

## 2. Customer API with Simulator

### Problem Overview
A REST API system to manage customers with proper validation, sorting, and persistence along with a simulator to test it under parallel load.

### API Implementation Highlights

#### Endpoints
- **POST /api/customers** - Adds multiple customers in a sorted manner
- **GET /api/customers** - Returns the array of all customers

#### Features
1. **Customer Validation**
   - Ensures all required fields are supplied
   - Validates that age is above 18
   - Validates that the ID has not been used before

2. **Custom Sorting Implementation**
   - Customers are inserted in sorted order by lastName then firstName
   - Uses a binary search algorithm for efficient insertion without relying on built-in sorting
   - Time complexity of O(log n) for finding insertion position

3. **Persistence**
   - Data is persisted to a JSON file
   - Ensures data is available after server restarts
   - Thread-safe implementation for concurrent access

4. **Thread Safety**
   - Uses locking mechanisms to handle concurrent requests safely
   - Ensures data consistency during parallel operations

### Simulator Implementation Highlights

#### Features
1. **Parallel Request Generation**
   - Sends multiple POST and GET requests in parallel
   - Stress-tests the API under concurrent load
   - Configurable number of requests

2. **Random Customer Generation**
   - Each request contains 2-5 random customers
   - Uses the specified first and last names in random combinations
   - Ages are randomized (now restricted to 18-90 to ensure validity)
   - IDs are generated uniquely within a large range (1-100000) to avoid conflicts

3. **Statistics Tracking**
   - Tracks the total number of customers attempted
   - Records success and failure counts
   - Categorizes errors by reason (Age validation, ID uniqueness issues, etc.)
   - Provides a summary report at the end of the simulation

### Architecture
- **API**: ASP.NET Core with minimal dependencies
- **Simulator**: Standalone console application using HttpClient
- **Data Storage**: File-based persistence using JSON
- **Synchronization**: Thread-safe operations using locks

### How to Run

#### Start the API
```
cd src/API/CustomerApi
dotnet run
```

#### Run the Simulator
```
cd src/API/CustomerApi.Simulator
dotnet run
```
Enter the number of requests when prompted (e.g., 100)

### Output Example
At the end of the simulation, you'll see statistics like:
```
--- SIMULATION STATISTICS ---
Total customers attempted: 359
Total customers succeeded: 206
Total customers failed: 153

Error Reasons:
- Duplicate IDs: 57 occurrences
- Age under 18: 84 occurrences
```

## Project Structure

```
src/
├── ATMDenominations/            # ATM Denomination Calculator
│   ├── ATMDenominations.csproj
│   └── Program.cs
│
└── API/                         # Customer API and Simulator  
    ├── CustomerApi/             # REST API implementation
    │   ├── Controllers/         # API controllers
    │   ├── Models/              # Data models
    │   ├── Services/            # Business logic
    │   └── Program.cs           # API startup
    │
    └── CustomerApi.Simulator/   # Parallel request simulator
        ├── Customer.cs          # Customer model
        ├── CustomerSimulator.cs # Simulation logic
        └── Program.cs           # Entry point
```

## Requirements Met

### ATM Denomination Exercise
- ✅ Calculates all possible combinations for all specified amounts
- ✅ Handles all three denominations (10, 50, 100 EUR)
- ✅ Implements efficient algorithms with performance comparison

### REST API Exercise
- ✅ POST endpoint accepts multiple customers
- ✅ Validates all required fields, age (>18), and unique IDs
- ✅ Implements custom sorting without using built-in sort functions
- ✅ Sorts by lastName then firstName
- ✅ Persists data between server restarts
- ✅ GET endpoint returns all customers

### Simulator Exercise
- ✅ Sends multiple requests in parallel
- ✅ Each request contains at least 2 customers
- ✅ Uses random combinations of the specified first/last names
- ✅ Generates random ages (now 18-90 to avoid validation errors)
- ✅ Uses unique IDs for each customer
- ✅ Tracks statistics of successful and failed operations
- ✅ Groups errors by reason for analysis