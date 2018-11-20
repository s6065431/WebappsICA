namespace ThAmCo.Events.Models
{
    public class Suitability
    {
        public string EventTypeId { get; set; }

        public EventTypesDto EventType { get; set; }

        public string VenueCode { get; set; }

        public VenuesDto Venue { get; set; }
    }
}