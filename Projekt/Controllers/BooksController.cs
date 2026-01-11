using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Books (wszyscy) + filtrowanie
        public async Task<IActionResult> Index(string? author, int? categoryId)
        {
            var q = _context.Books
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(author))
                q = q.Where(b => b.Author.Contains(author));

            if (categoryId.HasValue)
                q = q.Where(b => b.CategoryId == categoryId.Value);

            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);
            ViewData["Author"] = author;

            return View(await q.ToListAsync());
        }

        // GET: Books/Details/5 (wszyscy)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id.Value);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Create (tylko Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(new Book());
        }

        // POST: Books/Create (tylko Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [Bind("Title,Author,Description,CategoryId")] Book book,
            IFormFile? coverImage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", book.CategoryId);
                return View(book);
            }

            // Upload okładki
            if (coverImage != null && coverImage.Length > 0)
            {
                var ext = Path.GetExtension(coverImage.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "Dozwolone formaty okładki: jpg, jpeg, png, webp.");
                    ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", book.CategoryId);
                    return View(book);
                }

                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(stream);
                }

                book.CoverImagePath = $"uploads/{fileName}";
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Dodano książkę.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5 (tylko Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id.Value);
            if (book == null) return NotFound();

            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5 (tylko Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description,CategoryId")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", book.CategoryId);
                return View(book);
            }

            var dbBook = await _context.Books.FindAsync(id);
            if (dbBook == null) return NotFound();

            dbBook.Title = book.Title;
            dbBook.Author = book.Author;
            dbBook.Description = book.Description;
            dbBook.CategoryId = book.CategoryId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Zapisano zmiany.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5 (tylko Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id.Value);

            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5 (tylko Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // blokuj usuwanie jeśli jest aktywne wypożyczenie
            var hasActiveRenting = await _context.Rentings
                .AnyAsync(r => r.BookId == id && r.ReturnedAt == null);

            if (hasActiveRenting)
            {
                ModelState.AddModelError("", "Nie można usunąć książki, bo jest aktualnie wypożyczona.");

                var bookWithCat = await _context.Books
                    .Include(b => b.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);

                return View("Delete", bookWithCat);
            }

            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usunięto książkę.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
