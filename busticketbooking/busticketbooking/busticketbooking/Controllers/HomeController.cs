using Microsoft.AspNetCore.Mvc;
using busticketbooking.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using busticketbooking.Data;

namespace busticketbooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult TestDatabase()
        {
            try
            {
                // Attempt to query the database
                var passengers = _context.Passengers.ToList();
                return Content($"Database connection successful. Found {passengers.Count} passengers.");
            }
            catch (Exception ex)
            {
                return Content($"Database connection failed: {ex.Message}");
            }
        }
        public IActionResult Error(int? statusCode = null)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode,
                ErrorMessage = statusCode switch
                {
                    404 => "The page you are looking for was not found.",
                    500 => "An unexpected error occurred. Please try again later.",
                    _ => "An error occurred. Please contact support."
                }
            };

            return View(errorViewModel);
        }
    }
}

