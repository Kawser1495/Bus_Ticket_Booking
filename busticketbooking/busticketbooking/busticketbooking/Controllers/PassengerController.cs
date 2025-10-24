using Microsoft.AspNetCore.Mvc;
using busticketbooking.Data;
using busticketbooking.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using busticketbooking.Models;


namespace busticketbooking.Controllers
{
    public class PassengerController : Controller
    {

        private readonly AppDbContext _context;

        public PassengerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.Passengers.Any(p => p.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                // Map RegisterModel to Passenger entity
                var passenger = new Passenger
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password // Note: Password should ideally be hashed
                };

                // Save to database
                _context.Passengers.Add(passenger);
                _context.SaveChanges();

                // Redirect to login page after successful registration
                return RedirectToAction("Login");
            }

            // If validation fails, redisplay the form
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and Password are required.");
                return View();
            }

            var passenger = _context.Passengers
                .FirstOrDefault(p => p.Email.ToLower() == email.ToLower() && p.Password == password);

            if (passenger == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                return View();
            }

            // Set session value for PassengerID
            HttpContext.Session.SetInt32("PassengerID", passenger.PassengerID);

            // Redirect to the Profile page after successful login
            return RedirectToAction("Dashboard");
        }

        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Login", "Passenger");
        }




        [HttpGet]
        public IActionResult Profile()
        {
            var passengerId = HttpContext.Session.GetInt32("PassengerID");

            if (passengerId == null)
            {
                return RedirectToAction("Login");
            }

            var passenger = _context.Passengers.Find(passengerId);
            return View(passenger);
        }


        [HttpGet]
        public IActionResult SearchBuses(BusSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var query = _context.Schedules
                    .Include(s => s.Bus)
                    .Include(s => s.Route)
                    .Where(s => s.RouteID == model.RouteID && s.Date.Date == model.Date.Date);

                var buses = query
                    .Select(s => new Schedule
                    {
                        ScheduleID = s.ScheduleID,
                        BusID = s.BusID,
                        Bus = new Bus
                        {
                            BusNumber = s.Bus.BusNumber,
                            BusType = s.Bus.BusType
                        },
                        DepartureTime = s.DepartureTime,
                        ArrivalTime = s.ArrivalTime,
                        Date = s.Date
                    })
                    .ToList();

                model.Buses = buses;
            }

            model.Routes = _context.Routes
                .Select(r => new BusRoute
                {
                    RouteID = r.RouteID,
                    Source = r.Source,
                    Destination = r.Destination
                })
                .ToList();

            return View("Dashboard", model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing passenger from the database
                var existingPassenger = await _context.Passengers.FindAsync(passenger.PassengerID);

                if (existingPassenger != null)
                {
                    // Update the passenger's information
                    existingPassenger.Name = passenger.Name;
                    existingPassenger.Email = passenger.Email;
                    existingPassenger.Phone = passenger.Phone;
                    existingPassenger.Password = passenger.Password; // Note: Hash the password in production

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Update the session value if needed
                    HttpContext.Session.SetInt32("PassengerID", existingPassenger.PassengerID);

                    // Redirect to the dashboard
                    return RedirectToAction("Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Passenger not found.");
            }

            // If validation fails, redisplay the profile form
            return View("Profile", passenger);
        }



        public IActionResult Dashboard()
        {
            var model = new BusSearchViewModel
            {
                Routes = _context.Routes
                    .Select(r => new BusRoute
                    {
                        RouteID = r.RouteID,
                        Source = r.Source,
                        Destination = r.Destination
                    })
                    .ToList()
            };

            return View(model);
        }




        [HttpGet]
        public IActionResult SelectSeats(int scheduleId)
        {
            // Fetch the bus ID for the given schedule
            var schedule = _context.Schedules
                .Include(s => s.Bus)
                .FirstOrDefault(s => s.ScheduleID == scheduleId);

            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            // Fetch all seats for the bus
            var availableSeats = _context.Seats
                .Where(s => s.BusID == schedule.BusID)
                .ToList();

            ViewBag.ScheduleID = scheduleId;
            return View(availableSeats);
        }


        [HttpPost]
        public async Task<IActionResult> BookTickets(int scheduleId, string selectedSeats, int passengerId)
        {
            if (string.IsNullOrWhiteSpace(selectedSeats))
            {
                ModelState.AddModelError(string.Empty, "No seats selected.");
                return RedirectToAction("SelectSeats", new { scheduleId });
            }

            // Split the selected seats into a list
            var seatNumbers = selectedSeats.Split(',');

            // Fetch the seats to be booked
            var seatsToBook = _context.Seats
                .Where(s => seatNumbers.Contains(s.SeatNumber) && !s.IsBooked)
                .ToList();

            if (!seatsToBook.Any())
            {
                ModelState.AddModelError(string.Empty, "Selected seats are no longer available.");
                return RedirectToAction("SelectSeats", new { scheduleId });
            }

            // Mark the seats as booked
            foreach (var seat in seatsToBook)
            {
                seat.IsBooked = true;
            }

            // Create a new booking
            var booking = new Booking
            {
                PassengerID = passengerId,
                ScheduleID = scheduleId,
                BookingDate = DateTime.Now,
                TotalFare = seatsToBook.Count * 500, // Example fare calculation
                Status = "Booked"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("MakePayment", new { bookingId = booking.BookingID });
        }


        [HttpGet]
        public IActionResult MakePayment(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                .FirstOrDefault(b => b.BookingID == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return View(booking);
        }


        [HttpPost]
        public async Task<IActionResult> MakePayment(int bookingId, string paymentMode)
        {
            var booking = _context.Bookings
                .Include(b => b.Schedule)
                .FirstOrDefault(b => b.BookingID == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Create a new payment record
            var payment = new Payment
            {
                BookingID = bookingId,
                PaymentDate = DateTime.Now,
                Amount = booking.TotalFare,
                PaymentMode = paymentMode,
                Status = "Success"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewTickets");
        }

        // GET: View Tickets
        public IActionResult ViewTickets(int passengerId)
        {
            var bookings = _context.Bookings
                .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                .Where(b => b.PassengerID == passengerId)
                .ToList();

            return View(bookings);
        }

        // POST: Cancel Ticket
        [HttpPost]
        public async Task<IActionResult> CancelTicket(int bookingId)
        {
            var booking = _context.Bookings.Include(b => b.Payment).FirstOrDefault(b => b.BookingID == bookingId);
            if (booking == null || booking.Status == "Cancelled")
            {
                return NotFound("Booking not found or already cancelled.");
            }

            booking.Status = "Cancelled";
            if (booking.Payment != null)
            {
                booking.Payment.Status = "Refunded";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ViewTickets", new { passengerId = booking.PassengerID });
        }



    }
}

