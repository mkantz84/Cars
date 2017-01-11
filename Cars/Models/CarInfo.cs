using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cars.Models
{
    public class CarInfo
    {
        public Car Car { get; set; }
        public CarType CarType { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? HireDateStart { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? HireDateEnd { get; set; }
    }
}