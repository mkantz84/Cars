using System.Collections.Generic;

namespace Cars.Models
{
    public class Store
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }

        public virtual List<Car> Cars { get; set; }
    }
}