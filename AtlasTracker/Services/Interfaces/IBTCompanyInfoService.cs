using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {

        //Declaration of a method
        public Task<List<Company>> GetCompanyInfoByIdAsync(int? companyId);
        public Task<List<Invite>> GetAllMembersAsync(int? companyId);

        public Task<List<Project>> GetAllProjectsAsync(int? companyId);

        public Task<List<BTUser>> GetAllTicketsAsync(int? companyId);
        public Task<List<BTUser>> GetAllInvitesAsync(int? companyId);

    }
}
