using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace busticketbooking.Models
{
    public class Seat
    {
        [Key]
        public int SeatID { get; set; }

        [Required]
        [ForeignKey("Bus")]
        public int BusID { get; set; }
        public Bus Bus { get; set; }

        [Required]
        public string SeatNumber { get; set; }

        [Required]
        public string SeatType { get; set; } // Regular, Sleeper

        [Required]
        public bool IsBooked { get; set; }
    }
}

