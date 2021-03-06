﻿using System;
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
        private readonly EventsDataAccess _dataAccess;

        private readonly EventsDbContext _context;

        public StaffingController(EventsDbContext context)
        {
            _context = context;
            _dataAccess = new EventsDataAccess(context);
        }

        // GET: Staffings
        public async Task<IActionResult> Index(int? eventId)
        {
            var eventsDbContext = _context.Staffing
                .Include(s => s.Event)
                .Include(s => s.Staff)
                .Where(s => s.Event.IsActive && eventId == null ||  s.EventId == eventId);
            return View(await eventsDbContext.ToListAsync());
        }

        // GET: Staffings/Create
        public async Task<IActionResult> Create()
        {
            ViewData["EventId"] = new SelectList(_dataAccess.GetEvents(), "Id", "Title");
            ViewData["StaffId"] = new SelectList(await _context.Staff.ToListAsync(), "Id", "Id");
            return View();
        }

        // POST: Staffings/Create
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
            ViewData["EventId"] = new SelectList(_dataAccess.GetEvents(), "Id", "Title", staffing.EventId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.ToListAsync(), "Id", "Id", staffing.StaffId);
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

            return RedirectToAction("Index", "Events");
        }

        private bool StaffingExists(int id)
        {
            return _context.Staffing.Any(e => e.StaffId == id);
        }
    }
}
