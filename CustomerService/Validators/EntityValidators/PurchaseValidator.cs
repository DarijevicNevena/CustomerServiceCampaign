using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using FluentValidation;
using FluentValidation.Results;

namespace CustomerService.Validators.EntityValidators
{
    public class PurchaseValidator
    {
        private readonly IAgentService _agentService;
        private readonly IPurchaseService _purchaseService;
        private readonly ICampaignService _campaignService;
        private readonly ICustomerService _customerService;

        public PurchaseValidator(
        IAgentService agentService,
        IPurchaseService purchaseService,
        ICampaignService campaignService, 
        ICustomerService customerService)
        {
            _agentService = agentService;
            _purchaseService = purchaseService;
            _campaignService = campaignService;
            _customerService = customerService;

        }

        public async Task<PurchaseValidationResult> ValidateAsync(PurchaseWriteDto purchase)
        {
            var result = new PurchaseValidationResult();

            // Check customer
            var customerExists = await _customerService.DoesCustomerExist(purchase.CustomerId);
            if (!customerExists)
            {
                result.ValidationResult.Errors.Add(new ValidationFailure("CustomerId", "Customer does not exist."));
            }

            // Check if agent exists
            var agent = await _agentService.GetAgentByIdAsync(purchase.AgentId);
            if (agent == null)
            {
                result.ValidationResult.Errors.Add(new ValidationFailure("AgentId", "Agent does not exist."));
            }

            // Check if campaign exists
            var campaign = await _campaignService.GetCampaignByNameAsync(purchase.CampaignName);
            if (campaign == null)
            {
                result.ValidationResult.Errors.Add(new ValidationFailure("CampaignId", "Campaign does not exist."));
            }
            else
            {
                result.CampaignId = campaign.Id;  // Set the CampaignId for later use

                // Check if campaign is still opened when purchase is entered
                if (DateTime.Today.Date < campaign.StartDate.Date || DateTime.Today.Date > campaign.EndDate.Date)
                {
                    result.ValidationResult.Errors.Add(new ValidationFailure("PurchaseId", "Campaign has expired."));
                }

                // Check agent's daily limit for specific campaign
                if (await _purchaseService.IsPurchaseDailyLimitForAgentMet(purchase.AgentId, campaign.Id, DateTime.Today))
                {
                    result.ValidationResult.Errors.Add(new ValidationFailure("PurchaseId", "Agent's daily limit for this campaign is met."));
                }

                // Check if there's already a purchase for this customer for the specific campaign
                if (await _purchaseService.IsPurchaseCreatedForCustomerInCampaign(campaign.Id, purchase.CustomerId))
                {
                    result.ValidationResult.Errors.Add(new ValidationFailure("PurchaseId", "Purchase for specific customer in this campaign already exists."));
                }
            }

            return result;
        }
    }
}
