﻿using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces
{
    public interface IBTHistoryService 
    {
        Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);

        Task AddHistoryAsync(int ticketId, string model, string userId);

        Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);

        Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);
    }
}
