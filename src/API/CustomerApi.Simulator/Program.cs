using System.Text;
using System.Text.Json;

namespace CustomerApi.Simulator;

public class Program
{
    private const string BaseUrl = "http://localhost:5128";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Customer API Simulator");
        Console.WriteLine("Press Ctrl+C to exit.");

        Console.WriteLine("How many requests do you want to send?");
        var numberOfRequests = Convert.ToInt32(Console.ReadLine());
        
        if (numberOfRequests < 1)
        {
            Console.WriteLine("Number of requests must be at least 1.");
            return;
        }
        
        var simulator = new CustomerSimulator(BaseUrl);
        await simulator.RunSimulationAsync(numberOfRequests);
    }
}