using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public Ticket? Ticket { get; set; }
        public SelectList? DevelopersList { get; set; }
        public string? DevId { get; set; }
    }
}
