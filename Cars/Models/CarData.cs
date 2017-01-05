using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cars.Models
{
    public class CarData
    {
        public IEnumerable<Car> Cars { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
    }
}