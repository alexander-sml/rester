using rester.Abstractions;

namespace rester.Rest;

public class RestClient : IRestClient
{
    private readonly IAppHttpClientFactory _clientFactory;

    public RestClient(
        IAppHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task ExecuteAsync(RestClientConfiguration config, CancellationToken token)
    {
        using var client = _clientFactory.Create(config);

        using var request = HttpRequestMessageBuilder
            .Build(config.Method.ToString(), config.Url)
            .AddContent(config.Data, config.Headers)
            .AddHeaders(config.Headers)
            .CreateHttpRequestMessage();

        if (config.Verbose)
        {
            LogRequestParameters(request);
        }

        var response = await client.SendAsync(request, token);

        if (config.Verbose)
        {
            LogResponseParameters(response);
        }

        var responseBody = await response.Content.ReadAsStringAsync(token);

        if (!string.IsNullOrEmpty(config.OutputFile))
        {
            await File.WriteAllTextAsync(config.OutputFile, responseBody, token);
            if (config.Verbose)
            {
                Log($"* Ответ сохранен в файл: {config.OutputFile}");
            }
        }
        else
        {
            Log(responseBody);
        }
    }

    private static void LogResponseParameters(HttpResponseMessage response)
    {
        Log($"< HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}");
        foreach (var header in response.Headers)
        {
            Log($"< {header.Key}: {string.Join(", ", header.Value)}");
        }

        foreach (var header in response.Content.Headers)
        {
            Log($"< {header.Key}: {string.Join(", ", header.Value)}");
        }

        Log("<");
    }

    private static void LogRequestParameters(HttpRequestMessage request)
    {
        Log($"> {request.Method} {request.RequestUri} HTTP/1.1");
        Log($"> Host: {request.RequestUri!.Host}");
        foreach (var header in request.Headers)
        {
            Log($"> {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (request.Content != null)
        {
            foreach (var header in request.Content.Headers)
            {
                Log($"> {header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        Log(">");
    }

    private static void Log(string message)
    {
        Console.WriteLine(message);
    }
}