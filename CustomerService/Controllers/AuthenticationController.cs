using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CustomerService.Services.Contracts;
using CustomerService.Models.ModelDto;

namespace CustomerService.Controllers
{
    /// <summary>
    /// Handles authentication requests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticates an agent and returns a JWT if credentials are valid.
        /// </summary>
        /// <param name="loginDto">Dto containing credentials.</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        /// <response code="200">Returns created JWT token.</response>
        /// <response code="400">If the model state is invalid or the login credentials are incorrect.</response>
        /// <response code="401">Returned when authentication fails due to invalid credentials.</response>
        /// <response code="404">Returned when no agent matches the provided email.</response>
        /// <response code="500">Returned if there is an error during the process of generating the token.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var agent = await _authenticationService.AuthenticateAgent(loginDto.Email, loginDto.Password);
                if (agent == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                var token = _authenticationService.GenerateJwtToken(agent);
                return Ok(new { Token = token });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Agent not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
