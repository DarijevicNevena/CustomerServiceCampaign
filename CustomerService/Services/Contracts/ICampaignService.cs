using CustomerService.Models;
using CustomerService.Models.ModelDto;

namespace CustomerService.Services.Contracts
{
    public interface ICampaignService
    {
        Task<IEnumerable<CampaignReadDto>> GetAllCampaignsAsync();
        Task<CampaignReadDto> GetCampaignByIdAsync(int id);
        Task<Campaign?> GetCampaignByNameAsync(string name);
        Task<CampaignReadDto> CreateNewCampaignAsync(CampaignWriteDto campaign);
        Task DeleteCampaignAsync(int id);
    }
}
