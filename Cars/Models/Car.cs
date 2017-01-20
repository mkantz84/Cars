
namespace Cars.Models
{
    public class Car
    {
        public int ID { get; set; }
        public int CarNumber { get; set; }
        public int Kilometer { get; set; }
        public bool IsProper { get; set; }
        public bool IsAvailable { get; set; }
        public int CarTypeID { get; set; }
        public int StoreID { get; set; }

        public virtual Store Store { get; set; }
        public virtual CarType CarType { get; set; }
    }
}