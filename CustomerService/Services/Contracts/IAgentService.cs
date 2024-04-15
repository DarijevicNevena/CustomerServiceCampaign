using CustomerService.Models;
using CustomerService.Models.ModelDto;

namespace CustomerService.Services.Contracts
{
    public interface IAgentService
    {
        Task<IEnumerable<AgentReadDto>> GetAllAgentsAsync();
        Task<AgentReadDto?> GetAgentByIdAsync(int id);
        Task<Agent> GetAgentByEmailAsync(string email);
    }
}
