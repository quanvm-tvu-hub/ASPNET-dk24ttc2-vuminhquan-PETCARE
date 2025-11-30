using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebThuCungNew.Data;
using WebThuCungNew.Models;

namespace WebThuCungNew.Controllers
{
    [Authorize(Roles = "Sitter")]
    public class SitterController : Controller
    {
        private readonly PetServiceContext _context;

        public SitterController(PetServiceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var bookings = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Service)
                .Include(b => b.Pet)
                .Where(b => b.SitterId == userId)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int bookingId, string status)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId && b.SitterId == userId);

            if (booking != null)
            {
                if (status == "Confirmed" || status == "Completed")
                {
                    booking.Status = status;
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
