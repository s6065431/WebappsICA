﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Events.Data
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan? Duration { get; set; }

        [Required, MaxLength(3), MinLength(3)]
        public string TypeId { get; set; }

        public List<GuestBooking> Bookings { get; set; }

        public string VenueName { get; set; }

        [MinLength(13), MaxLength(13)]
        public string ReservationRef { get; set; }

        public List<Staffing> Staffing { get; set; }
    }
}
