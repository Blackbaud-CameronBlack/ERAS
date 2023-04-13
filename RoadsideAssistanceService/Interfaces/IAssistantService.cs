using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Interfaces
{
    public interface IAssistantService
    {
        Assistant? GetAssistant(int assistantId);
        void UpsertAssistant(Assistant assistant);
        void ClearAssistants();
        void UpdateAssistantLocation(Assistant assistant);
        SortedSet<Assistant> FindNearestAssistants(Geolocation geolocation, int limit);
    }
}
