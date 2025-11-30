using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuCungNew.Data;
using WebThuCungNew.Models;

namespace WebThuCungNew.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PetServiceContext _context;

        public AdminController(PetServiceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Manage Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            ModelState.Remove("Pets");
            ModelState.Remove("ClientBookings");
            ModelState.Remove("SitterBookings");

            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(user);
                }

                user.CreatedAt = DateTime.Now;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm người dùng thành công!";
                return RedirectToAction(nameof(Users));
            }
            return View(user);
        }

        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            ModelState.Remove("Pets");
            ModelState.Remove("ClientBookings");
            ModelState.Remove("SitterBookings");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Users));
            }
            return View(user);
        }

        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null) return NotFound();
            
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Don't allow deleting yourself
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            if (user.UserId == currentUserId)
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản của chính bạn!";
                return RedirectToAction(nameof(Users));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            return RedirectToAction(nameof(Users));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // Manage Services
        public async Task<IActionResult> Services()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Services));
            }
            return View(service);
        }

        public async Task<IActionResult> EditService(int? id)
        {
            if (id == null) return NotFound();
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(int id, Service service)
        {
            if (id != service.ServiceId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Services));
            }
            return View(service);
        }

        public async Task<IActionResult> DeleteService(int? id)
        {
            if (id == null) return NotFound();
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();
            
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Services));
        }

        // Manage Bookings
        public async Task<IActionResult> Bookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Sitter)
                .Include(b => b.Service)
                .Include(b => b.Pet)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
            
            ViewBag.Sitters = await _context.Users.Where(u => u.Role == "Sitter").ToListAsync();
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> AssignSitter(int bookingId, int sitterId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.SitterId = sitterId;
                booking.Status = "Confirmed";
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Bookings));
        }

        // Reports
        public async Task<IActionResult> Reports()
        {
            // Simple report: Total bookings this month
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var bookings = await _context.Bookings
                .Where(b => b.BookingDate >= startOfMonth)
                .ToListAsync();

            ViewBag.TotalBookings = bookings.Count();
            ViewBag.TotalRevenue = bookings.Sum(b => b.TotalPrice);
            
            return View();
        }
    }
}
