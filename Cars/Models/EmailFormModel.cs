using System.ComponentModel.DataAnnotations;

namespace Cars.Models
{
    public class EmailFormModel
    {
        [Required, Display(Name = "Your name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Your email"), EmailAddress]
        public string FromEmail { get; set; }
        public string FromMobile { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}