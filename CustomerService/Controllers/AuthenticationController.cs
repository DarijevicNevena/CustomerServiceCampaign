using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerService.Services.Contracts;
using CustomerService.Models;
using CustomerService.DTOs;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

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
