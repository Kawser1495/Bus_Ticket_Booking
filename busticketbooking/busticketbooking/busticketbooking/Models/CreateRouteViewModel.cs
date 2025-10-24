using System;
using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class CreateRouteViewModel
    {
        [Required(ErrorMessage = "Source is required.")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        public TimeSpan Duration { get; set; }

        [Required(ErrorMessage = "Distance is required.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Distance must be greater than 0.")]
        public double Distance { get; set; }
    }
}
