using rester.Rest;

namespace rester.Abstractions;

public interface IRestClient
{
    Task ExecuteAsync(RestClientConfiguration config, CancellationToken token);
}