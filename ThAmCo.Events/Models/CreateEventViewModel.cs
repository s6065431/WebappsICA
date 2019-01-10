using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class CreateEventViewModel
    {

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan? Duration { get; set; }
        
        public string TypeId { get; set; }

        public IEnumerable<EventTypesDto> EventTypes { get; set; }

    }
}