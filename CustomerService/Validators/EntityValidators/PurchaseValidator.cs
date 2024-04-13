using CustomerService.Models;
using CustomerService.Services;
using FluentValidation;
using FluentValidation.Results;

namespace CustomerService.Validators.EntityValidators
{
    public class PurchaseValidator : AbstractValidator<Purchase>, IValidator<Purchase>
    {
        private readonly IAgentService _agentService;
        private readonly IPurchaseService _purchaseService;
        private readonly ICampaignService _campaignService;

        public PurchaseValidator(
        IAgentService agentService,
        IPurchaseService purchaseService,
        ICampaignService campaignService) : this()
        {
            _agentService = agentService;
            _purchaseService = purchaseService;
            _campaignService = campaignService;

            SetupRules();
        }

        public PurchaseValidator()
        {
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(purchase => purchase.AgentId)
                .NotEmpty().WithMessage("Agent ID is required.");

            RuleFor(purchase => purchase.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.");

            RuleFor(purchase => purchase.CampaignId)
                .NotEmpty().WithMessage("Campaign ID is required.");

            RuleFor(purchase => purchase.Price)
                .NotEmpty().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(purchase => purchase.Discount)
                .NotEmpty().WithMessage("Discount is required.")
                .GreaterThan(0).WithMessage("Discount must be greater than 0.");

            RuleFor(purchase => purchase.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(purchase => purchase.Discount).LessThan(purchase => purchase.Price).WithMessage("Discount must be less than Price.");
        }

        public async Task<ValidationResult> ValidateAsync(Purchase purchase)
        {
            var validationResult = new ValidationResult();

            //Check if agent exists
            var agent = await _agentService.GetAgentByIdAsync(purchase.AgentId);
            if (agent == null)
            {
                validationResult.Errors.Add(new ValidationFailure("AgentId", "Agent does not exist."));
            }

            //Check if campaign exists
            var campaign = await _campaignService.GetCampaignByIdAsync(purchase.CampaignId);
            if (campaign == null)
            {
                validationResult.Errors.Add(new ValidationFailure("CampaignId", "Campaign does not exist."));
            }

            //Check if campaign is still opened when purchase is entered
            if (purchase.Date.Date < campaign?.StartDate.Date || purchase.Date.Date > campaign?.EndDate.Date)
            {
                validationResult.Errors.Add(new ValidationFailure("PurchaseId", "Campaign has expired."));
            }

            //Check agent's daily limit for specific campaign
            if (await _purchaseService.IsPurchaseDailyLimitForAgentMet(purchase.AgentId, purchase.CampaignId, DateTime.Today))
            {
                validationResult.Errors.Add(new ValidationFailure("PurchaseId", "Agent's daily limit for this campaign is met."));
            }

            //Check is there already a purchase for this customer for specific campaign
            if (await _purchaseService.IsPurchaseCreatedForCustomerInCampaign(purchase.CampaignId, purchase.CustomerId))
            {
                validationResult.Errors.Add(new ValidationFailure("PurchaseId", "Purchase for specific customer in this campaign already exists."));
            }


            //Check agent's daily limit

            return validationResult;
        }
    }
}
