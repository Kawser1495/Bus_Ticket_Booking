using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class CreateBusViewModel
    {
        [Required(ErrorMessage = "Bus number is required.")]
        public string BusNumber { get; set; }

        [Required(ErrorMessage = "Bus type is required.")]
        public string BusType { get; set; }

        [Required(ErrorMessage = "Total seats are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be greater than 0.")]
        public int TotalSeats { get; set; }
    }

}
