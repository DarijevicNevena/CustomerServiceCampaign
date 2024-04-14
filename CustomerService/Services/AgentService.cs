using CustomerService.Data.Base;
using CustomerService.Models;
using CustomerService.Services.Contracts;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Agent> GetAgentByEmailAsync(string email)
        {
            var agent = await _agentRepository.SearchAsync(a => a.Email == email);
            if (agent.Count() == 0)
            {
                throw new KeyNotFoundException($"Agent with email {email} not found.");
            }
            return agent.Single();
        }
    }
}
