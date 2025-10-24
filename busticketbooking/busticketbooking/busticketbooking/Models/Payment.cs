using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace busticketbooking.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingID { get; set; }
        public Booking Booking { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMode { get; set; } // Card, UPI, Wallet

        [Required]
        public string Status { get; set; } // Success/Failed
    }
}

