using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {

        //Declaration of a method
        public  Task<Company> GetCompanyInfoByIdAsync(int? companyId);
        public Task<List<BTUser>> GetAllMembersAsync(int? companyId);

        public Task<List<Project>> GetAllProjectsAsync(int? companyId);

        public Task<List<Ticket>> GetAllTicketsAsync(int? companyId);
        public Task<List<Invite>> GetAllInvitesAsync(int? companyId);

    }
}
