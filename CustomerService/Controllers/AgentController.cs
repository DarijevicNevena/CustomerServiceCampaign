using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgentReadDto>>> GetAgents()
        {
            var agents = await _agentService.GetAllAgentsAsync();
            return Ok(agents);
        }

        [HttpGet("{id:int}", Name = "GetAgentbyId")]
        public async Task<ActionResult<AgentReadDto>> GetAgentById(int id)
        {
            try
            {
                var agent = await _agentService.GetAgentByIdAsync(id);
                return Ok(agent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

    }
}
