using CustomerService.Models;

namespace CustomerService.Services.Contracts
{
    public interface IPurchaseService
    {
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task<Purchase> GetPurchaseByIdAsync(int id);
        Task<Purchase> CreateNewPurchaseAsync(Purchase purchase);
        Task DeletePurchaseAsync(int id);
        Task<bool> IsPurchaseDailyLimitForAgentMet(int agentId, int campaignId, DateTime day);
        Task<bool> IsPurchaseCreatedForCustomerInCampaign(int campaignId, int customerId);
    }
}
