using CustomerService.Models;

namespace CustomerService.Services
{
    public interface IAgentService
    {
        Task<IEnumerable<Agent>> GetAllAgentsAsync();
        Task<Agent> GetAgentByIdAsync(int id);
    }
}
