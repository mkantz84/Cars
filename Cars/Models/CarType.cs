using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Cars.Models
{
    public enum GearType
    {
        MANUAL,
        AUTOMATIC,
    }
    public class CarType
    {
        public int CarTypeID { get; set; }
        public string ModelName { get; set; }
        public string ManifacturerName { get; set; }
        public int DailyPrice { get; set; }
        public int LateDayPrice { get; set; }
        public int Year { get; set; }
        public GearType? Gear { get; set; }
        public string picture { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        //public override string ToString()
        //{
        //    return string.Format("{0} {1}", ModelName, ManifacturerName);
        //}
    }
}