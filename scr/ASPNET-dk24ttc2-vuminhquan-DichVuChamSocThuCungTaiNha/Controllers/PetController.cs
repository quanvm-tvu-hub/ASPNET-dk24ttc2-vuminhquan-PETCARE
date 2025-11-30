using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebThuCungNew.Data;
using WebThuCungNew.Models;

namespace WebThuCungNew.Controllers
{
    [Authorize]
    public class PetController : Controller
    {
        private readonly PetServiceContext _context;

        public PetController(PetServiceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var pets = await _context.Pets.Where(p => p.OwnerId == userId).ToListAsync();
            return View(pets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pet pet)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            pet.OwnerId = userId;

            // Remove Owner from ModelState validation
            ModelState.Remove("Owner");

            if (ModelState.IsValid)
            {
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirst("UserId").Value);
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == id && p.OwnerId == userId);
            
            if (pet == null) return NotFound();
            return View(pet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Pet pet)
        {
            if (id != pet.PetId) return NotFound();

            var userId = int.Parse(User.FindFirst("UserId").Value);
            pet.OwnerId = userId;

            // Remove Owner from ModelState validation
            ModelState.Remove("Owner");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.PetId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirst("UserId").Value);
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == id && p.OwnerId == userId);

            if (pet == null) return NotFound();

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetId == id);
        }
    }
}
