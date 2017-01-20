using System.Collections.Generic;

namespace Cars.Models
{
    public enum GearType
    {
        DEFAULT,
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
    }
}