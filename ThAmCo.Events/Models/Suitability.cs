namespace ThAmCo.Events.Models
{
    public class Suitability
    {
        public string EventTypeId { get; set; }

        public EventType EventType { get; set; }

        public string VenueCode { get; set; }

        public Venue Venue { get; set; }
    }
}