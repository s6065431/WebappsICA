using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Data
{
    public class EventsDataAccess
    {

        private readonly EventsDbContext _context;

        public EventsDataAccess(EventsDbContext context)
        {
            _context = context;
        }

        public IQueryable<Event> GetEvents()
        {
            return _context.Events.Where(e => e.IsActive);
        }

        public IEnumerable<GuestBooking> GetGuests()
        {
            return _context.Guests
                .Include(g => g.Customer)
                .Include(g => g.Event)
                .Where(g => g.Event.IsActive);
        }

        public IEnumerable<Staffing> GetStaffing()
        {
            return _context.Staffing
                .Include(s => s.Staff)
                .Include(s => s.Event)
                .Where(s => s.Event.IsActive);
        }

    }
}