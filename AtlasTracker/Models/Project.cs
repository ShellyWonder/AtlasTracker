using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models
{
    public class Project
    {
        //Primary Key
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(240, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Project Name")]
        public string? Name { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} at most {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Project Description")]
        public string? Description { get; set; }
       

        [Required]
        [Display(Name = "CreatedDate")]
        [DataType(DataType.Date)]
        public DateTimeOffset CreatedDate { get; set;}

        
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set;}

        
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset EndDate { get; set; }

        
        public int ProjectPriorityId { get; set; }
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }
        
        public string? ImageFileName { get; set; }   

        public byte[]? ImageFileData { get; set; }
        [Display(Name = "File Extension")]
        public string? ImageContentType { get; set; }

       

        public bool Archived { get; set; }

       
        

        //Member(s)--nav
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket>? Tickets { get; set; } = new HashSet<Ticket>();

        //Nav properties
        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }
    }
}
