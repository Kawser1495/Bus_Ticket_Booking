using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

