using AutoMapper;
using CustomerService.Data.Base;
using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IMapper _mapper;

        public PurchaseService(IRepository<Purchase> purchaseRepository, IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
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

        public async Task<PurchaseReadDto> CreateNewPurchaseAsync(PurchaseWriteDto purchaseDto, int campaignId)
        {
            if (purchaseDto == null)
            {
                throw new ArgumentNullException(nameof(purchaseDto), "Purchase cannot be null.");
            }

            var purchase = _mapper.Map<Purchase>(purchaseDto);
            purchase.CampaignId = campaignId;

            var createdPurchase = await _purchaseRepository.AddAsync(purchase);
            return _mapper.Map<PurchaseReadDto>(createdPurchase);
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

        public async Task<IEnumerable<Purchase>> GetPurchasesByCampaignAsync(int campaignId)
        {
            var purchases = await _purchaseRepository.SearchExtendedAsync(
             p => p.CampaignId == campaignId,
             query => query.Include(p => p.Agent));

            return purchases.ToList();
        }
    }
}
