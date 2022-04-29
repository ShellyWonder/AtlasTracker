#nullable disable
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Models;
using Microsoft.AspNetCore.Identity;
using AtlasTracker.Services.Interfaces;
using AtlasTracker.Extensions;
using AtlasTracker.Models.Enums;
using AtlasTracker.Services;
using Microsoft.AspNetCore.Authorization;
using AtlasTracker.Models.ViewModels;

namespace AtlasTracker.Controllers
{

    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTProjectService _projectService;
        private readonly IBTFileService _fileService;
        private readonly IBTNotificationService notificationService;
        private readonly IBTHistoryService _historyService;
        private readonly IBTRolesService _rolesService;
        public TicketsController(ApplicationDbContext context,
                                 UserManager<BTUser> userManager,
                                 IBTTicketService ticketService,
                                 IBTCompanyInfoService companyInfoService,
                                 IBTLookupService lookupService,
                                 IBTProjectService projectService,
                                 IBTFileService fileService,
                                 IBTNotificationService notificationService,
                                 IBTHistoryService historyService,
                                 IBTRolesService rolesService)

        {
            _context = context;
            _userManager = userManager;
            _ticketService = ticketService;
            _companyInfoService = companyInfoService;
            _lookupService = lookupService;
            _projectService = projectService;
            _fileService = fileService;
            this.notificationService = notificationService;
            _historyService = historyService;
            _rolesService = rolesService;
        }


        //GET: MY TICKETS

        public async Task<IActionResult> MyTickets()
        {
            string userId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId();

            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(userId, companyId);
            //if (User)
            //{

            //}
            return View(tickets);
        }

        public async Task<IActionResult> AllTickets()
        {
            List<Ticket> tickets = new();
            int companyId = User.Identity.GetCompanyId();
            if (User.IsInRole(nameof(BTRole.Admin)) || User.IsInRole(nameof(BTRole.ProjectManager)))
            {
                tickets = await _companyInfoService.GetAllTicketsAsync(companyId);
            }
            else
            {
                tickets = (await _ticketService.GetAllTicketsByCompanyAsync(companyId));
            }

            return View(tickets);
        }

