﻿using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces

{
    public interface IBTLookupService
    {
        public  Task<List<TicketPriority>> GetTicketPrioritiesAsync();
        public Task<List<TicketStatus>> GetTicketStatusesAsync();
        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
        public Task<List<TicketType>> GetTicketTypesAsync();
        public Task<int?> LookupNotificationTypeIdAsync(string typeName);

    }
}
