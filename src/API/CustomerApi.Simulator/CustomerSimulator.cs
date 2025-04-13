using System.Text;
using System.Text.Json;

namespace CustomerApi.Simulator;

public class CustomerSimulator(string baseUrl)
{
    private readonly HttpClient _httpClient = new();
    private readonly Random _random = new();
    private readonly HashSet<int> _usedIds = [];
    private readonly object _idLock = new();

    private int _totalCustomersAttempted = 0;
    private int _totalCustomersSucceeded = 0;
    private Dictionary<string, int> _errorReasons = new();

    private readonly string[] _firstNames =
    [
        "Leia", "Sadie", "Jose", "Sara", "Frank",
        "Dewey", "Tomas", "Joel", "Lukas", "Carlos"
    ];

    private readonly string[] _lastNames =
    [
        "Liberty", "Ray", "Harrison", "Ronan", "Drew",
        "Powell", "Larsen", "Chan", "Anderson", "Lane"
    ];

    public async Task RunSimulationAsync(int numberOfRequests, int maxCustomersPerRequest = 5)
    {
        Console.WriteLine("Starting customer API simulation...");

        var tasks = new List<Task>();

        for (var i = 0; i < numberOfRequests; i++)
        {
            tasks.Add(SendPostCustomersRequestAsync(GenerateRandomCustomers(2, maxCustomersPerRequest)));
        }

        for (var i = 0; i < numberOfRequests / 2; i++)
        {
            tasks.Add(SendGetCustomersRequestAsync());
        }

        await Task.WhenAll(tasks);
        await SendGetCustomersRequestAsync();

        DisplayStatistics();

        Console.WriteLine("Simulation completed.");
    }

    private void DisplayStatistics()
    {
        Console.WriteLine("\n--- SIMULATION STATISTICS ---");
        Console.WriteLine($"Total customers attempted: {_totalCustomersAttempted}");
        Console.WriteLine($"Total customers succeeded: {_totalCustomersSucceeded}");
        Console.WriteLine($"Total customers failed: {_totalCustomersAttempted - _totalCustomersSucceeded}");

        if (_errorReasons.Count > 0)
        {
            Console.WriteLine("\nError Reasons:");
            foreach (var error in _errorReasons.Where(e => !string.IsNullOrWhiteSpace(e.Key) && e.Value > 0))
            {
                Console.WriteLine($"- {error.Key}: {error.Value} occurrences");
            }
        }
    }

    private List<Customer> GenerateRandomCustomers(int minCount, int maxCount)
    {
        var count = _random.Next(minCount, maxCount + 1);
        var customers = new List<Customer>();

        for (var i = 0; i < count; i++)
        {
            int id;
            lock (_idLock)
            {
                do
                {
                    id = _random.Next(1, 100001);
                } while (_usedIds.Contains(id));

                _usedIds.Add(id);
            }

            customers.Add(new Customer
            {
                Id = id,
                FirstName = _firstNames[_random.Next(_firstNames.Length)],
                LastName = _lastNames[_random.Next(_lastNames.Length)],
                Age = _random.Next(18, 91)
            });
        }

        return customers;
    }

    private async Task SendPostCustomersRequestAsync(List<Customer> customers)
    {
        try
        {
            _totalCustomersAttempted += customers.Count;

            var customersJson = JsonSerializer.Serialize(customers);
            var content = new StringContent(customersJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/customers", content);

            if (response.IsSuccessStatusCode)
            {
                _totalCustomersSucceeded += customers.Count;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                var errorReason = ParseErrorReason(errorContent);
                if (!string.IsNullOrWhiteSpace(errorReason))
                {
                    if (_errorReasons.ContainsKey(errorReason))
                        _errorReasons[errorReason] += customers.Count;
                    else
                        _errorReasons[errorReason] = customers.Count;
                }
            }
        }
        catch (Exception ex)
        {
            var errorReason = $"Exception: {ex.Message}";

            if (_errorReasons.TryGetValue(errorReason, out var value))
                _errorReasons[errorReason] = ++value;
            else
                _errorReasons[errorReason] = 1;
        }
    }

    private string ParseErrorReason(string errorContent)
    {
        if (string.IsNullOrWhiteSpace(errorContent))
        {
            return "Unknown error";
        }

        if (errorContent.Contains("Age must be at least 18"))
        {
            return "Age under 18";
        }

        if (errorContent.Contains("unique"))
        {
            return "Duplicate IDs";
        }

        if (errorContent.Contains("At least one customer must be provided"))
        {
            return "Empty customer list";
        }

        return errorContent.Length > 50 ? string.Concat(errorContent.AsSpan(0, 50), "...") : errorContent;
    }

    private async Task SendGetCustomersRequestAsync()
    {
        try
        {
            await _httpClient.GetAsync($"{baseUrl}/api/customers");
        }
        catch
        {
            // Silently ignore get request errors to avoid cluttering the console
        }
    }
}