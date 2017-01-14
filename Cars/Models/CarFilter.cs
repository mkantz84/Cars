using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cars.Models
{
    public class CarFilter
    {
        public GearType Gear { get; set; }
        public int Year { get; set; }
        public string Manifacturer { get; set; }
        public string Model { get; set; }
        public string FreeText { get; set; }
    }
}