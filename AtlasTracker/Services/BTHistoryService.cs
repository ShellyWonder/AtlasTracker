using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;

namespace AtlasTracker.Services
{
    public class BTHistoryService : IBTHistoryService
    {
        public async Task? AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task? AddHistoryAsync(int ticketId, string model, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
