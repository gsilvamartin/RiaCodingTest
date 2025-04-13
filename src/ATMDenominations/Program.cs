using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ATMDenominations
{
    /*
    * ATM Denominations Algorithm Performance Analysis
    *
    * Performance Comparison:
    *
    * 1. Backtracking Approach:
    *    - Time Complexity: O(n^m) where n is the maximum number of notes of each denomination
    *      that can be used (target/smallest denomination) and m is the number of denominations.
    *    - Space Complexity: O(m) for the recursion stack.
    *    - For small problems (few denominations, small amounts), backtracking can be faster due to
    *      less overhead and simpler implementation.
    *
    * 2. Dynamic Programming Approach:
    *    - Time Complexity: O(target_amount * num_denominations * num_combinations) where num_combinations
    *      refers to the average number of combinations for each amount (due to duplicate checking).
    *    - Space Complexity: O(target_amount * max_combinations) for storing all combinations.
    *    - For larger problems (many denominations or large amounts), DP becomes significantly faster
    *      as it avoids recalculating the same subproblems.
    *
    * When to use which approach:
    * - For small amounts and few denominations (like in this example): Backtracking is simpler and often fast enough.
    * - For large amounts (thousands or more) or many denominations: Dynamic Programming is vastly superior.
    * - If memory is very limited: Backtracking uses less memory.
    *
    * Note: The current implementation of the Dynamic Programming approach includes duplicate checking
    * which adds overhead. An optimized implementation could improve performance significantly.
    */
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("===========================");
            Console.WriteLine("ATM Denomination Calculator");
            Console.WriteLine("===========================");

            // Available denominations
            int[] denominations = { 10, 50, 100 };

            // Amounts to check
            int[] amounts = { 30, 50, 60, 80, 140, 230, 370, 610, 980 };

            Console.WriteLine("Choose algorithm:");
            Console.WriteLine("1. Backtracking");
            Console.WriteLine("2. Dynamic Programming");
            Console.Write("Enter your choice (1 or 2): ");

            string choice = Console.ReadLine();
            bool useDynamicProgramming = choice == "2";

            foreach (int amount in amounts)
            {
                Console.WriteLine($"\nPossible combinations for {amount} EUR:");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                if (useDynamicProgramming)
                {
                    CalculateCombinationsDynamic(denominations, amount);
                }
                else
                {
                    CalculateCombinationsBacktracking(denominations, amount);
                }

                stopwatch.Stop();
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine("--------------------------");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Backtracking Approach

        static void CalculateCombinationsBacktracking(int[] denominations, int targetAmount)
        {
            // Sort denominations in descending order for a cleaner output
            Array.Sort(denominations);
            Array.Reverse(denominations);

            List<Dictionary<int, int>> combinations = new List<Dictionary<int, int>>();
            Dictionary<int, int> currentCombination = new Dictionary<int, int>();

            foreach (int denom in denominations)
            {
                currentCombination[denom] = 0;
            }

            FindCombinations(denominations, targetAmount, 0, currentCombination, combinations);

            DisplayCombinations(combinations);
        }

        // Backtracking algorithm to find all combinations of denominations that sum up to the target amounts
        static void FindCombinations(
            int[] denominations,
            int remainingAmount,
            int currentIndex,
            Dictionary<int, int> currentCombination,
            List<Dictionary<int, int>> combinations
        )
        {
            // Base case: If remaining amount is 0, we found a valid combination
            if (remainingAmount == 0)
            {
                combinations.Add(new Dictionary<int, int>(currentCombination));
                return;
            }

            // Base case: If we've used all denominations or remaining amount is negative
            if (currentIndex >= denominations.Length || remainingAmount < 0)
            {
                return;
            }

            int currentDenomination = denominations[currentIndex];

            // Calculate maximum number of current denomination that can be used
            int maxCount = remainingAmount / currentDenomination;

            // Try different counts of the current denomination
            for (int count = 0; count <= maxCount; count++)
            {
                currentCombination[currentDenomination] = count;
                int newRemainingAmount = remainingAmount - (count * currentDenomination);

                // Recursively find combinations for the remaining amount using next denomination
                FindCombinations(
                    denominations,
                    newRemainingAmount,
                    currentIndex + 1,
                    currentCombination,
                    combinations
                );
            }

            // Reset the count for backtracking
            currentCombination[currentDenomination] = 0;
        }

        #endregion

        #region Dynamic Programming Approach

        static void CalculateCombinationsDynamic(int[] denominations, int targetAmount)
        {
            // Sort denominations to make sure we're consistent
            Array.Sort(denominations);

            // This will hold all possible combinations for each amount
            List<List<Dictionary<int, int>>> allCombinations =
                new List<List<Dictionary<int, int>>>();

            // Initialize the list for each amount from 0 to targetAmount
            for (int i = 0; i <= targetAmount; i++)
            {
                allCombinations.Add(new List<Dictionary<int, int>>());
            }

            // Base case: amount 0 has one combination (using no notes)
            Dictionary<int, int> emptyCombination = new Dictionary<int, int>();
            foreach (int denom in denominations)
            {
                emptyCombination[denom] = 0;
            }

            allCombinations[0].Add(emptyCombination);

            // Build the table
            foreach (int denom in denominations)
            {
                // Start from the denomination value to avoid negative indices
                for (int amount = denom; amount <= targetAmount; amount++)
                {
                    // For each combination at (amount - denom), add one more note of denom
                    foreach (var combination in allCombinations[amount - denom])
                    {
                        Dictionary<int, int> newCombination = new Dictionary<int, int>(combination);
                        newCombination[denom]++;

                        // Check if this combination is unique (avoid duplicates)
                        bool isDuplicate = false;
                        foreach (var existingComb in allCombinations[amount])
                        {
                            if (AreCombinationsEqual(existingComb, newCombination))
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate)
                        {
                            allCombinations[amount].Add(newCombination);
                        }
                    }
                }
            }

            // Sort combinations for consistent output (larger denominations first)
            List<Dictionary<int, int>> sortedCombinations = new List<Dictionary<int, int>>(
                allCombinations[targetAmount]
            );
            sortedCombinations.Sort((a, b) => CompareCombinations(a, b, denominations));

            // Display combinations
            DisplayCombinations(sortedCombinations);
        }

        // Helper method to check if two combinations are the same
        static bool AreCombinationsEqual(Dictionary<int, int> combo1, Dictionary<int, int> combo2)
        {
            if (combo1.Count != combo2.Count)
                return false;

            foreach (var key in combo1.Keys)
            {
                if (!combo2.ContainsKey(key) || combo1[key] != combo2[key])
                    return false;
            }

            return true;
        }

        // Helper method to compare combinations for sorting
        static int CompareCombinations(
            Dictionary<int, int> combo1,
            Dictionary<int, int> combo2,
            int[] denominations
        )
        {
            // Sort by highest denomination first
            Array.Sort(denominations);
            Array.Reverse(denominations);

            foreach (var denom in denominations)
            {
                int diff = combo2[denom] - combo1[denom]; // Descending order
                if (diff != 0)
                    return diff;
            }

            return 0;
        }

        #endregion

        #region Common Display Method

        static void DisplayCombinations(List<Dictionary<int, int>> combinations)
        {
            if (combinations.Count == 0)
            {
                Console.WriteLine("No valid combinations found.");
                return;
            }

            // Display combinations
            int count = 1;
            foreach (var combination in combinations)
            {
                List<string> parts = new List<string>();

                // Sort keys in descending order for display
                List<int> sortedKeys = new List<int>(combination.Keys);
                sortedKeys.Sort();
                sortedKeys.Reverse();

                foreach (var denom in sortedKeys)
                {
                    if (combination[denom] > 0)
                    {
                        parts.Add($"{combination[denom]} x {denom} EUR");
                    }
                }

                Console.WriteLine($"{count}. {string.Join(" + ", parts)}");
                count++;
            }
        }

        #endregion
    }
}
