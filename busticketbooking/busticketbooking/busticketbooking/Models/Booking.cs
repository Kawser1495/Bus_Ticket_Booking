using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using busticketbooking.Models;

namespace busticketbooking.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        [ForeignKey("Passenger")]
        public int PassengerID { get; set; }
        public Passenger Passenger { get; set; }

        [Required]
        [ForeignKey("Schedule")]
        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public decimal TotalFare { get; set; }

        [Required]
        public string Status { get; set; } 

        // Navigation property
        public Payment Payment { get; set; }
    }
}

