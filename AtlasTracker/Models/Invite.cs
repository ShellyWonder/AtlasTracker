using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }
        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }
        [DisplayName("Join Date")]
        public DateTimeOffset? JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project Name")]
        public int ProjectId { get; set; }

        [Required]
        [DisplayName("Invitor")]
        public string? InvitorId { get; set; }

        [Required]
        [DisplayName("Invitee")]
        public string? InviteeId { get; set; }

        [Required]
        [DisplayName("Invitee Email")]
        public string? InviteeEmail { get; set; }

        [Required]
        [DisplayName("Invitee First Name")]
        public string? InviteeFirstName { get; set; }

        [Required]
        [DisplayName("Invitee Last Name")]
        public string? InviteeLastName { get; set; }

        [Required]
        [DisplayName("Invite Message")]
        public string? Message { get; set; }

        public bool IsValid { get; set; }

        public virtual Company? Company { get; set; }
        public virtual BTUser? Invitor { get; set; }
        public virtual BTUser? Invitee { get; set; }
        public virtual Project? Project { get; set; }
    }
}