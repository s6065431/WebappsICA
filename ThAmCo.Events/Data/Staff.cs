using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Data
{
    public class Staff
    {
        [Required]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string FullName => FirstName + " " + Surname;

        public bool FirstAidQualified { get; set; }

        public List<Staffing> Staffing { get; set; }
    }
}
