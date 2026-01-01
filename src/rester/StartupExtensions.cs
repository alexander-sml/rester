using Microsoft.Extensions.DependencyInjection;
using rester.Abstractions;
using rester.Rest;

namespace rester;

public static class StartupExtensions
{
    public static ServiceCollection Configure(this ServiceCollection services)
    {
        services.AddSingleton<ICommandRunner, CommandRunner>();
        services.AddSingleton<ICommandAppBuilder, CommandAppBuilder>();
        services.AddSingleton<IRestClient, RestClient>();
        services.AddSingleton<IAppHttpClientFactory, AppHttpClientFactory>();
        services.AddSingleton<IWebProxyFactory, WebProxyFactory>();

        return services;
    }
}