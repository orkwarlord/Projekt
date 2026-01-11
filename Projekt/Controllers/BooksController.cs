using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.DTO;
using Projekt.Models;

namespace Projekt.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books + filtrowanie
        public async Task<IActionResult> Index(string? author, int? categoryId)
        {
            var q = _context.Books
                .Include(b => b.Category)
                .Select(b => new BookDTO(b)).AsQueryable();

            if (!string.IsNullOrWhiteSpace(author))
                q = q.Where(b => b.Author.Contains(author));

            if (categoryId.HasValue)
                q = q.Where(b => b.CategoryId == categoryId);

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", categoryId);
            ViewData["Author"] = author;

            return View(await q.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(new BookDTO(book));
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Description,CategoryId")] BookDTO bookDTO)
        {
            Book book = new Book()
            {
                Id = bookDTO.Id,
                Author = bookDTO.Author,
                Title = bookDTO.Title,
                Description = bookDTO.Description,
                CategoryId = bookDTO.CategoryId,


            };
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description,CategoryId")] BookDTO bookDTO)
        {
            if (id != bookDTO.Id) return NotFound();
            Book book = new Book()
            {
                Id = bookDTO.Id,
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Description = bookDTO.Description,
                CategoryId = bookDTO.CategoryId,

            };
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // blokuj usuwanie TYLKO jeśli jest aktywne wypożyczenie
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
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
