using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }     
        [Required]
        public string? Name { get; set; }
    }
}
