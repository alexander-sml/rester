using System.CommandLine;
using rester.Abstractions;
using rester.Rest;

namespace rester;

public class CommandRunner : ICommandRunner
{
    private readonly IRestClient _restClient;
    
    public CommandRunner(IRestClient restClient)
    {
        _restClient = restClient;
    }

    public Task<int> ExecuteAsync(string[] args)
    {
        var cmd = Create();
        var parseResult = cmd.Parse(args);
        return parseResult.InvokeAsync();
    }

    private RootCommand Create()
    {
        return new CommandAppBuilder("A modern, cross-platform alternative to curl and wget")
            .AddArgument<string>(Commands.UrlName, "URL for request")
            .AddOption<HttpMethods>(Commands.MethodName, "-X", "Set HTTP method")
            .AddOptionAllowMultipleArguments<string>(Commands.HeadersName, "-H", "Add HTTP header (format: \"Key: Value\")")
            .AddOption<string>(Commands.DataName, "-d", "Request body data")
            .AddOption<string>(Commands.OutputFileName, "-o", "Write response to file")
            .AddOption<bool>(Commands.VerboseName, "-v", "Show detailed request/response info")
            .AddOptionWithDefault(Commands.ConnectTimeoutName, "Request timeout in seconds (default: 30)", 30)
            .AddOptionWithDefault(Commands.MaxTimeName, "Total timeout in seconds (default: 60)", 60)
            .AddOption<bool>(Commands.InsecureName, "-k", "Allow insecure SSL connections")
            .AddOption<bool>(Commands.FollowRedirectsName, "-L", "Follow redirects")
            .AddOption<string>(Commands.ProxyName, "Use proxy server")
            .AddOption<int>(Commands.RetryCountName, "-r", "Number of retries on failure")
            .SetAction(async (result, token) =>
            {
                var config = Map(result);

                try
                {
                    await _restClient.ExecuteAsync(config, token);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync($"Ошибка: {ex.Message}");
                    Environment.Exit(1);
                }
            })
            .Create();
    }
    
    private static RestClientConfiguration Map(ParseResult result)
    {
        return new RestClientConfiguration
        {
            Url = result.GetRequiredValue<string>(Commands.UrlName),
            Method = result.GetValue<HttpMethods>(Commands.MethodName),
            Headers = result.GetValue<string[]?>(Commands.HeadersName),
            Data = result.GetValue<string?>(Commands.DataName),
            OutputFile = result.GetValue<string?>(Commands.OutputFileName),
            Verbose = result.GetValue<bool>(Commands.VerboseName),
            ConnectTimeout = result.GetValue<int>(Commands.ConnectTimeoutName),
            MaxTime = result.GetValue<int>(Commands.MaxTimeName),
            Insecure = result.GetValue<bool>(Commands.InsecureName),
            FollowRedirects = result.GetValue<bool>(Commands.FollowRedirectsName),
            Proxy = result.GetValue<string>(Commands.ProxyName),
            RetryCount = result.GetValue<int?>(Commands.RetryCountName)
        };
    }
}