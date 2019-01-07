using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class AvailabilityDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Capacity { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0.0, Double.MaxValue)]
        public double CostPerHour { get; set; }
    }
}
