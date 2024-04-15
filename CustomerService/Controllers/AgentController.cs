using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    /// <summary>
    /// Manages agent operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        /// <summary>
        /// Returns a list of all agents.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /api/agent
        /// </remarks>
        /// <returns>A list of agent dtos</returns>
        /// <response code="200">Returns the list of agents</response>
        /// <response code="500">If there is an internal server error</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AgentReadDto>>> GetAgents()
        {
            var agents = await _agentService.GetAllAgentsAsync();
            return Ok(agents);
        }

        /// <summary>
        ///Returns a specific agent by their ID.
        /// </summary>
        /// <param name="id">The ID of the agent to return.</param>
        /// <remarks>
        /// Sample request:
        /// GET /api/agent/5
        /// </remarks>
        /// <returns>Agent Dto</returns>
        /// <response code="200">Returns the requested agent</response>
        /// <response code="404">If no agent is found with the provided ID</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet("{id:int}", Name = "GetAgentById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
