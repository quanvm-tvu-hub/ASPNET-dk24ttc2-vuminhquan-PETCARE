using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebThuCungNew.Models;
using WebThuCungNew.Data;

namespace WebThuCungNew.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PetServiceContext _context;

    public HomeController(ILogger<HomeController> logger, PetServiceContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Test()
    {
        return View();
    }

    // Action to seed users manually
    public IActionResult SeedUsers()
    {
        if (!_context.Users.Any())
        {
            var users = new User[]
            {
                new User
                {
                    Email = "admin@petservice.com",
                    Password = "admin123",
                    FullName = "System Admin",
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Email = "sitter1@petservice.com",
                    Password = "sitter123",
                    FullName = "John Sitter",
                    Role = "Sitter",
                    Phone = "0909000001",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Email = "client1@petservice.com",
                    Password = "client123",
                    FullName = "Alice Client",
                    Role = "Client",
                    Phone = "0909000002",
                    CreatedAt = DateTime.Now
                }
            };

            foreach (User u in users)
            {
                _context.Users.Add(u);
            }
            _context.SaveChanges();

            return Content("Users seeded successfully! You can now login with:\n\n" +
                "Admin: admin@petservice.com / admin123\n" +
                "Sitter: sitter1@petservice.com / sitter123\n" +
                "Client: client1@petservice.com / client123");
        }

        return Content("Users already exist in database!");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
