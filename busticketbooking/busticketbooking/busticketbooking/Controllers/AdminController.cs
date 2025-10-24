using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using busticketbooking.Data;
using busticketbooking.Models;
using System.Linq;
using System.Threading.Tasks;
using ModelsRoute = busticketbooking.Models.BusRoute;


namespace busticketbooking.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: Admin Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and Password are required.");
                return View();
            }

            // Force client-side evaluation
            var admin = _context.Admins
                .AsEnumerable()
                .FirstOrDefault(a => string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase));

            if (admin == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View();
            }

            // Verify the password
            if (admin.Password != password) // Note: Replace this with hashed password verification in production
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View();
            }

            // Set session value for AdminID
            HttpContext.Session.SetInt32("AdminID", admin.AdminID);

            // Redirect to the Admin Panel
            return RedirectToAction("Dashboard");
        }



        // Admin Logout
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToAction("Login");
        }
        public IActionResult Profile()
        {
            var adminId = HttpContext.Session.GetInt32("AdminID");

            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var admin = _context.Admins.FirstOrDefault(a => a.AdminID == adminId);

            if (admin == null)
            {
                return NotFound("Admin not found.");
            }

            return View(admin);
        }

        public IActionResult Dashboard()
        {
            var adminId = HttpContext.Session.GetInt32("AdminID");

            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            // Pass admin-specific data to the view if needed
            ViewData["AdminName"] = _context.Admins.FirstOrDefault(a => a.AdminID == adminId)?.Name;

            return View();
        }

        public async Task<IActionResult> ManageRoutes()
        {
            var routes = await _context.Routes.ToListAsync();
            return View(routes);
        }

        [HttpGet]
        public IActionResult CreateRoute()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CreateRoute(CreateRouteViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map the view model to the BusRoute entity
                var route = new BusRoute
                {
                    Source = model.Source,
                    Destination = model.Destination,
                    Duration = model.Duration,
                    Distance = model.Distance
                };

                _context.Routes.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageRoutes));
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
            {
                return NotFound("Route not found.");
            }

            // Map the BusRoute entity to CreateRouteViewModel
            var viewModel = new CreateRouteViewModel
            {
                Source = route.Source,
                Destination = route.Destination,
                Duration = route.Duration,
                Distance = route.Distance
            };

            ViewData["RouteID"] = id; // Pass the RouteID to the view
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoute(int id, CreateRouteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingRoute = await _context.Routes.FindAsync(id);
                if (existingRoute == null)
                {
                    return NotFound("Route not found.");
                }

                // Update the BusRoute entity with data from the view model
                existingRoute.Source = model.Source;
                existingRoute.Destination = model.Destination;
                existingRoute.Duration = model.Duration;
                existingRoute.Distance = model.Distance;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageRoutes));
            }

            ViewData["RouteID"] = id; // Ensure RouteID is passed back to the view
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route != null)
            {
                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageRoutes));
        }



        // Manage Schedules
        public async Task<IActionResult> ManageSchedules()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .ToListAsync();
            return View(schedules);
        }
        [HttpGet]
        public IActionResult CreateSchedule()
        {
            // Populate dropdowns for buses and routes
            ViewBag.Buses = _context.Buses.Select(b => new { b.BusID, b.BusNumber }).ToList();
            ViewBag.Routes = _context.Routes.Select(r => new { r.RouteID, r.Source, r.Destination }).ToList();

            return View(new ScheduleViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> CreateSchedule(ScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var schedule = new Schedule
                {
                    BusID = model.BusID,
                    RouteID = model.RouteID,
                    DepartureTime = model.DepartureTime,
                    ArrivalTime = model.ArrivalTime,
                    Date = model.Date
                };

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageSchedules));
            }

            // Repopulate dropdowns if validation fails
            ViewBag.Buses = _context.Buses.Select(b => new { b.BusID, b.BusNumber }).ToList();
            ViewBag.Routes = _context.Routes.Select(r => new { r.RouteID, r.Source, r.Destination }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditSchedule(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.ScheduleID == id);

            if (schedule == null)
            {
                return NotFound("Schedule not found.");
            }

            // Map the Schedule entity to ScheduleViewModel
            var viewModel = new ScheduleViewModel
            {
                ScheduleID = schedule.ScheduleID,
                BusID = schedule.BusID,
                RouteID = schedule.RouteID,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                Date = schedule.Date
            };

            // Populate dropdowns
            ViewBag.Buses = _context.Buses.Select(b => new { b.BusID, b.BusNumber }).ToList();
            ViewBag.Routes = _context.Routes.Select(r => new { r.RouteID, r.Source, r.Destination }).ToList();

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule(ScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingSchedule = await _context.Schedules.FindAsync(model.ScheduleID);
                if (existingSchedule == null)
                {
                    return NotFound("Schedule not found.");
                }

                // Update the Schedule entity with data from the view model
                existingSchedule.BusID = model.BusID;
                existingSchedule.RouteID = model.RouteID;
                existingSchedule.DepartureTime = model.DepartureTime;
                existingSchedule.ArrivalTime = model.ArrivalTime;
                existingSchedule.Date = model.Date;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageSchedules));
            }

            // Repopulate dropdowns if validation fails
            ViewBag.Buses = _context.Buses.Select(b => new { b.BusID, b.BusNumber }).ToList();
            ViewBag.Routes = _context.Routes.Select(r => new { r.RouteID, r.Source, r.Destination }).ToList();

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageSchedules));
        }



        // Manage Buses
        public async Task<IActionResult> ManageBuses()
        {
            var buses = await _context.Buses.ToListAsync();
            return View(buses);
        }

        // Manage Seat Layouts
        public async Task<IActionResult> ManageSeats(int busId)
        {
            var seats = await _context.Seats
                .Where(s => s.BusID == busId)
                .ToListAsync();
            return View(seats);
        }

        public IActionResult ManageBookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.Passenger) // Include Passenger details
                .Include(b => b.Schedule) // Include Schedule details
                .ThenInclude(s => s.Route) // Include Route details within Schedule
                .ToList();

            return View(bookings);
        }


        // Monitor Transactions
        public async Task<IActionResult> MonitorTransactions()
        {
            var payments = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Passenger)
                .ToListAsync();
            return View(payments);
        }

        public async Task<IActionResult> GenerateReports()
        {
            // Fetch booking data
            var bookings = await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Schedule)
                .ThenInclude(s => s.Bus)
                .ToListAsync();

            // Fetch payment data
            var payments = await _context.Payments
                .Include(p => p.Booking)
                .ToListAsync();

            // Pass data to the view
            ViewBag.Bookings = bookings;
            ViewBag.Payments = payments;

            return View();
        }


        // Process Refunds
        public async Task<IActionResult> ProcessRefund(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.BookingID == bookingId);

            if (booking == null || booking.Status == "Cancelled")
            {
                return NotFound("Booking not found or already cancelled.");
            }

            // Mark the booking as cancelled
            booking.Status = "Cancelled";

            // Process refund (update payment status)
            if (booking.Payment != null)
            {
                booking.Payment.Status = "Refunded";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Refund processed successfully.";
            return RedirectToAction(nameof(ManageBookings));
        }
        // Add Bus
        [HttpGet]
        public IActionResult CreateBus()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBus(CreateBusViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bus = new Bus
                {
                    BusNumber = model.BusNumber,
                    BusType = model.BusType,
                    TotalSeats = model.TotalSeats
                };

                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageBuses));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound("Bus not found.");
            }

            // Map the Bus entity to CreateBusViewModel
            var viewModel = new CreateBusViewModel
            {
                BusNumber = bus.BusNumber,
                BusType = bus.BusType,
                TotalSeats = bus.TotalSeats
            };

            ViewData["BusID"] = id; // Pass the BusID to the view
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditBus(int id, CreateBusViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingBus = await _context.Buses.FindAsync(id);
                if (existingBus == null)
                {
                    return NotFound("Bus not found.");
                }

                // Update the bus details
                existingBus.BusNumber = model.BusNumber;
                existingBus.BusType = model.BusType;
                existingBus.TotalSeats = model.TotalSeats;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageBuses));
            }

            ViewData["BusID"] = id; // Ensure BusID is passed back to the view
            return View(model);
        }




        // Delete Bus
        [HttpPost]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus != null)
            {
                _context.Buses.Remove(bus);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageBuses));
        }

        [HttpGet]
        public IActionResult CreateSeat(int busId)
        {
            if (busId <= 0)
            {
                return BadRequest("Invalid Bus ID.");
            }

            var bus = _context.Buses.FirstOrDefault(b => b.BusID == busId);
            if (bus == null)
            {
                return NotFound("Bus not found.");
            }

            // Populate seat types based on bus type
            var seatTypes = bus.BusType == "AC" ? new[] {  "Sleeper" , "Regular" } : new[] { "Regular" };

            var viewModel = new CreateSeatViewModel
            {
                BusID = busId
            };

            ViewBag.SeatTypes = seatTypes;
            return View(viewModel);
        }





        [HttpPost]
        public async Task<IActionResult> CreateSeat(CreateSeatViewModel model)
        {
            Console.WriteLine($"Received BusID: {model.BusID}");

            if (ModelState.IsValid)
            {
                var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusID == model.BusID);
                if (bus == null)
                {
                    Console.WriteLine("Bus not found in the database.");
                    return NotFound("Bus not found.");
                }

                Console.WriteLine($"Bus found: {bus.BusNumber}, Total Seats: {bus.TotalSeats}");

                var existingSeats = _context.Seats.Where(s => s.BusID == model.BusID).ToList();
                int totalSeats = bus.TotalSeats;
                int currentSeatCount = existingSeats.Count;

                Console.WriteLine($"Existing Seats: {currentSeatCount}, Total Seats: {totalSeats}");

                int columnsPerRow = 4;
                var newSeats = new List<Seat>();

                for (int i = currentSeatCount; i < totalSeats; i++)
                {
                    int rowNumber = i / columnsPerRow;
                    int columnNumber = (i % columnsPerRow) + 1;
                    char rowLetter = (char)('A' + rowNumber);
                    string seatNumber = $"{rowLetter}{columnNumber}";

                    Console.WriteLine($"Generating Seat: {seatNumber}");

                    newSeats.Add(new Seat
                    {
                        BusID = model.BusID,
                        SeatNumber = seatNumber,
                        SeatType = model.SeatType,
                        IsBooked = false
                    });
                }

                _context.Seats.AddRange(newSeats);
                await _context.SaveChangesAsync();

                Console.WriteLine("Seats successfully created.");
                return RedirectToAction(nameof(ManageSeats), new { busId = model.BusID });
            }

            Console.WriteLine("Model validation failed.");
            var busDetails = _context.Buses.FirstOrDefault(b => b.BusID == model.BusID);
            if (busDetails != null)
            {
                ViewBag.SeatTypes = busDetails.BusType == "AC" ? new[] { "Regular", "Sleeper" } : new[] { "Regular" };
            }

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> EditSeat(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null)
            {
                return NotFound("Seat not found.");
            }

            // Map the Seat entity to SeatViewModel
            var viewModel = new SeatViewModel
            {
                SeatID = seat.SeatID,
                BusID = seat.BusID,
                SeatType = seat.SeatType,
                IsBooked = seat.IsBooked
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSeat(SeatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingSeat = await _context.Seats.FindAsync(model.SeatID);
                if (existingSeat == null)
                {
                    return NotFound("Seat not found.");
                }

                // Update the Seat entity with data from the view model
                existingSeat.SeatType = model.SeatType;
                existingSeat.IsBooked = model.IsBooked;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageSeats), new { busId = model.BusID });
            }

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat != null)
            {
                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageSeats), new { busId = seat.BusID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllSeats(int busId)
        {
            var seats = _context.Seats.Where(s => s.BusID == busId);
            if (seats.Any())
            {
                _context.Seats.RemoveRange(seats);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageSeats), new { busId });
        }

    }
}

