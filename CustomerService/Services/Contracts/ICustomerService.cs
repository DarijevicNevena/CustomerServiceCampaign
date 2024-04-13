namespace CustomerService.Services.Contracts
{
    public interface ICustomerService
    {
        Task<bool> DoesCustomerExist(int customerId);
    }
}