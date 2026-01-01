using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using rester.Abstractions;

namespace rester.Rest;

public class AppHttpClientFactory : IAppHttpClientFactory
{
    private readonly IWebProxyFactory _webProxyFactory;

    public AppHttpClientFactory(IWebProxyFactory webProxyFactory)
    {
        _webProxyFactory = webProxyFactory;
    }
    
    public HttpClient Create(RestClientConfiguration config)
    {
        var handler = ConfigureHandler(config);
        
        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(config.MaxTime),
        };

        return client;
    }

    private HttpMessageHandler ConfigureHandler(RestClientConfiguration config)
    {
        var handler = CreateHttpClientHandler(config.Insecure, config.FollowRedirects, config.Proxy);
        if (!config.RetryCount.HasValue || config.RetryCount <= 0)
        {
            return handler;
        }
        
        var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new HttpRetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = config.RetryCount.Value
            })
            .Build();
        
        var resilienceHandler = new ResilienceHandler(retryPipeline)
        {
            InnerHandler = handler,
        };

        return resilienceHandler;
    }
    
    private HttpClientHandler CreateHttpClientHandler(bool insecure, bool followRedirects, string? proxy)
    {
        return new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = insecure ? (message, cert, chain, errors) => true : null,
            AllowAutoRedirect = followRedirects,
            AutomaticDecompression = DecompressionMethods.All,
            Proxy = string.IsNullOrEmpty(proxy) ? null : _webProxyFactory.Create(proxy)
        };
    }
}