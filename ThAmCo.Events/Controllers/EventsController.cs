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

        private readonly EventsDataAccess _dataAccess;

        private readonly VenuesClient _venuesClient;

        public EventsController(EventsDbContext context, VenuesClient venuesClient)
        {
            _context = context;
            _dataAccess = new EventsDataAccess(context);
            _venuesClient = venuesClient;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            
            return View(await _dataAccess.GetEvents()
                .Include(b => b.Bookings)
                .Include(e => e.Staffing)
                .ThenInclude(s => s.Staff)
                .ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var @event = await _dataAccess.GetEvents()
                .Include(b => b.Bookings)
                .ThenInclude(b => b.Customer)
                .Include(e => e.Staffing)
                .ThenInclude(s => s.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            var eventTypes = await _venuesClient.GetEventTypes();

            var viewModel = new CreateEventViewModel {
                EventTypes = eventTypes
            };

            return View(viewModel);
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Date,Duration,TypeId")] CreateEventViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var @event = new Event
                {
                    Title = viewModel.Title,
                    Date = viewModel.Date,
                    Duration = viewModel.Duration,
                    TypeId = viewModel.TypeId
                };

                _context.Add(@event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }

        // GET: Events/Book
        public async Task<IActionResult> Book(int id)
        {
            var @event = await _dataAccess.GetEvents().FirstOrDefaultAsync(e => e.Id == id);

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
            Event @event = await _dataAccess.GetEvents().FirstOrDefaultAsync(e => e.Id == eventId);

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
                    var @event = await _dataAccess.GetEvents().FirstOrDefaultAsync(e => e.Id == postVenue.EventId);
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

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _dataAccess.GetEvents().FirstOrDefaultAsync(e => e.Id == id);

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
        public async Task<IActionResult> Delete(int id)
        {
            var @event = await _dataAccess.GetEvents()
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
            var @event = await _dataAccess.GetEvents().FirstOrDefaultAsync(e => e.Id == id);

            _context.Remove(@event);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Staffing(int id)
        {
            var @event = await _dataAccess.GetEvents()
                .Include(e => e.Staffing)
                .ThenInclude(s => s.Staff)
                .Include(e => e.Bookings)
                .ThenInclude(b => b.Customer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        public async Task<IActionResult> GuestList(int id)
        {
            var @event = await _dataAccess.GetEvents()
                .Include(e => e.Staffing)
                .ThenInclude(s => s.Staff)
                .Include(e => e.Bookings)
                .ThenInclude(b => b.Customer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        private bool EventExists(int id)
        {
            return _dataAccess.GetEvents().Any(e => e.Id == id);
        }
    }
}
