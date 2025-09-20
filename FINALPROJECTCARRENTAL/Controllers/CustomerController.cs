using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.Models.ViewModels;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public CustomerController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Check if user is logged in
        private bool IsLoggedIn()
        {
            var userId = HttpContext.Session.GetString("UserID");
            return !string.IsNullOrEmpty(userId);
        }

        // Get current user ID
        private int GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserID");
            return int.TryParse(userIdString, out int userId) ? userId : 0;
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // View available cars
        public async Task<IActionResult> Cars()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var cars = await _context.Cars
                .Where(c => c.IsAvailable)
                .ToListAsync();

            return View(cars);
        }

        // Book a car
        [HttpGet]
        public async Task<IActionResult> BookCar(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null || !car.IsAvailable)
            {
                TempData["ErrorMessage"] = "Car not available for booking.";
                return RedirectToAction("Cars");
            }

            var viewModel = new BookingViewModel
            {
                CarID = car.CarID,
                CarName = car.CarName,
                CarModel = car.CarModel,
                ImageUrl = car.ImageUrl,
                PickupDate = DateTime.Today,
                ReturnDate = DateTime.Today.AddDays(1)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookCar(BookingViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Validate dates
                if (model.PickupDate < DateTime.Today)
                {
                    ModelState.AddModelError("PickupDate", "Pickup date cannot be in the past.");
                    return View(model);
                }

                if (model.ReturnDate <= model.PickupDate)
                {
                    ModelState.AddModelError("ReturnDate", "Return date must be after pickup date.");
                    return View(model);
                }

                // Check if car is still available
                var car = await _context.Cars.FindAsync(model.CarID);
                if (car == null || !car.IsAvailable)
                {
                    TempData["ErrorMessage"] = "Car is no longer available for booking.";
                    return RedirectToAction("Cars");
                }

                // Check for overlapping bookings
                var overlappingBookings = await _context.Bookings
                    .Where(b => b.CarID == model.CarID &&
                               ((b.PickupDate <= model.PickupDate && b.ReturnDate > model.PickupDate) ||
                                (b.PickupDate < model.ReturnDate && b.ReturnDate >= model.ReturnDate) ||
                                (b.PickupDate >= model.PickupDate && b.ReturnDate <= model.ReturnDate)))
                    .AnyAsync();

                if (overlappingBookings)
                {
                    ModelState.AddModelError("", "Car is not available for the selected dates.");
                    return View(model);
                }

                // Calculate total cost (assuming $50 per day)
                var days = (model.ReturnDate - model.PickupDate).Days;
                var totalCost = days * 50;

                // Create booking
                var booking = new Booking
                {
                    CustomerID = GetCurrentUserId(),
                    CarID = model.CarID,
                    PickupDate = model.PickupDate,
                    ReturnDate = model.ReturnDate,
                    TotalCost = totalCost
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // send email notification
                var carDisplay = $"{car.CarName} {car.CarModel}".Trim();
                var emailSubject = $"New Booking: {carDisplay} ({model.PickupDate:yyyy-MM-dd} to {model.ReturnDate:yyyy-MM-dd})";
                var body = $@"<h3>New Car Booking</h3>
                              <p><strong>User ID:</strong> {GetCurrentUserId()}</p>
                              <p><strong>Car:</strong> {carDisplay}</p>
                              <p><strong>Pickup:</strong> {model.PickupDate:yyyy-MM-dd}</p>
                              <p><strong>Return:</strong> {model.ReturnDate:yyyy-MM-dd}</p>
                              <p><strong>Total Cost:</strong> ${totalCost}</p>";
                await _emailService.SendDefaultAsync(emailSubject, body);

                TempData["SuccessMessage"] = $"Car booked successfully! Total cost: ${totalCost}";
                return RedirectToAction("MyBookings");
            }

            return View(model);
        }

        // View customer's bookings
        public async Task<IActionResult> MyBookings()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.CustomerID == userId)
                .OrderByDescending(b => b.PickupDate)
                .ToListAsync();

            return View(bookings);
        }

        // Cancel a booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingID == id && b.CustomerID == userId);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking cancelled successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Booking not found or you don't have permission to cancel it.";
            }

            return RedirectToAction("MyBookings");
        }
    }
}