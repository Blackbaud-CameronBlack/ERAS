namespace EmergencyRoadsideAssistanceService.Models
{
    public class Assistant : IComparable<Assistant>
    {
        public int AssistantId { get; set; }
        public string AssistantName { get; set; } = string.Empty;
        public Geolocation CurrentLocation { get; set; } = new Geolocation();
        public bool CurrentlyEngaged { get; set; } = false;
        public double DistanceFromCustomer { get; set; }
        public int CompareTo(Assistant? other)
        {
            if (other == null) return 1;
            return this.AssistantId.CompareTo(other.AssistantId);
        }
        public override string ToString()
        {
            return $"{AssistantId}, {AssistantName}, {CurrentLocation.LocationName}";
        }
    }
}
