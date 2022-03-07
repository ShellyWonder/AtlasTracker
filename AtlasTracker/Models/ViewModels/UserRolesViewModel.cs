using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public BTUser? BTUser { get; set;}

        public MultiSelectList? Roles { get; set; }
        public List<string>? SelectedRoles { get; set; }


    }
}
