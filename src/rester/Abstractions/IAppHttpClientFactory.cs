using rester.Rest;

namespace rester.Abstractions;

public interface IAppHttpClientFactory
{
    HttpClient Create(RestClientConfiguration config);
}