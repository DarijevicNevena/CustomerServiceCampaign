using CustomerService.Data.Base;
using CustomerService.Models;
using CustomerService.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Services
{
    public class PurchaseServise : IPurchaseService
    {
        private readonly IRepository<Purchase> _purchaseRepository;

        public PurchaseServise(IRepository<Purchase> purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await _purchaseRepository.GetAllAsync();
        }

        public async Task<Purchase> GetPurchaseByIdAsync(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
            {
                throw new KeyNotFoundException($"Purchase with ID {id} not found.");
            }
            return purchase;
        }

        public async Task<Purchase> CreateNewPurchaseAsync(Purchase purchase)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase), "Purchase cannot be null.");
            }

            return await _purchaseRepository.AddAsync(purchase);
        }

        public async Task DeletePurchaseAsync(int id)
        {
            var purchaseToDelete = await _purchaseRepository.GetByIdAsync(id);
            if (purchaseToDelete == null)
            {
                throw new KeyNotFoundException($"Purchase with ID {id} not found.");
            }
            await _purchaseRepository.DeleteAsync(id);
        }

        public async Task<bool> IsPurchaseDailyLimitForAgentMet(int agentId, int campaignId, DateTime day)
        {
            var purchases = await _purchaseRepository.SearchAsync(p =>
                p.AgentId == agentId &&
                p.CampaignId == campaignId &&
                p.Date.Date == day.Date);

            return purchases.Count() >= 5;
        }

        public async Task<bool> IsPurchaseCreatedForCustomerInCampaign(int campaignId, int customerId)
        {
            var purchases = await _purchaseRepository.SearchAsync(p =>
               p.CustomerId == customerId &&
               p.CampaignId == campaignId);

            return purchases.Any();
        }
    }
}
