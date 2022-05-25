using Microsoft.AspNetCore.Mvc;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtlasTracker.Services;
using AtlasTracker.Models;

namespace AtlasTracker.Controllers
{
 
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTRolesService _rolesService;

       public  UserRolesController(IBTCompanyInfoService companyInfoService, IBTRolesService rolesService) 
        {
    
        _companyInfoService = companyInfoService;
            _rolesService = rolesService;
        }

            public IActionResult Index()
                {
                    return View();  
                }
        
            [HttpGet]
            public async Task<IActionResult> ManageUserRoles()
            {
            //add an instance of ViewModel as a list(model)
             
            List<UserRolesViewModel> model = new();

            //get Company Id
            int companyId = User.Identity!.GetCompanyId();
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);
            foreach(BTUser user in users)
            {
                //instantiate viewModel

                UserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            return View(model); 
        }
        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(UserRolesViewModel member) 
        {
        int companyId = User.Identity!.GetCompanyId();
       
            //instantiate the btUser
        BTUser? btUser = (await _companyInfoService.GetAllMembersAsync(companyId))
                                                   .FirstOrDefault(u => u.Id == member.BTUser?.Id);
        IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser!);
        //Get selected Roles for the user:

        string userRole = member.SelectedRoles?.FirstOrDefault()!;
        if (!string.IsNullOrEmpty(userRole))
        {
            if(await _rolesService.RemoveUserFromRolesAsync(btUser!, roles))
            {
                await _rolesService.AddUserToRoleAsync(btUser!, userRole);
            }
        }

        return RedirectToAction("Dashboard", "Home");
        }




    }
}
