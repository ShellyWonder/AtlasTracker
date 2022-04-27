using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }     
        [Required]
        [Display(Name ="Priority Name")]
        public string? Name { get; set; }
    }
}
