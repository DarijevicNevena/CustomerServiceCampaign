using CustomerService.Models;

namespace CustomerService.Services.Contracts
{
    public interface IAgentService
    {
        Task<Agent> AuthenticateAgent(string email, string password);
        Task<IEnumerable<Agent>> GetAllAgentsAsync();
        Task<Agent> GetAgentByIdAsync(int id);
    }
}
