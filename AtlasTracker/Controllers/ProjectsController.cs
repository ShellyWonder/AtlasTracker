#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AtlasTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Models.Enums;
using AtlasTracker.Services;

namespace AtlasTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTFileService _fileService;
        public ProjectsController(ApplicationDbContext context, IBTProjectService projectService, UserManager<BTUser> userManager, IBTRolesService rolesService, IBTLookupService lookupService, IBTCompanyInfoService companyInfoService, IBTFileService fileService)
        {
            _context = context;
            _projectService = projectService;
            _userManager = userManager;
            _rolesService = rolesService;
            _lookupService = lookupService;
            _companyInfoService = companyInfoService;
            _fileService = fileService;
        }

        /// <summary>


        /// UnassignedProjects


        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }
        //Get:Projects/MyProjects
        //projects by user
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);
            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }

        //Get:Projects/AllProjects
        public async Task<IActionResult> AllProjects()
        {
            List<Project> projects = new();
            int companyId = User.Identity.GetCompanyId();
            if (User.IsInRole(nameof(BTRole.Admin)) || User.IsInRole(nameof(BTRole.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            }

            return View(projects);
        }

        /// ArchivedProjects
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Project> projects = await _projectService.GetArchivedProjectsByCompany(companyId);

            return View(projects);
        }
        /// Unassigned Projects
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Project> projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);

        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId();
            AssignPMViewModel model = new();
            model.Project = await _projectService.GetProjectByIdAsync(projectId.Value, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                return RedirectToAction(nameof(AllProjects));
            }
            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project!.Id });
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //string userId = _userManager.GetUserId(User);
            //BTUser btuser = _context.Users.Find(userId);

            int companyId = User.Identity.GetCompanyId();
            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();
            AddProjectWithPMViewModel model = new();

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;

                    }
                    model.Project.CreatedDate = DateTimeOffset.UtcNow;
                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate.DateTime, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate.DateTime, DateTimeKind.Utc);



                    model.Project.CompanyId = companyId;
                    await _projectService.AddNewProjectAsync(model.Project);



                    if (!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                    }



                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {



                    throw;
                }
            }
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model.Project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            AddProjectWithPMViewModel model = new();
            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (model.Project == null)
            {
                return NotFound();
            }
            BTUser projectManager = await _projectService.GetProjectManagerAsync(model.Project.Id);
            if (projectManager != null)
            {
                model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName", projectManager.Id);

            }
            else
            {

                model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");

            }
                model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;

                    }
                    model.Project.CreatedDate = DateTimeOffset.UtcNow;
                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate.DateTime, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate.DateTime, DateTimeKind.Utc);

                    await _projectService.UpdateProjectAsync(model.Project);

                    //add PM if chosen
                    if (!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);

                    }
                    return RedirectToAction("Index");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project!.Id))
                    {
                        return NotFound();
                    }
                    else
                    {

                        throw;
                    }
                }

            }
           
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

       

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveProject(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RestoreProject(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
        }
    }
}
