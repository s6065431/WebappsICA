using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class VenuesViewModel
    {
       

        public IEnumerable<AvailabilityDto> AvailableVenues { get; set; }

        public string VenueCode { get; set; }

        public string VenueName { get; set; }

        public string EventTitle { get; set; }

        public int EventId { get; set; }
    }

}