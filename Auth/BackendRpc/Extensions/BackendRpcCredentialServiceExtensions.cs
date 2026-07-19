using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Product.Common.Auth.BackendRpc.Credentials;
using Product.Common.Auth.BackendRpc.Options;

namespace Product.Common.Auth.BackendRpc.Extensions;

public static class BackendRpcCredentialServiceExtensions
{
    public static IServiceCollection AddBackendRpcCredentialAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsSectionName,
        string hmacKeyConfigPath,
        Action<BackendRpcAuthOptions> configureHeaders)
    {
        services.AddOptions<BackendRpcCredentialOptions>()
            .Bind(configuration.GetSection(optionsSectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<BackendRpcCredentialOptions>>(sp =>
            new BackendRpcCredentialOptionsValidator(
                sp.GetRequiredService<IHostEnvironment>(),
                hmacKeyConfigPath));

        services.AddSingleton<IBackendRpcCredentialSigner, HmacBackendRpcCredentialSigner>();
        services.AddSingleton<BackendRpcCredentialGenerator>();
        services.AddSingleton<BackendRpcCredentialIssuer>();
        services.AddBackendRpcAuth(configureHeaders);
        return services;
    }
}
