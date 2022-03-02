using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        

        [DisplayName("File Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Added")]

        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }

        public string? ImageFileName { get; set; }

        public byte[]? ImageFileData { get; set; }
        [Display(Name = "File Extension")]
        public string? ImageContentType { get; set; }

        //nav prop
        public virtual Ticket? Ticket { get; set; }

        public virtual BTUser? User { get; set; }
    }
}
