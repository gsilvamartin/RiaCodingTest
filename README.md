# ATM Denomination Calculator

This is a C# console application that calculates all possible combinations of Euro denominations that an ATM can dispense for various requested amounts.

## Problem Statement

An ATM has three cartridges for different denominations:
- 10 EUR cartridge
- 50 EUR cartridge
- 100 EUR cartridge

The program calculates all possible combinations for the following payout amounts:
- 30 EUR
- 50 EUR
- 60 EUR
- 80 EUR
- 140 EUR
- 230 EUR
- 370 EUR
- 610 EUR
- 980 EUR

## Implementation

The solution offers two different algorithms to solve this problem:

1. **Backtracking Algorithm** - A recursive approach that explores all possible combinations by trying different counts of each denomination.
   - Time Complexity: O(n^m) where n is the maximum number of notes per denomination and m is the number of denominations.
   - Better for smaller amounts and fewer denominations.

2. **Dynamic Programming Algorithm** - Builds solutions iteratively by storing and reusing intermediate results.
   - Time Complexity: O(target_amount * num_denominations * num_combinations)
   - More efficient for larger amounts and more denominations.

The application includes performance measurement to compare the efficiency of both approaches.

## How to Run

1. Navigate to the project directory
```
cd src/ATMDenominations
```

2. Build the project
```
dotnet build
```

3. Run the application
```
dotnet run
```

4. Choose the algorithm (1 for Backtracking, 2 for Dynamic Programming)

## Example Output

For 100 EUR, the available payout denominations would be:
- 1 x 100 EUR
- 2 x 50 EUR
- 1 x 50 EUR + 5 x 10 EUR
- 10 x 10 EUR

## Performance Considerations

- For the small amounts in this example, backtracking may actually perform better due to less overhead.
- For larger amounts (thousands or more) or many denominations, dynamic programming becomes significantly faster.
- The implementation includes performance timing to help compare the approaches.