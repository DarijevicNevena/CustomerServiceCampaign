using AutoMapper;
using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;

namespace CustomerService.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IMapper _mapper;

        public CampaignService(IRepository<Campaign> campaignRepository, IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CampaignReadDto>> GetAllCampaignsAsync()
        {
            var campaigns = await _campaignRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CampaignReadDto>>(campaigns);
        }

        public async Task<CampaignReadDto> GetCampaignByIdAsync(int id)
        {
            var campaign = await _campaignRepository.GetByIdAsync(id);
            if (campaign == null)
            {
                throw new KeyNotFoundException($"Campaign with ID {id} not found.");
            }
            return _mapper.Map<CampaignReadDto>(campaign);
        }

        public async Task<CampaignReadDto> CreateNewCampaignAsync(CampaignWriteDto campaignDto)
        {
            if (campaignDto == null)
            {
                throw new ArgumentNullException(nameof(campaignDto), "Campaign DTO cannot be null.");
            }

            var campaign = _mapper.Map<Campaign>(campaignDto);
            campaign.EndDate = campaign.StartDate.AddDays(6);
            var createdCampaign = await _campaignRepository.AddAsync(campaign);

            return _mapper.Map<CampaignReadDto>(createdCampaign);
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

        public async Task<Campaign?> GetCampaignByNameAsync(string name)
        {
            var campaign = await _campaignRepository.SearchAsync(c =>
              c.CampaignName == name);
            return campaign.SingleOrDefault();
        }
    }
}
