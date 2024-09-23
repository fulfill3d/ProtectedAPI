using Microsoft.Azure.Functions.Worker.Http;

namespace ProtectedAPI.Service.Interfaces
{
    public interface IJwtValidatorService
    {
        Task<string?> AuthenticateAndAuthorize(HttpRequestData req, string[] acceptedScopes);
    }
}