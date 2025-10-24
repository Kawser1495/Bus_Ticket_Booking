using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class Bus
    {
        [Key]
        public int BusID { get; set; }

        [Required]
        public string BusNumber { get; set; }

        [Required]
        public string BusType { get; set; } // AC/Non-AC, Sleeper/Seater

        [Required]
        public int TotalSeats { get; set; }

        // Navigation properties
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
    }
}

