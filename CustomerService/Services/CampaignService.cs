using CustomerService.Data.Base;
using CustomerService.Models;
using CustomerService.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository;

        public CampaignService(IRepository<Campaign> campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync()
        {
            return await _campaignRepository.GetAllAsync();
        }

        public async Task<Campaign> GetCampaignByIdAsync(int id)
        {
            var campaign = await _campaignRepository.GetByIdAsync(id);
            if (campaign == null)
            {
                throw new KeyNotFoundException($"Campaign with ID {id} not found.");
            }
            return campaign;
        }

        public async Task<Campaign> CreateNewCampaignAsync(Campaign campaign)
        {
            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign), "Campaign cannot be null.");
            }

            campaign.EndDate = campaign.StartDate.AddDays(6);

            return await _campaignRepository.AddAsync(campaign);
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var campaignToDelete = await _campaignRepository.GetByIdAsync(id);
            if (campaignToDelete == null)
            {
                throw new KeyNotFoundException($"Campaign with ID {id} not found.");
            }
            await _campaignRepository.DeleteAsync(id);
        }
    }
}
