using System;
using System.ComponentModel.DataAnnotations;

namespace busticketbooking.Models
{
    public class ScheduleViewModel
    {
        public int ScheduleID { get; set; }

        [Required(ErrorMessage = "Bus is required.")]
        public int BusID { get; set; }

        [Required(ErrorMessage = "Route is required.")]
        public int RouteID { get; set; }

        [Required(ErrorMessage = "Departure time is required.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required.")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }
    }
}
