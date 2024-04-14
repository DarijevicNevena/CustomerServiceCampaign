using CustomerService.Models;

namespace CustomerService.Services.Contracts
{
    public interface IAgentService
    {
        Task<IEnumerable<Agent>> GetAllAgentsAsync();
        Task<Agent> GetAgentByIdAsync(int id);
        Task<Agent> GetAgentByEmailAsync(string email);
    }
}
