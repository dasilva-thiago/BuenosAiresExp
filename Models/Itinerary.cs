using System;
using System.Collections.Generic;
using System.Text;

namespace BuenosAiresExp.Models
{
        public class Itinerary
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime Date { get; set; } = DateTime.Today;
            public string? Notes { get; set; }
            public List<ItineraryItem> Items { get; set; } = new();
        
    }
}
