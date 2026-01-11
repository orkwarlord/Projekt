using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;
using Projekt.DTO;

namespace Projekt.Controllers
{
    [Authorize]
    public class RentingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RentingsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //domyslna strona wypozyczen = profil uzytkownika (historia wypozyczen)
        // GET: /Rentings
        public IActionResult Index()
        {
            return RedirectToAction(nameof(My));
        }

        // USER: moje wypożyczenia (profil)
        // GET: Rentings/My
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var rentings = await _context.Rentings
                .Where(r => r.AppUserId == userId)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Category)
                .OrderByDescending(r => r.RentedAt)
                .ToListAsync();

            return View(rentings);

        // USER: wypożycz książkę
        // POST: Rentings/Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists) return NotFound();

            var alreadyRented = await _context.Rentings
                .AnyAsync(r => r.BookId == bookId && r.ReturnedAt == null);

            if (alreadyRented)
            {
                TempData["Error"] = "Ta książka jest aktualnie niedostępna.";
                return RedirectToAction("Index", "Books");
            }

            _context.Rentings.Add(new Renting
            {
                BookId = bookId,
                AppUserId = userId,
                RentedAt = DateTime.UtcNow,
                ReturnedAt = null
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Wypożyczono książkę.";
            return RedirectToAction("Index", "Books");
        }

        // USER: oddaj książkę
        // POST: Rentings/Return
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int rentingId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var renting = await _context.Rentings
                .FirstOrDefaultAsync(r => r.Id == rentingId);

            if (renting == null) return NotFound();

            if (renting.ReturnedAt != null)
            {
                TempData["Error"] = "To wypożyczenie jest już zakończone (książka była oddana).";
                return RedirectToAction(nameof(My));
            }
            Renting renting = new Renting()
            {
                Id = rentingDTO.Id,
                BookId = rentingDTO.BookId,
                AppUserId = GetUserId(),
                RentedAt = rentingDTO.RentedAt,
                ReturnedAt = rentingDTO.ReturnedAt

            //bez ról: user moze oddac tylko swoje wypozyczenie
            if (renting.AppUserId != userId)
                return Forbid();

            renting.ReturnedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Zwrócono książkę.";
            return RedirectToAction(nameof(My));
        }

        //details dla zalogowanego usera
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var renting = await _context.Rentings
                .Include(r => r.Book).ThenInclude(b => b.Category)
                .FirstOrDefaultAsync(r => r.Id == id && r.AppUserId == userId);

            if (renting == null) return NotFound();

            return View(renting);
        }
    }
}
