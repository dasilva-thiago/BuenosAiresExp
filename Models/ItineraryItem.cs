namespace BuenosAiresExp.Models
{
    public class ItineraryItem
    {
        public int Id { get; set; }

        public int ItineraryId { get; set; }
        public Itinerary Itinerary { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public int Order { get; set; }
    }
}