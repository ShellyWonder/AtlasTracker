﻿using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;




namespace AtlasTracker.Services

{
    public class BTNotificationService : IBTNotificationService
    {


        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTRolesService _rolesService;

        public BTNotificationService(ApplicationDbContext context,
                                    IBTRolesService rolesService,
                                     IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
            _emailSender = emailSender;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                                                 .Include(n => n.Recipient)
                                                                 .Include(n => n.Sender)
                                                                 .Include(n => n.Ticket)
                                                                    .ThenInclude(t => t.Project)
                                                                 .Where(n => n.RecipientId == userId).ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                                                 .Include(n => n.Recipient)
                                                                 .Include(n => n.Sender)
                                                                 .Include(n => n.Ticket)
                                                                 .ThenInclude(t => t.Project)
                                                                 .Where(n => n.SenderId == userId).ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            try
            {
                BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

                string btUserEmail = btUser!.Email;
                string message = notification.Message!;

                //Send Email
                try
                {
                    await _emailSender.SendEmailAsync(btUserEmail, emailSubject, message);
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }


            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

                
    }

}


