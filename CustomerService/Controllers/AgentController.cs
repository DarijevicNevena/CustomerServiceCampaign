using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


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
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            var agents = await _agentService.GetAllAgentsAsync();
            return Ok(agents);
        }

        [HttpGet("{id:int}", Name = "GetAgentbyId")]
        public async Task<ActionResult<Agent>> GetAgentById(int id)
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
