using AutoMapper;
using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Services
{
    public class AgentService : IAgentService
    {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IMapper _mapper;

        public AgentService(IRepository<Agent> agentRepository, IMapper mapper)
        {
            _agentRepository = agentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AgentReadDto>> GetAllAgentsAsync()
        {
            var agents = await _agentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AgentReadDto>>(agents);
        }

        public async Task<AgentReadDto?> GetAgentByIdAsync(int id)
        {
            var agent = await _agentRepository.GetByIdAsync(id);
            if (agent == null)
            {
                throw new KeyNotFoundException($"Agent with ID {id} not found.");
            }
            return _mapper.Map<AgentReadDto>(agent);
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
