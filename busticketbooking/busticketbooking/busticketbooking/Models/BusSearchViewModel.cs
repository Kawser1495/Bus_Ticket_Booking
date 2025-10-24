using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class BusSearchViewModel
    {
        [Required(ErrorMessage = "Route is required.")]
        public int RouteID { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        public List<Schedule> Buses { get; set; }

        public List<BusRoute> Routes { get; set; } // List of routes for the dropdown
    }
}

