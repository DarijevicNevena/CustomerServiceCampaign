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
        private readonly IAgentService _agentService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAgentService agentService, IConfiguration configuration)
        {
            _agentService = agentService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Return bad request with model state errors
                return BadRequest(ModelState);
            }

            var agent = await _agentService.AuthenticateAgent(loginDto.Email, loginDto.Password);
            if (agent == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = GenerateJwtToken(agent);
            return Ok(new { Token = token });
        }


        private string GenerateJwtToken(Agent agent)
        {
            //Specify signing algorithm
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])), SecurityAlgorithms.HmacSha256);
            //Create header
            var header = new JwtHeader(credentials);
            //Define token expiry time
            DateTime expiry = DateTime.UtcNow.AddHours(1);
            int ts = (int)(expiry - new DateTime(1970, 1, 1)).TotalSeconds;
            //Define payload
            var payload = new JwtPayload
            {
                {"email", agent.Email},
                {"exp", ts},
                {"iss", "https://localhost:7264"},
                {"aud","https://localhost:7264"}

            };
            //Create token in string format
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }
    }

}
