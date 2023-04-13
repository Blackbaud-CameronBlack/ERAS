namespace EmergencyRoadsideAssistanceService.Models
{
    public class AssistantLocation
    {
        public int AssistantId { get; set; }
        public Geolocation CurrentLocation { get; set; } = new Geolocation();
        public bool CurrentlyEngaged { get; set; }        
    }
}
