using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebThuCungNew.Data;
using WebThuCungNew.Models;

namespace WebThuCungNew.Controllers
{
    public class BookingController : Controller
    {
        private readonly PetServiceContext _context;

        public BookingController(PetServiceContext context)
        {
            _context = context;
        }

        // API endpoint to get services (for AJAX)
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var services = await _context.Services
                    .Select(s => new
                    {
                        serviceId = s.ServiceId,
                        serviceName = s.ServiceName,
                        description = s.Description,
                        price = s.Price,
                        imageUrl = s.ImageUrl ?? ""
                    })
                    .ToListAsync();
                
                return Json(services);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // List Services to book (for logged in users)
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return NotFound();

            ViewBag.Service = service;
            
            // If user is logged in, get their pets
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst("UserId").Value);
                var pets = await _context.Pets.Where(p => p.OwnerId == userId).ToListAsync();
                ViewBag.Pets = pets;
                ViewBag.IsAuthenticated = true;
            }
            else
            {
                ViewBag.IsAuthenticated = false;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(Booking booking, string guestName, string guestEmail, string guestPhone, string petName, string petType)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst("UserId").Value);
                booking.ClientId = userId;
            }
            else
            {
                // Guest booking
                booking.GuestName = guestName;
                booking.GuestEmail = guestEmail;
                booking.GuestPhone = guestPhone;
                booking.PetName = petName;
                booking.PetType = petType;
            }

            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            // Calculate price
            var service = await _context.Services.FindAsync(booking.ServiceId);
            if (service != null)
            {
                booking.TotalPrice = service.Price;
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.";
            return RedirectToAction("BookingSuccess", new { id = booking.BookingId });
        }

        public async Task<IActionResult> BookingSuccess(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Pet)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [Authorize]
        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var bookings = await _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Pet)
                .Where(b => b.ClientId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id && b.ClientId == userId);

            if (booking != null && booking.Status == "Pending")
            {
                booking.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(History));
        }
    }
}
