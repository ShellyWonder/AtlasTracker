using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class TicketHistory
    {
        //primary key
        public int Id { get; set; }
        
        //Foreign Key
        [Required]
        [DisplayName("Ticket Name")]
        public int TicketId { get; set; }

        [DisplayName("Updated Ticket Item")]
        public string? PropertyName { get; set; }

        [DisplayName("Attachment Description")]
        public string? Description { get; set; }

        [DisplayName("Created Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset? CreatedDate { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Previous")]
        public string? OldValue { get; set; }

        [DisplayName("Current")]
        public string? NewValue { get; set; }

        //Foreign Key
        [Required]
        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        //Nav properties
        [DisplayName("Ticket")]
        public virtual Ticket? Ticket { get; set; }
        

        public virtual BTUser? User { get; set; }
    }
}
