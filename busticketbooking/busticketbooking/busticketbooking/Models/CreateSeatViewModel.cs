using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class CreateSeatViewModel
    {
        [Required]
        public int BusID { get; set; } // The ID of the bus to which the seat belongs

        [Required(ErrorMessage = "Seat type is required.")]
        public string SeatType { get; set; } // The type of the seat (e.g., Regular, Sleeper)
    }
}
