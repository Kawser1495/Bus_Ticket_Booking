using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace busticketbooking.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }

        [Required]
        [ForeignKey("Bus")]
        public int BusID { get; set; }
        public Bus Bus { get; set; }

        [Required]
        [ForeignKey("Route")]
        public int RouteID { get; set; }
        public BusRoute Route { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation property
        public ICollection<Booking> Bookings { get; set; }
    }
}

