using CustomerService.Models;
using CustomerService.Models.ModelDto;

namespace CustomerService.Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<AgentReadDto> AuthenticateAgent(string email, string password);
        string GenerateJwtToken(AgentReadDto agent);
    }
}
