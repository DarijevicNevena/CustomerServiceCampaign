using CustomerService.Data.Base;
using CustomerService.Models;
using CustomerService.Services.Contracts;

namespace CustomerService.Services
{
    public class AgentService : IAgentService
    {
        private readonly IRepository<Agent> _agentRepository;

        public AgentService(IRepository<Agent> agentRepository)
        {
            _agentRepository = agentRepository;
        }

        public async Task<IEnumerable<Agent>> GetAllAgentsAsync()
        {
            return await _agentRepository.GetAllAsync();
        }

        public async Task<Agent> GetAgentByIdAsync(int id)
        {
            var agent = await _agentRepository.GetByIdAsync(id);
            if (agent == null)
            {
                throw new KeyNotFoundException($"Agent with ID {id} not found.");
            }
            return agent;
        }
    }
}
