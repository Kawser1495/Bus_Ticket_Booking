using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class BusRoute
    {
        [Key]
        public int RouteID { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public double Distance { get; set; }

        // Navigation property
        public ICollection<Schedule> Schedules { get; set; }
    }
}

