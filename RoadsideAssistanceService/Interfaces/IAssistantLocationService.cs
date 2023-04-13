using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Interfaces
{
    public interface IAssistantLocationService
    {
        void UpdateAssistantLocation(Assistant assistant);
        IEnumerable<KeyValuePair<int, double>> FindNearestAssistants(Geolocation geolocation, int limit);
        void ClearAssistantLocations();
    }
}
