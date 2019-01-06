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
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string VenueCode { get; set; }

        [ForeignKey(nameof(VenueCode))]
        public VenuesDto Venue { get; set; }

        [Range(0.0, Double.MaxValue)]
        public double CostPerHour { get; set; }

        public ReservationDto Reservation { get; set; }

    }
}
