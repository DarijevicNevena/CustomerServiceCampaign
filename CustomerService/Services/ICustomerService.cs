namespace CustomerService.Services
{
    public interface ICustomerService
    {
        Task<bool> DoesCustomerExist(int customerId);
    }
}