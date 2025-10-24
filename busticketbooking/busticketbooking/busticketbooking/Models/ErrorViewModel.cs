using System;

namespace busticketbooking.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string ErrorMessage { get; set; }
        public int? StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

