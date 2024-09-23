using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ProtectedAPI;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        var configuration = builder.Build();
        var token = new DefaultAzureCredential();
        var appConfigUrl = configuration["app_config_url"] ?? string.Empty;
        
        builder.AddAzureAppConfiguration(config =>
        {
            config.Connect(new Uri(appConfigUrl), token);
            config.ConfigureKeyVault(kv => kv.SetCredential(token));
        });
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        
        services.RegisterServices(tokenOptions =>
        {
            tokenOptions.MetadataUrl = configuration["metadata_url_in_app_configuration"] ?? string.Empty;
            tokenOptions.Issuer = configuration["issuer_in_app_configuration"] ?? string.Empty;
            tokenOptions.ClientId = configuration["client_id_url_in_app_configuration"] ?? string.Empty;
        },
        authScope =>
        {
            authScope.TestScope = configuration["scope_url_in_app_configuration"] ?? string.Empty;
        });
    })
    .Build();

host.Run();