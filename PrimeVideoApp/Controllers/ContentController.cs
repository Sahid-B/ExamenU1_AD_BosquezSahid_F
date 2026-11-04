using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrimeVideoApp.Data;
using PrimeVideoApp.Hubs;
using PrimeVideoApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace PrimeVideoApp.Controllers
{
    public class ContentController : Controller
    {
        private readonly PrimeVideoContext _context;
        private readonly IHubContext<ContentHub> _hubContext;
        private readonly IMemoryCache _cache;

        public ContentController(PrimeVideoContext context, IHubContext<ContentHub> hubContext, IMemoryCache cache)
        {
            _context = context;
            _hubContext = hubContext;
            _cache = cache;
        }

        // GET: Content
        public async Task<IActionResult> Index(int page = 1)
        {
            var contents = await GetContentsPage(page);
            _cache.TryGetValue("ContentStats", out var stats);
            ViewBag.Stats = stats;
            return View(contents);
        }

        // GET: Content/Table
        public async Task<IActionResult> Table(int page = 1)
        {
            return PartialView("_ContentTable", await GetContentsPage(page));
        }

        private Task<PaginatedList<Content>> GetContentsPage(int page)
        {
            const int pageSize = 10;
            return PaginatedList<Content>.CreateAsync(_context.Contents.OrderBy(c => c.Id), page, pageSize);
        }

        // GET: Content/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // GET: Content/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Content/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SourceId,ContentType,Title,DateAdded,ReleaseYear,Rating,DurationValue,DurationUnit,DurationText,Description")] Content content)
        {
            if (ModelState.IsValid)
            {
                _context.Add(content);
                await _context.SaveChangesAsync();
                await NotifyContentChanged("created", content.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        // GET: Content/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }

        // POST: Content/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SourceId,ContentType,Title,DateAdded,ReleaseYear,Rating,DurationValue,DurationUnit,DurationText,Description")] Content content)
        {
            if (id != content.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(content);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentExists(content.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                await NotifyContentChanged("updated", content.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        // GET: Content/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // POST: Content/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content != null)
            {
                _context.Contents.Remove(content);
            }

            await _context.SaveChangesAsync();
            await NotifyContentChanged("deleted", id);
            return RedirectToAction(nameof(Index));
        }

        private Task NotifyContentChanged(string action, int id)
        {
            return _hubContext.Clients.All.SendAsync("ContentChanged", action, id);
        }

        private bool ContentExists(int id)
        {
            return _context.Contents.Any(e => e.Id == id);
        }
    }
}
