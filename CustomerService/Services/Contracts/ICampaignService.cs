using CustomerService.Models;

namespace CustomerService.Services.Contracts
{
    public interface ICampaignService
    {
        Task<IEnumerable<Campaign>> GetAllCampaignsAsync();
        Task<Campaign> GetCampaignByIdAsync(int id);
        Task<Campaign> CreateNewCampaignAsync(Campaign campaign);
        Task DeleteCampaignAsync(int id);
    }
}
