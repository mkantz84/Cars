using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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
        //[DataType(DataType.Date)]
        //public DateTime? HireDateStart { get; set; }
        //[DataType(DataType.Date)]
        //public DateTime? HireDateEnd { get; set; }

        public virtual Store Store { get; set; }
        public virtual CarType CarType { get; set; }
    }
}