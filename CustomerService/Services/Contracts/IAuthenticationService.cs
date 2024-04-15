using CustomerService.Models;
using CustomerService.Models.ModelDto;

namespace CustomerService.Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<Agent> AuthenticateAgent(string email, string password);
        string GenerateJwtToken(Agent agent);
    }
}
