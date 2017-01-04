using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Cars.Models
{
    public class Rental
    {
        public int RentalID { get; set; }
        public int CarNumber { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReturningDate { get; set; }        

        public virtual Car Car { get; set; }
        public virtual User User { get; set; }
    }
}