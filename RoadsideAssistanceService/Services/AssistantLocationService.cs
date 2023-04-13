using EmergencyRoadsideAssistanceService.Interfaces;
using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Services
{
    public class AssistantLocationService : IAssistantLocationService
    {
        private readonly Dictionary<int, AssistantLocation> _assistantLocations = new();

        public IEnumerable<KeyValuePair<int, double>> FindNearestAssistants(Geolocation geolocation, int limit)
        {
            // This would be handled in a geospatial indexed database.
            // This is throwaway code to mock out getting the nearest assistants to a location.
            var closestAssistants = new PriorityQueue<int, double>();
            foreach (var assistantLocationKvp in _assistantLocations)
            {
                if (!assistantLocationKvp.Value.CurrentlyEngaged)
                {
                    closestAssistants.Enqueue(assistantLocationKvp.Key, 
                        Geolocation.CalculateDistance(geolocation, 
                        assistantLocationKvp.Value.CurrentLocation));
                }
            }

            // Grab the top <limit> results of closest assistants and add them to the results
            var results = new Dictionary<int, double>();
            var items = closestAssistants.Count;
            if (closestAssistants.Count >= limit) items = limit;

            for (int i = 0; i < items; i++)
            {
                if (closestAssistants.TryDequeue(out int assistantId, out double distance))
                {
                    //results.Add(assistantId, distance);
                    yield return new KeyValuePair<int, double>(assistantId, distance);
                }                    
            }
        }

        public void UpdateAssistantLocation(Assistant assistant)
        {
            // This would be handled in a geospatial indexed database.
            // This is throwaway code to mock out updating the location and engagement for an assistant.
            var newAssistantLocation = new AssistantLocation
            {
                AssistantId = assistant.AssistantId,
                CurrentLocation = assistant.CurrentLocation,
                CurrentlyEngaged = assistant.CurrentlyEngaged
            };

            if (!_assistantLocations.ContainsKey(assistant.AssistantId))
            {
                _assistantLocations.Add(assistant.AssistantId, newAssistantLocation);
            }
            else
            {
                _assistantLocations[assistant.AssistantId] = newAssistantLocation;
            }
        }

        public void ClearAssistantLocations()
        {
            _assistantLocations.Clear();
        }
    }
}
