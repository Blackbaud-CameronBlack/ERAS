using EmergencyRoadsideAssistanceService.Interfaces;
using EmergencyRoadsideAssistanceService.Models;
using EmergencyRoadsideAssistanceService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

// Dependency injection to wire everything up
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<IAssistantService, AssistantService>();
        services.AddSingleton<IAssistantLocationService, AssistantLocationService>();
        services.AddSingleton<IRoadsideAssistanceService, RoadsideAssistanceService>();
    })
    .Build();

// Get the services we will use for the demo: Assistant, Customer, and RoadsideAssistanceService (main one)
var assistantService = host.Services.GetRequiredService<IAssistantService>();
var customerService = host.Services.GetRequiredService<ICustomerService>();
var roadsideAssistanceService = host.Services.GetRequiredService<IRoadsideAssistanceService>();

// Load 1000 random locations for our assistants
var locations = LoadRandomLocations(1000);
var assistantId = 1;
foreach (var loc in locations)
{
    var assistant = new Assistant
    {
        AssistantId = assistantId++,
        AssistantName = $"Assistant {assistantId}",
        CurrentLocation = loc
    };
    assistantService.UpsertAssistant(assistant);
}

// Set the customer location to Grand Forks
var customer = new Customer { CustomerId = 1, CustomerName = "Customer 1", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
customerService.UpsertCustomer(customer);

// Test 1
// Show top 5 assistants closest to the customer
ShowNearestAssistants(roadsideAssistanceService, customer);

// Try some contrived examples
assistantService.ClearAssistants();

// Add 4 assistants
var a1 = new Assistant { AssistantId = 1, AssistantName = $"Assistant 1", CurrentLocation = new Geolocation(46.8737648, -96.76780389999999, "Moorhead") };
var a2 = new Assistant { AssistantId = 2, AssistantName = $"Assistant 2", CurrentLocation = new Geolocation(46.8771863, -96.7898034, "Fargo") };
var a3 = new Assistant { AssistantId = 3, AssistantName = $"Assistant 3", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
var a4 = new Assistant { AssistantId = 4, AssistantName = $"Assistant 4", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
assistantService.UpsertAssistant(a1);
assistantService.UpsertAssistant(a2);
assistantService.UpsertAssistant(a3);
assistantService.UpsertAssistant(a4);

// Test 2
// Show top 5 assistants closest to the customer (will only show 4 here)
Console.WriteLine("Showing top 5 assistants, but there are only 4 online");
ShowNearestAssistants(roadsideAssistanceService, customer);

// Test 3
Console.WriteLine("Reserving an assistant");
var reservedAssistant = roadsideAssistanceService.ReserveAssistant(customer, customer.CurrentLocation);
Console.WriteLine($"Currently reserved: {reservedAssistant}");

// Show top 5 assistants closest to the customer (will only show 3 here since one is reserved)
Console.WriteLine("Showing assistants left after reservation");
ShowNearestAssistants(roadsideAssistanceService, customer);

// Test 4
// Try to reserve another assistant without releasing the first one, should return null
Console.WriteLine("Trying to reserve another assistant without releasing the prior one");
var anotherReservedAssistant = roadsideAssistanceService.ReserveAssistant(customer, customer.CurrentLocation);
Console.WriteLine($"Null returned trying to reserve another assistant: {anotherReservedAssistant == null}");

// Test 5
// Add 4 more customers, all from the same crash in Grand Forks
var c2 = new Customer { CustomerId = 2, CustomerName = "Customer 2", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
var c3 = new Customer { CustomerId = 3, CustomerName = "Customer 3", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
var c4 = new Customer { CustomerId = 4, CustomerName = "Customer 4", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
var c5 = new Customer { CustomerId = 5, CustomerName = "Customer 5", CurrentLocation = new Geolocation(47.9252568, -97.0328547, "Grand Forks") };
customerService.UpsertCustomer(c2);
customerService.UpsertCustomer(c3);
customerService.UpsertCustomer(c4);
customerService.UpsertCustomer(c5);

// Try to reserve assistants, all should get one except the last
Console.WriteLine("Reserving remaining assistants...");
Console.WriteLine($"Customer 2 reserving assistant: {roadsideAssistanceService.ReserveAssistant(c2, c2.CurrentLocation)}");
Console.WriteLine($"Customer 3 reserving assistant: {roadsideAssistanceService.ReserveAssistant(c3, c3.CurrentLocation)}");
Console.WriteLine($"Customer 4 reserving assistant: {roadsideAssistanceService.ReserveAssistant(c4, c4.CurrentLocation)}");
Console.WriteLine($"Customer 5 reserving assistant: {roadsideAssistanceService.ReserveAssistant(c5, c5.CurrentLocation)}");

// Test 6
// Release the original reservation
roadsideAssistanceService.ReleaseAssistant(customer, reservedAssistant!);
Console.WriteLine("Released first assistant, there should be 1 available now");
ShowNearestAssistants(roadsideAssistanceService, customer);
Console.WriteLine($"Customer 5 reserving assistant: {roadsideAssistanceService.ReserveAssistant(c5, c5.CurrentLocation)}");
return;





// Dumps nearest assistants
static void ShowNearestAssistants(IRoadsideAssistanceService roadsideAssistanceService, Customer customer, int limit = 5)
{
    Console.WriteLine("Top 5 nearest assistants");
    foreach (var assistant in roadsideAssistanceService.FindNearestAssistants(customer.CurrentLocation, limit))
    {
        Console.WriteLine($"Distance in km: {assistant.DistanceFromCustomer} - {assistant.AssistantName} - {assistant.CurrentLocation.LocationName}");
    }
    Console.WriteLine();
}

// Loads random city locations from a json file
static List<Geolocation> LoadRandomLocations(int locationNum)
{
    Console.WriteLine("Generating random locations");
    var randomLocations = new List<Geolocation>();
    var cities = JsonSerializer.Deserialize <List<GeoCity>>(File.ReadAllText(@"Data\cities.json"));
    if (cities != null)
    {
        Random random = new Random();
        for (int i = 0; i < locationNum; i++)
        {
            var loc = cities[random.Next(cities.Count)];
            randomLocations.Add(
                new Geolocation
                {
                    LocationName = loc.city,
                    Latitude = loc.latitude,
                    Longitude = loc.longitude
                });
            Console.WriteLine(loc.city);
        }
    }
    return randomLocations;
}

public class GeoCity
{
    public string city { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
    public double latitude { get; set; }
    public double longitude { get; set; }
}
