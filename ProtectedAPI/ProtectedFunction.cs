using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using ProtectedAPI.Service.Interfaces;
using ProtectedAPI.Service.Options;

namespace ProtectedAPI
{
    public class ProtectedFunction(
        IJwtValidatorService jwtValidatorService, IOptions<AuthorizationScope> opt)
    {
        private readonly AuthorizationScope _protectedScope = opt.Value;
        
        [Function(nameof(Protected))]
        public async Task<HttpResponseData> Protected(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] 
            HttpRequestData req, ExecutionContext executionContext)
        {
            var acceptedScopes = new[] { _protectedScope.TestScope };
            var userId = await jwtValidatorService.ValidateTokenAndCheckScopes(req, acceptedScopes);
                
            var response = req.CreateResponse();
            if (userId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}