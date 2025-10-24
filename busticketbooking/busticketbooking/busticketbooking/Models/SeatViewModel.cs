using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class SeatViewModel
    {
        [Required]
        public int SeatID { get; set; }

        [Required]
        public int BusID { get; set; }

        [Required(ErrorMessage = "Seat type is required.")]
        public string SeatType { get; set; }

        [Required]
        public bool IsBooked { get; set; }
    }
}
