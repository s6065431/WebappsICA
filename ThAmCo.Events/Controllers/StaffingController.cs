using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Controllers
{
    public class StaffingController : Controller
    {
        private readonly EventsDbContext _context;

        public StaffingController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Staffings
        public async Task<IActionResult> Index()
        {
            var eventsDbContext = _context.Staffing.Include(s => s.Event).Include(s => s.Staff);
            return View(await eventsDbContext.ToListAsync());
        }



        // GET: Staffings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title");
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Id");
            return View();
        }

        // POST: Staffings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,EventId")] Staffing staffing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staffing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title", staffing.EventId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Id", staffing.StaffId);
            return View(staffing);
        }





        // GET: Staffings/Delete/5
        public async Task<IActionResult> Delete(int eventId, int staffId)
        {

            var staffing = await _context.Staffing
                .Include(s => s.Event)
                .Include(s => s.Staff)
                .FirstOrDefaultAsync(m => m.StaffId == staffId && m.EventId == eventId);
            if (staffing == null)
            {
                return NotFound();
            }

            return View(staffing);
        }

        // POST: Staffings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId, int staffId)
        {
            var staffing = await _context.Staffing.FirstOrDefaultAsync(m => m.StaffId == staffId && m.EventId == eventId);
            _context.Staffing.Remove(staffing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffingExists(int id)
        {
            return _context.Staffing.Any(e => e.StaffId == id);
        }
    }
}
