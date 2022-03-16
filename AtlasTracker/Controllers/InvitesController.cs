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
using AtlasTracker.Extensions;
using Microsoft.AspNetCore.DataProtection;

using Microsoft.AspNetCore.Identity.UI.Services;

namespace AtlasTracker.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IDataProtector _protector;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTInviteService _inviteService;
        private readonly IEmailSender _emailSender;
        public InvitesController(ApplicationDbContext context, IBTProjectService projectService, IDataProtectionProvider dataProtectionProvider, IEmailSender emailSender, IBTCompanyInfoService companyInfoService, IBTInviteService inviteService)
        {
            _context = context;
            _projectService = projectService;
            _protector = dataProtectionProvider.CreateProtector("CF.ATLAS.TheBugTracker");
            _emailSender = emailSender;
            _companyInfoService = companyInfoService;
            _inviteService = inviteService;
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // GET: Invites/Create
        public async Task<IActionResult> CreateAsync()
        {
            int companyId = User.Identity.GetCompanyId();
            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");

            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        {
            int companyId = User.Identity.GetCompanyId();
            ModelState.Remove("InvitorId");
            if (ModelState.IsValid)
            {
                try
                {
                    Guid guid = Guid.NewGuid();
                    string token = _protector.Protect(guid.ToString());
                    string email = _protector.Protect(invite.InviteeEmail);
                    string company = _protector.Protect(companyId.ToString());

                    string destination = invite.InviteeEmail;

                    Company btCompany = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
                    string subject = $"/ AtlasTracker: {btCompany.Name} Invite";
                    string callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company }, protocol: Request.Scheme);

                    string body = $@"{invite.Message} <br />
                        Please join my Company. <br />
                        Click the following link to join our team. <br />
                        <a href=""{callbackUrl}"">COLLABORATE</a>";

                    await _emailSender.SendEmailAsync(destination, subject, body);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    throw;
                }
            }
                    ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
                    return View(invite);
           
        }
        [HttpGet]
        public async Task<IActionResult> ProcessInvite(string token, string email, string company)
        {
            if (token == null)
            {
                return NotFound();
            }
            Guid companyToken = Guid.Parse(_protector.Unprotect(token));
            string inviteeEmail = _protector.Unprotect(email);
            int companyId = int.Parse(_protector.Unprotect(company));
            try
            {
                Invite invite = await _inviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);
                if (invite != null)
                {
                    return View(invite);
                }
                return NotFound();
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
