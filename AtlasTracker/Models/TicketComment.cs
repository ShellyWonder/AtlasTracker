using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

       [Required]
        [DisplayName("Member Comment")]
        [StringLength(2000)]
        public string? Comment { get; set; }
        
        [DataType(DataType.Date)]
        
        [DisplayName("Date")]
        public DateTimeOffset CreatedDate { get; set; }

        public int TicketId { get; set;}

        public string? UserId { get; set; }

        //Nav properties
        [DisplayName("Ticket")]
        public virtual Ticket? Ticket { get; set; }
        [DisplayName("Team Member")]

        public virtual BTUser? User { get; set; }

    }

}
