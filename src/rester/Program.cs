using Microsoft.Extensions.DependencyInjection;
using rester.Abstractions;

namespace rester;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .Configure()
            .BuildServiceProvider();

        var runner = serviceProvider.GetRequiredService<ICommandRunner>();

        return await runner.ExecuteAsync(args);
    }
}