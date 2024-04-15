using AutoMapper;
using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAgentService _agentService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthenticationService(IAgentService agentService, IConfiguration configuration, IMapper mapper)
        {
            _agentService = agentService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<Agent> AuthenticateAgent(string email, string password)
        {
            var agent = await _agentService.GetAgentByEmailAsync(email);

            if (agent != null && VerifyPasswordHash(password, agent.PasswordHash))
            {
                return agent;
            }

            return null;
        }

        public string GenerateJwtToken(Agent agent)
        {
            var keyString = _configuration["JWT_KEY"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT Key is not set in environment variables.");

            var key = Encoding.UTF8.GetBytes(keyString);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, agent.Email),
        new Claim(ClaimTypes.NameIdentifier, agent.Id.ToString())
    };

            var expiry = DateTime.UtcNow.AddHours(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                SigningCredentials = credentials,
                Issuer = _configuration["JWT_ISSUER"],
                Audience = _configuration["JWT_AUDIENCE"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, string? storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
