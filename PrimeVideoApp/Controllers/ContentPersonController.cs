using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrimeVideoApp.Data;
using PrimeVideoApp.Models;

namespace PrimeVideoApp.Controllers
{
    public class ContentPersonController : Controller
    {
        private readonly PrimeVideoContext _context;

        public ContentPersonController(PrimeVideoContext context)
        {
            _context = context;
        }

        // GET: ContentPerson
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var primeVideoContext = _context.ContentPeople
                .Include(c => c.Content)
                .Include(c => c.Person)
                .OrderBy(c => c.ContentId)
                .ThenBy(c => c.PersonId)
                .ThenBy(c => c.Role);
            return View(await PaginatedList<ContentPerson>.CreateAsync(primeVideoContext, page, pageSize));
        }

        // GET: ContentPerson/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentPerson = await _context.ContentPeople
                .Include(c => c.Content)
                .Include(c => c.Person)
                .FirstOrDefaultAsync(m => m.ContentId == id);
            if (contentPerson == null)
            {
                return NotFound();
            }

            return View(contentPerson);
        }

        // GET: ContentPerson/Create
        public IActionResult Create()
        {
            ViewData["ContentId"] = new SelectList(_context.Contents, "Id", "Id");
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Id");
            return View();
        }

        // POST: ContentPerson/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContentId,PersonId,Role,CreditOrder")] ContentPerson contentPerson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contentPerson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContentId"] = new SelectList(_context.Contents, "Id", "Id", contentPerson.ContentId);
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Id", contentPerson.PersonId);
            return View(contentPerson);
        }

        // GET: ContentPerson/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentPerson = await _context.ContentPeople.FindAsync(id);
            if (contentPerson == null)
            {
                return NotFound();
            }
            ViewData["ContentId"] = new SelectList(_context.Contents, "Id", "Id", contentPerson.ContentId);
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Id", contentPerson.PersonId);
            return View(contentPerson);
        }

        // POST: ContentPerson/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContentId,PersonId,Role,CreditOrder")] ContentPerson contentPerson)
        {
            if (id != contentPerson.ContentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contentPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentPersonExists(contentPerson.ContentId))
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
            ViewData["ContentId"] = new SelectList(_context.Contents, "Id", "Id", contentPerson.ContentId);
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Id", contentPerson.PersonId);
            return View(contentPerson);
        }

        // GET: ContentPerson/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentPerson = await _context.ContentPeople
                .Include(c => c.Content)
                .Include(c => c.Person)
                .FirstOrDefaultAsync(m => m.ContentId == id);
            if (contentPerson == null)
            {
                return NotFound();
            }

            return View(contentPerson);
        }

        // POST: ContentPerson/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contentPerson = await _context.ContentPeople.FindAsync(id);
            if (contentPerson != null)
            {
                _context.ContentPeople.Remove(contentPerson);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentPersonExists(int id)
        {
            return _context.ContentPeople.Any(e => e.ContentId == id);
        }
    }
}
