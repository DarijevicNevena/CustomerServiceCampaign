using CustomerService.Models;
using CustomerService.Models.ModelDto;

namespace CustomerService.Services.Contracts
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseReadDto>> GetAllPurchasesAsync();
        Task<PurchaseReadDto> GetPurchaseByIdAsync(int id);
        Task<PurchaseReadDto> CreateNewPurchaseAsync(PurchaseWriteDto purchase,int campaignId);
        Task DeletePurchaseAsync(int id);
        Task<bool> IsPurchaseDailyLimitForAgentMet(int agentId, int campaignId, DateTime day);
        Task<bool> IsPurchaseCreatedForCustomerInCampaign(int campaignId, int customerId);
        Task<IEnumerable<Purchase>> GetPurchasesByCampaignAsync(int campaignId);
    }
}
