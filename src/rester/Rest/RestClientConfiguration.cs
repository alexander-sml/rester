namespace rester.Rest;

public class RestClientConfiguration
{
    public required string Url { get; set; }
    public HttpMethods Method { get; set; }
    public string[]? Headers { get; set; }
    public string? Data { get; set; }
    public string? OutputFile { get; set; }
    public bool Verbose { get; set; }
    public int ConnectTimeout { get; set; } = 30;
    public int MaxTime { get; set; } = 60;
    public bool Insecure { get; set; }
    public bool FollowRedirects { get; set; }
    public string? Proxy { get; set; }
    public int? RetryCount { get; set; }
}