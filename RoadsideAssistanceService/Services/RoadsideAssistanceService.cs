using EmergencyRoadsideAssistanceService.Interfaces;
using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Services
{
    public class RoadsideAssistanceService : IRoadsideAssistanceService
    {        
        private readonly ICustomerService _customerService;
        private readonly IAssistantService _assistantService;
        private const int MAX_LIMIT = 100; // Get this from configuration 

        public RoadsideAssistanceService(ICustomerService customerService, IAssistantService assistantService)
        {
            if (customerService == null) throw new ArgumentNullException(nameof(customerService));
            if (assistantService == null) throw new ArgumentNullException(nameof(assistantService));
            
            _customerService = customerService;
            _assistantService = assistantService;
        }

        public SortedSet<Assistant> FindNearestAssistants(Geolocation geolocation, int limit)
        {
            return _assistantService.FindNearestAssistants(geolocation, limit);
        }

        public void ReleaseAssistant(Customer customer, Assistant assistant)
        {
            customer.CurrentlyEngaged = false;
            _customerService.UpsertCustomer(customer);

            assistant.CurrentlyEngaged = false;
            _assistantService.UpsertAssistant(assistant);
        }

        public Assistant? ReserveAssistant(Customer customer, Geolocation customerLocation)
        {
            // Don't reserve another assistant if already being helped
            if (customer.CurrentlyEngaged) return null; 

            var closestAssistants = _assistantService.FindNearestAssistants(customerLocation, MAX_LIMIT);
            if (closestAssistants.Any())
            {
                var assistant = closestAssistants.First();
                if (assistant != null)
                {
                    assistant.CurrentlyEngaged = true;
                    _assistantService.UpsertAssistant(assistant);

                    customer.CurrentlyEngaged = true;
                    _customerService.UpsertCustomer(customer);

                    return assistant;
                }
            }
            return null;
        }

        public void UpdateAssistantLocation(Assistant assistant, Geolocation assistantLocation)
        {
            assistant.CurrentLocation = assistantLocation;
            _assistantService.UpsertAssistant(assistant);
        }
    }
}
