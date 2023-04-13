using EmergencyRoadsideAssistanceService.Interfaces;
using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Services
{
    public class AssistantService : IAssistantService
    {
        private readonly Dictionary<int, Assistant> _assistants = new();
        private readonly IAssistantLocationService _assistantLocationService;

        public AssistantService(IAssistantLocationService locationService)
        {
            if (locationService == null) throw new ArgumentNullException(nameof(locationService));
            _assistantLocationService = locationService;
        }

        public Assistant? GetAssistant(int assistantId)
        {
            if (_assistants.ContainsKey(assistantId))
            {
                return _assistants[assistantId];
            }
            return null;
        }

        public void UpsertAssistant(Assistant assistant)
        {
            if (!_assistants.ContainsKey(assistant.AssistantId))
            {
                _assistants.Add(assistant.AssistantId, assistant);
            }
            else
            {
                _assistants[assistant.AssistantId] = assistant;
            }
            _assistantLocationService.UpdateAssistantLocation(assistant);
        }

        public void ClearAssistants()
        {
            _assistants.Clear();
            _assistantLocationService.ClearAssistantLocations();
        }

        public void UpdateAssistantLocation(Assistant assistant)
        {
            _assistantLocationService.UpdateAssistantLocation(assistant);
        }

        public SortedSet<Assistant> FindNearestAssistants(Geolocation geolocation, int limit)
        {
            // Using the custom comparer will allow us to have duplicate distances
            var results = new SortedSet<Assistant>(Comparer<Assistant>.Create((a, b) =>
            {
                int result = a!.DistanceFromCustomer.CompareTo(b!.DistanceFromCustomer);
                if (result == 0)
                    return 1;   // Handle equality as being greater than
                else
                    return result;
            }));

            foreach (var result in _assistantLocationService.FindNearestAssistants(geolocation, limit))
            {
                var assistant = GetAssistant(result.Key);
                if (assistant != null)
                {
                    assistant.DistanceFromCustomer = result.Value;
                    results.Add(assistant);
                }
            }
            return results;
        }
    }
}
