using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    public class RentingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rentings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Rentings.Include(r => r.AppUser).Include(r => r.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Rentings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings
                .Include(r => r.AppUser)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        // GET: Rentings/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id");
            return View();
        }

        // POST: Rentings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,AppUserId,RentedAt,ReturnedAt")] Renting renting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(renting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", renting.AppUserId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id", renting.BookId);
            return View(renting);
        }

        // GET: Rentings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings.FindAsync(id);
            if (renting == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", renting.AppUserId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id", renting.BookId);
            return View(renting);
        }

        // POST: Rentings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,AppUserId,RentedAt,ReturnedAt")] Renting renting)
        {
            if (id != renting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(renting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentingExists(renting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", renting.AppUserId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Id", renting.BookId);
            return View(renting);
        }

        // GET: Rentings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings
                .Include(r => r.AppUser)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        // POST: Rentings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var renting = await _context.Rentings.FindAsync(id);
            if (renting != null)
            {
                _context.Rentings.Remove(renting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentingExists(int id)
        {
            return _context.Rentings.Any(e => e.Id == id);
        }
    }
}
