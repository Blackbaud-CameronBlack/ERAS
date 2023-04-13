namespace EmergencyRoadsideAssistanceService.Models
{
    public class Customer
    {
        public int CustomerId { get; set; } 
        public string CustomerName { get; set; } = string.Empty;
        public Geolocation CurrentLocation { get; set; } = new Geolocation();
        public bool CurrentlyEngaged { get; set; } = false;
    }
}
