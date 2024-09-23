using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using ProtectedAPI.Service.Interfaces;
using ProtectedAPI.Service.Options;
using HttpRequestData = Microsoft.Azure.Functions.Worker.Http.HttpRequestData;

namespace ProtectedAPI.Service
{
    public class JwtValidatorService(IOptions<TokenValidationOptions> opt) : IJwtValidatorService
    {
        private const string ScopeClaimType1 = "http://schemas.microsoft.com/identity/claims/scope";
        private const string ScopeClaimType2 = "scp";
        private const string OidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private const string AuthKey = "Authorization";
        private const string TokenHeaderIdentifier = "Bearer ";
        private readonly string _clientId = opt.Value.ClientId;
        private readonly string _issuer = opt.Value.Issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager = 
            new (opt.Value.MetadataUrl, new OpenIdConnectConfigurationRetriever());

        public async Task<string?> AuthenticateAndAuthorize(HttpRequestData req, string[] acceptedScopes)
        {
            if (!req.Headers.TryGetValues(AuthKey, out var authHeaders))
            {
                return null;
            }

            var authHeader = authHeaders.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(TokenHeaderIdentifier))
            {
                return null;
            }

            var bearerToken = authHeader.Substring(TokenHeaderIdentifier.Length);

            try
            {
                var openIdConfig = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _clientId,
                    ValidateLifetime = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(bearerToken, validationParameters, out SecurityToken validatedToken);

                var scopes = principal.Claims
                    .Where(c => c.Type == ScopeClaimType1 || c.Type == ScopeClaimType2)
                    .SelectMany(c => c.Value.Split(' '))
                    .ToList();

                if (!scopes.Any(scope => acceptedScopes.Contains(scope)))
                {
                    return null;
                }

                var oid = principal.Claims.FirstOrDefault(c => c.Type == OidClaimType)?.Value;

                return oid;
            }
            catch
            {
                return null;
            }
        }
    }
}
