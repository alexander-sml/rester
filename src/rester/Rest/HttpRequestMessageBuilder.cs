using System.Net.Http.Headers;
using System.Text;
using rester.Abstractions;

namespace rester.Rest;

public class HttpRequestMessageBuilder : IHttpRequestMessageBuilder
{
    private const string DefaultContentType = "application/json";
    private const string ContentTypeHeader = "Content-Type";
    
    private readonly HttpRequestMessage _request;

    private HttpRequestMessageBuilder(string method, string url)
    {
        _request = new HttpRequestMessage()
        {
            Method = new HttpMethod(method),
            RequestUri = new Uri(url)
        };
    }

    public static IHttpRequestMessageBuilder Build(string method, string url)
    {
        return new HttpRequestMessageBuilder(method, url);
    }

    public IHttpRequestMessageBuilder AddContent(string? data, string[]? headers)
    {
        if (!string.IsNullOrEmpty(data) && headers?.Length > 0)
        {
            var contentTypeHeader = headers.FirstOrDefault(h =>
                h.StartsWith(ContentTypeHeader, StringComparison.InvariantCultureIgnoreCase));
            var value = !string.IsNullOrEmpty(contentTypeHeader)
                ? ParseValue(contentTypeHeader) ?? DefaultContentType
                : DefaultContentType;
            _request.Content = new StringContent(data, Encoding.UTF8, value);
        }

        return this;
    }

    public IHttpRequestMessageBuilder AddHeaders(string[]? headers)
    {
        if (headers?.Length > 0)
        {
            foreach (var header in headers)
            {
                var parts = header.Split(':', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (string.Equals(key, "Authorization", StringComparison.OrdinalIgnoreCase)
                        && value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        _request.Headers.Authorization = new AuthenticationHeaderValue(
                            "Bearer", value[7..].Trim());
                    }
                    else
                    {
                        _request.Headers.TryAddWithoutValidation(key, value);
                    }
                }
            }
        }

        return this;
    }

    public HttpRequestMessage CreateHttpRequestMessage()
    {
        return _request;
    }

    private string? ParseValue(string value)
    {
        var segments = value.Split(":");
        if (segments.Length != 2)
        {
            return null;
        }

        return segments[1].Trim();
    }
}