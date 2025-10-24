using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        // Navigation property
        public ICollection<Booking> Bookings { get; set; }
    }
}
