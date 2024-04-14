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
        public async Task<Agent> AuthenticateAgent(string email, string password)
        {

            var foundAgents = await _agentRepository.SearchAsync(a => a.Email == email);


            // Check the provided password against the stored hash
            if (foundAgents.Count() == 1 && VerifyPasswordHash(password, foundAgents.Single().PasswordHash))
            {
                return foundAgents.Single();
            }

            return null;
        }

        private bool VerifyPasswordHash(string password, string? storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
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
