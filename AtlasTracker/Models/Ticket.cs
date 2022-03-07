﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Ticket
    {
        //primary key
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Ticket Title")]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000)]
        [DisplayName("Ticket Description")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="CreatedDate")]
        public DateTimeOffset? CreatedDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated")]
        public DateTimeOffset? Updated { get; set; }

        [Display(Name = "Archived By Project")]
        public bool Archived { get; set; }
                      
        [Display(Name = "Archived By Project")]
        public bool ArchivedByProject { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int TicketTypeId {get; set;}

        public int TicketPriorityId { get; set; }

        public int TicketStatusId { get; set; }
        [Required]
        public string? OwnerUserId { get; set; }
        
        public string? DeveloperUserId { get; set; }
       
        
        
        //nav properties
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }

        public virtual TicketType? TicketType { get; set; }

        public virtual BTUser? OwnerUser { get; set; }

        public virtual BTUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment>? Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment>? Attachments { get; set; } = new HashSet<TicketAttachment>();

        public virtual ICollection<TicketStatus>? Statuses { get; set; } = new HashSet<TicketStatus>();

        public virtual ICollection<TicketHistory>? History { get; set; } = new HashSet<TicketHistory>();

        public virtual ICollection<Notification>? Notifications { get; set; } = new HashSet<Notification>();







    }
}
