using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services
{
    public class BTLookupService: IBTLookupService
    {
        private readonly ApplicationDbContext _context;

        public BTLookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<TicketPriority>> GetTicketPrioritiesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<TicketStatus>> GetTicketStatusesAsync()
        {
            return await _context.TicketStatuses.ToListAsync();
        }

        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            return await _context.TicketTypes.ToListAsync();
        }

        public async Task<int?> LookupNotificationTypeIdAsync(string typeName)
        {
            try
            {
                NotificationType? notificationType = await _context.NotificationTypes.FirstOrDefaultAsync(n => n.Name == typeName);
                return notificationType!.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    } 


}
