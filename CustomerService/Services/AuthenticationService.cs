using CustomerService.Models;
using CustomerService.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CustomerService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAgentService _agentService;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IAgentService agentService, IConfiguration configuration)
        {
            _agentService = agentService;
            _configuration = configuration;
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

            var expiry = DateTime.UtcNow.AddHours(1);
            var payload = new JwtPayload
        {
            {"email", agent.Email},
            {"exp", (int)expiry.Subtract(new DateTime(1970, 1, 1)).TotalSeconds},
            {"iss", _configuration["JWT_ISSUER"] ?? "https://localhost:7264"},
            {"aud", _configuration["JWT_AUDIENCE"] ?? "https://localhost:7264"}
        };

            var token = new JwtSecurityToken(new JwtHeader(credentials), payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, string? storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