        /// ArchivedTickets
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(tickets);
        }
        //GET: Unassigned Tickets
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);
            return View(tickets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id, TicketId,Comment")] TicketComment ticketComment)
        {
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.CreatedDate = DateTime.UtcNow;

                    await _ticketService.AddTicketCommentAsync(ticketComment);

                }
                catch (Exception)
                {

                    throw;
                }
            }
            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BTUser btuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(btuser.CompanyId), "Id", "Name");

            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btuser.Id), "Id", "Name");

            }
            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            return View();
        }

        // POST: Tickets/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            ModelState.Remove("OwnerUserId");

            if (ModelState.IsValid)
            {
                ticket.OwnerUserId = _userManager.GetUserId(User);
                ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync("New")).Value;
                ticket.CreatedDate = DateTimeOffset.UtcNow;
                await _ticketService.UpdateTicketAsync(ticket);


                await _historyService.AddHistoryAsync(null, ticket, ticket.OwnerUserId);

                return RedirectToAction(nameof(AllTickets));
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");


            //: Ticket Create Notification
            BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
            int companyId = User.Identity!.GetCompanyId()!;
            Notification notification = new();


            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (id == null)
            {
                return NotFound();
            }


            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedDate,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }
            ModelState.Remove("OwnerUserId");
            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                try
                {
                    ticket.CreatedDate = DateTime.SpecifyKind(ticket.CreatedDate.DateTime, DateTimeKind.Utc);
                    ticket.Updated = DateTimeOffset.UtcNow;
                    await _ticketService.UpdateTicketAsync(ticket);

                    // Ticket Edit notification
                    BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                    int companyId = User.Identity!.GetCompanyId()!;
                    Notification notification = new()
                    {
                        TicketId = ticket.Id,
                        NotificationTypeId = (await _lookupService.LookupNotificationTypeIdAsync(nameof(BTNotificationType.Ticket))).Value,
                        Title = "Ticket updated",
                        Message = $"Ticket: {ticket.Title}, was updated by {btUser.FullName}",
                        CreatedDate = DateTime.UtcNow,
                        SenderId = btUser.Id,
                        RecipientId = projectManager?.Id
                    };
                    // Notify PM or Admin
                    if (projectManager != null)
                    {
                        await notificationService.AddNotificationAsync(notification);
                        await notificationService.SendEmailNotificationAsync(notification, "Ticket Updated");
                    }
                    else
                    {
                        BTUser admin = (await _rolesService.GetUsersInRoleAsync(nameof(BTRole.Admin), companyId)).FirstOrDefault();
                        notification.RecipientId = admin?.Id;
                        //Admin notification
                        await notificationService.AddNotificationAsync(notification);
                        await notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, nameof(BTRole.Admin));
                    }
                    //Notify Developer
                    if (ticket.DeveloperUserId != null)
                    {
                        Notification devNotification = new()
                        {
                            TicketId = ticket.Id,
                            NotificationTypeId = (await _lookupService.LookupNotificationTypeIdAsync(nameof(BTNotificationType.Ticket))).Value,
                            Title = "Ticket Updated",
                            Message = $"Ticket: {ticket.Title}, was updated by {btUser.FullName}",
                            CreatedDate = DateTimeOffset.Now,
                            SenderId = btUser.Id,
                            RecipientId = ticket.DeveloperUserId
                        };
                        await notificationService.AddNotificationAsync(devNotification);
                        await notificationService.SendEmailNotificationAsync(devNotification, "Ticket Updated");
                    }




                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllTickets));
            }
            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);

            return View(ticket);
        }

        //Get Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);

        }

        // Get: Tickets/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);

        }

        // POST: Tickets/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveTicket(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.ArchiveTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }

        //GET: Tickets/AssignDeveloper
        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            AssignDeveloperViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);
            model.DevelopersList = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(BTRole.Developer)), "Id", "FullName");

            return View(model);
        }

        //POST: Tickets/AssignDeveloper
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if (model.DevId != null)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                Ticket ticket = model.Ticket;
                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DevId);
                    await _historyService.AddHistoryAsync(ticket, model.Ticket, model.DevId);

                    // Assign Developer Notification
                    if (model.Ticket.DeveloperUserId != null)
                    {
                        Notification devNotification = new()
                        {
                            TicketId = model.Ticket.Id,
                            NotificationTypeId = (await _lookupService.LookupNotificationTypeIdAsync(nameof(BTNotificationType.Ticket))).Value,
                            Title = "Ticket Updated",
                            Message = $"Ticket: {model.Ticket.Title}, was updated by {btUser.FullName}",
                            CreatedDate = DateTime.UtcNow,
                            SenderId = btUser.Id,
                            RecipientId = model.Ticket.DeveloperUserId
                        };
                        await notificationService.AddNotificationAsync(devNotification);
                        await notificationService.SendEmailNotificationAsync(devNotification, "Ticket Updated");
                    }
                }
                catch (Exception)
                {



                    throw;
                }
                return RedirectToAction(nameof(Details), new { id = model.Ticket?.Id });
            }
            return RedirectToAction(nameof(AssignDeveloper), new { id = model.Ticket?.Id });
        }


        // Get: Tickets/Restore/5

        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);

        }

        // POST: Tickets/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreTicket(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.RestoreTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.ImageFormFile != null)
            {
                ticketAttachment.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.ImageFormFile);
                ticketAttachment.ImageFileName = ticketAttachment.ImageFormFile.FileName;
                ticketAttachment.ImageContentType = ticketAttachment.ImageFormFile.ContentType;

                ticketAttachment.CreatedDate = DateTimeOffset.Now;
                ticketAttachment.UserId = _userManager.GetUserId(User);

                await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }
        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    public async Task<IActionResult> ShowFile(int id)
    {
        TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.ImageFileName;
        byte[] fileData = ticketAttachment.ImageFileData;
        string ext = Path.GetExtension(fileName).Replace(".", "");

        Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
        return File(fileData, $"application/{ext}");
    }
    }
}
