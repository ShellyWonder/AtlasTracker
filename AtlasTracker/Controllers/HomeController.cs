using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
 
namespace AtlasTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyInfoService _companyInfoService;
        

        public HomeController(ILogger<HomeController> logger, IBTCompanyInfoService companyInfoService)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
        }
        
        [HttpGet]   
        [Authorize]
        public  async Task<IActionResult> Dashboard(string swalMessage = null!)
        {
            int companyId = User.Identity!.GetCompanyId();
            DashboardViewModel model = new();
            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
            model.Projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            model.Tickets = await _companyInfoService.GetAllTicketsAsync(companyId);
            model.Members = await _companyInfoService.GetAllMembersAsync(companyId);

            ViewData["SwalMessage"] = swalMessage;
            return View(model);
        }
        public IActionResult Landing()
        {
            return View();
        }
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }
        public IActionResult Default()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}