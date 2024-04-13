using CustomerService.Models;

namespace CustomerService.Services
{
    public interface IPurchaseService
    {
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task<Purchase> GetPurchaseByIdAsync(int id);
        Task<Purchase> CreateNewPurchaseAsync(Purchase purchase);
        Task DeletePurchaseAsync(int id);
    }
}
