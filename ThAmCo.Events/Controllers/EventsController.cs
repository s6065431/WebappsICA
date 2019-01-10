using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;
using ThAmCo.Events.Services;

namespace ThAmCo.Events.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventsDbContext _context;
        private readonly VenuesClient _venuesClient;

        public EventsController(EventsDbContext context, VenuesClient venuesClient)
        {
            _context = context;
            _venuesClient = venuesClient;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            
            return View(await _context.Events.Include(b => b.Bookings)
                .Include(e => e.Staffing)
                .ThenInclude(s => s.Staff)
                .ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }
        // GET: Events/Book
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            var avails =  _venuesClient.GetAvailablities(@event);

            var bookingViewModel = new VenuesViewModel
            {
                EventId = @event.Id,
                AvailableVenues = avails,
                VenueName = @event.VenueName
            };

            return View(bookingViewModel);
        }

        public async Task<IActionResult> BookVenuePost([FromQuery] int eventId, [FromQuery] string venueCode, [FromQuery] string venueName)
        {
            Event @event = await _context.Events.FindAsync(eventId);

            if (@event == null)
            {
                return NotFound();
            }

            VenuesViewModel postBook = new VenuesViewModel
            {
                EventId = eventId,
                EventTitle = @event.Title,
                VenueCode = venueCode,
                VenueName = venueName
            };
            return View(postBook);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> BookVenuePost([Bind("EventId,VenueCode,VenueName")] VenuesViewModel postVenue)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var @event = await _context.Events.FindAsync(postVenue.EventId);
                    if (@event == null)
                    {
                        return NotFound();
                    }

                    if (@event.ReservationRef != null)
                    {
                        var r = await _venuesClient.DeleteReservation(@event);
                    }

                    ReservationPostDto postReservation = new ReservationPostDto
                    {
                        VenueCode = postVenue.VenueCode,
                        StaffId = "s",
                        EventDate = @event.Date,
                    };
                    var reserveVenue = await _venuesClient.AddReservation(postReservation);

                    @event.ReservationRef = reserveVenue.Reference;
                    @event.VenueName = postVenue.VenueName;

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    if (!EventExists(postVenue.EventId))
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

            return View(postVenue);
        }

        
        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Duration,TypeId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id )
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            var avails = _venuesClient.GetAvailablities(@event);

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:22263");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            var reservation = new ReservationViewModel
            {
                EventDate = @event.Date,
                VenueCode = @event.VenueName
            };
            HttpResponseMessage response = await client.PostAsJsonAsync("api/Availabilities", reservation);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,Duration,TypeId")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Staffing(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Staffing)
                .ThenInclude(s => s.Staff)
                .FirstOrDefaultAsync(e => e.Id == id);

            return View(@event);
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
