using Microsoft.Extensions.DependencyInjection;
using ProtectedAPI.Service;
using ProtectedAPI.Service.Interfaces;
using ProtectedAPI.Service.Options;

namespace ProtectedAPI
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services, 
            Action<TokenValidationOptions> tokenValidationOptions, 
            Action<AuthorizationScope> authorizationScope)
        {
            services.ConfigureServiceOptions<TokenValidationOptions>((_, opt) => tokenValidationOptions(opt));
            services.ConfigureServiceOptions<AuthorizationScope>((_, opt) => authorizationScope(opt));
            services.AddSingleton<IJwtValidatorService, JwtValidatorService>();
        }
        
        private static void ConfigureServiceOptions<TOptions>(
            this IServiceCollection services,
            Action<IServiceProvider, TOptions> configure)
            where TOptions : class
        {
            services
                .AddOptions<TOptions>()
                .Configure<IServiceProvider>((options, resolver) => configure(resolver, options));
        }
    }
}