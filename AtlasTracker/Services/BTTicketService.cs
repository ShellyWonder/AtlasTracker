using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketService(ApplicationDbContext context)
        {
            _context = context;
        }


        //CRUD Create
        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        //CRUD Read
        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                                    .Include(t => t.Title)
                                    .Include(t => t.Description)
                                    .Include(t => t.CreatedDate)
                                    .Include(t => t.Updated)
                                    .Include(t => t.Attachments)
                                    .Include(t => t.Comments)
                                    .Include(t => t.DeveloperUser)
                                    .Include(t => t.DeveloperUserId)
                                    .Include(t => t.OwnerUser)
                                    .Include(t => t.OwnerUserId)
                                    .Include(t => t.TicketStatusId)
                                    .FirstOrDefaultAsync(t => t.Id == ticketId);     
                     return  ticket!;
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        //CRUD Update
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        //CRUD Delete
        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Add(ticket.Archived);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }
        //******************************************************************************************************
        public Task AssignTicketAsync(int ticketId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        
        public Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            throw new NotImplementedException();
        }

        public Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            throw new NotImplementedException();
        }
        
    }
}

