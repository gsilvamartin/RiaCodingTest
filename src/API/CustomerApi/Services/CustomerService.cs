using System.Text.Json;
using CustomerApi.Models;

namespace CustomerApi.Services;

public class CustomerService
{
    private const string DataFilePath = "customers.json";

    private List<Customer> _customers = [];
    private readonly Lock _lock = new();

    public CustomerService()
    {
        LoadCustomers();
    }

    public IEnumerable<Customer> GetAllCustomers()
    {
        lock (_lock)
        {
            return _customers.ToList();
        }
    }

    public bool AddCustomers(List<Customer> newCustomers)
    {
        lock (_lock)
        {
            foreach (var customer in newCustomers)
            {
                if (customer.Age < 18)
                {
                    return false;
                }

                if (_customers.Any(c => c.Id == customer.Id))
                {
                    return false;
                }
            }

            foreach (var customer in newCustomers)
                InsertCustomerInOrder(customer);

            SaveCustomers();
            return true;
        }
    }

    private void InsertCustomerInOrder(Customer customer)
    {
        if (_customers.Count == 0)
        {
            _customers.Add(customer);
            return;
        }

        var position = FindInsertPosition(customer, 0, _customers.Count - 1);
        _customers.Insert(position, customer);
    }

    private int FindInsertPosition(Customer customer, int start, int end)
    {
        while (true)
        {
            if (start > end)
                return start;

            var mid = (start + end) / 2;

            var comparison = string.CompareOrdinal(customer.LastName, _customers[mid].LastName);

            if (comparison == 0)
                comparison = string.CompareOrdinal(customer.FirstName, _customers[mid].FirstName);

            if (comparison < 0)
            {
                end = mid - 1;
                continue;
            }

            start = mid + 1;
        }
    }

    private void LoadCustomers()
    {
        try
        {
            if (!File.Exists(DataFilePath)) return;
            var json = File.ReadAllText(DataFilePath);

            _customers = JsonSerializer.Deserialize<List<Customer>>(json) ?? [];
        }
        catch (Exception)
        {
            _customers = [];
        }
    }

    private void SaveCustomers()
    {
        try
        {
            var json = JsonSerializer.Serialize(_customers);
            File.WriteAllText(DataFilePath, json);
        }
        catch (Exception)
        {
            // Log the error
        }
    }
}