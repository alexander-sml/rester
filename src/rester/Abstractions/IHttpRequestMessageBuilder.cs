namespace rester.Abstractions;

public interface IHttpRequestMessageBuilder
{
    IHttpRequestMessageBuilder AddContent(string? data, string[]? headers);
    IHttpRequestMessageBuilder AddHeaders(string[]? headers);
    HttpRequestMessage CreateHttpRequestMessage();
}