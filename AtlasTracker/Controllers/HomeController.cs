using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.Enums;
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
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;

        public HomeController(ILogger<HomeController> logger, 
                                    IBTCompanyInfoService companyInfoService, 
                                    IBTProjectService projectService, 
                                    IBTTicketService ticketService)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            _projectService = projectService;
            _ticketService = ticketService;
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
        [HttpPost]
        //Google Charts Implementation
        public async Task<JsonResult> GglProjectTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });
            foreach (Project prj in projects)
            {
                chartData.Add(new object[] { prj.Name!, prj.Tickets!.Count() });
            }
            return Json(chartData);
        }
        [HttpPost]
        //Google Charts Implementation
        public async Task<JsonResult> GglProjectPriority()
        {
            int companyId = User.Identity!.GetCompanyId();
            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            List<object> chartData = new();
            chartData.Add(new object[] { "Priority", "Count" });
            foreach (string priority in Enum.GetNames(typeof(BTProjectPriority)))
            {
                int priorityCount = (await _projectService.GetAllProjectsByPriority(companyId, priority)).Count();
                chartData.Add(new object[] { priority, priorityCount });
            }

            return Json(chartData);
        }
        [HttpPost]
        //Google Charts Implementation
        public async Task<JsonResult> GglTicketPriority()
        {
            int companyId = User.Identity!.GetCompanyId();

            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "Priority", "Count" });


            foreach (string priority in Enum.GetNames(typeof(BTTicketPriority)))
            {
                int priorityCount = (await _ticketService.GetAllTicketsByPriorityAsync(companyId, priority)).Count();
                chartData.Add(new object[] { priority, priorityCount });
            }

            return Json(chartData);
        }
        [HttpPost]
        //Google Charts Implementation
        public async Task<JsonResult> GglTicketStatus()
        {
            int companyId = User.Identity!.GetCompanyId();

            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "Status", "Count" });


            foreach (string status in Enum.GetNames(typeof(BTTicketStatus)))
            {
                int statusCount = (await _ticketService.GetAllTicketsByStatusAsync(companyId, status)).Count();
                chartData.Add(new object[] { status, statusCount });
            }

            return Json(chartData);
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