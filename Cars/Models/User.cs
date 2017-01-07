using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Cars.Models
{
    public enum SexType
    {
        MALE,
        FEMALE,
    }
    public class User
    {
        public int UserID { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public SexType Sex { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsManager { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; }
    }
}