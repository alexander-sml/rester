using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Moq;
using rester.Abstractions;
using rester.Rest;
using Xunit;

namespace rester.Tests;

public class AppHttpClientFactoryTests
{
    private readonly Mock<IWebProxyFactory> _webProxyFactoryMock;
    private readonly AppHttpClientFactory _factory;

    public AppHttpClientFactoryTests()
    {
        _webProxyFactoryMock = new Mock<IWebProxyFactory>();
        _factory = new AppHttpClientFactory(_webProxyFactoryMock.Object);
    }

    [Fact]
    public void Create_WithValidConfig_ReturnsHttpClientWithCorrectTimeout()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            MaxTime = 90
        };

        // Act
        var client = _factory.Create(config);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(90), client.Timeout);
    }

    [Fact]
    public void Create_WithInsecureTrue_SetsServerCertificateValidationCallback()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            Insecure = true
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandler<HttpClientHandler>(client);

        // Assert
        Assert.NotNull(handler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public void Create_WithInsecureFalse_DoesNotSetServerCertificateValidationCallback()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            Insecure = false
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandler<HttpClientHandler>(client);

        // Assert
        Assert.Null(handler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public void Create_WithFollowRedirectsTrue_SetsAllowAutoRedirect()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            FollowRedirects = true
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandler<HttpClientHandler>(client);

        // Assert
        Assert.True(handler.AllowAutoRedirect);
    }

    [Fact]
    public void Create_WithFollowRedirectsFalse_SetsAllowAutoRedirect()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            FollowRedirects = false
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandler<HttpClientHandler>(client);

        // Assert
        Assert.False(handler.AllowAutoRedirect);
    }

    [Fact]
    public void Create_WithProxy_CallsWebProxyFactory()
    {
        // Arrange
        var proxyAddress = "http://proxy.example.com:8080";
        var expectedProxy = new WebProxy(proxyAddress);
        _webProxyFactoryMock.Setup(x => x.Create(proxyAddress)).Returns(expectedProxy);

        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            Proxy = proxyAddress
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandler<HttpClientHandler>(client);

        // Assert
        _webProxyFactoryMock.Verify(x => x.Create(proxyAddress), Times.Once);
        Assert.Equal(expectedProxy, handler.Proxy);
    }

    [Fact]
    public void Create_WithoutProxy_DoesNotCallWebProxyFactory()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            Proxy = null
        };

        // Act
        var client = _factory.Create(config);

        // Assert
        _webProxyFactoryMock.Verify(x => x.Create(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Create_WithEmptyProxy_DoesNotCallWebProxyFactory()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            Proxy = string.Empty
        };

        // Act
        var client = _factory.Create(config);

        // Assert
        _webProxyFactoryMock.Verify(x => x.Create(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Create_WithRetryCount_AddsResilienceHandler()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            RetryCount = 3
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandlerInner(client);

        // Assert
        Assert.IsType<ResilienceHandler>(handler);
    }

    [Fact]
    public void Create_WithoutRetryCount_DoesNotAddResilienceHandler()
    {
        // Arrange
        var config = new RestClientConfiguration { Url = "https://api.example.com" };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandlerInner(client);

        // Assert
        Assert.IsType<HttpClientHandler>(handler);
    }

    [Fact]
    public void Create_WithRetryCountNull_DoesNotAddResilienceHandler()
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            RetryCount = null
        };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandlerInner(client);

        // Assert
        Assert.IsType<HttpClientHandler>(handler);
    }

    [Fact]
    public void Create_Always_SetsAutomaticDecompression()
    {
        // Arrange
        var config = new RestClientConfiguration { Url = "https://api.example.com" };

        // Act
        var client = _factory.Create(config);
        var handler = GetHandler<HttpClientHandler>(client);

        // Assert
        Assert.Equal(DecompressionMethods.All, handler.AutomaticDecompression);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(120)]
    public void Create_WithDifferentMaxTimeValues_SetsCorrectTimeout(int maxTime)
    {
        // Arrange
        var config = new RestClientConfiguration
        {
            Url = "https://api.example.com",
            MaxTime = maxTime
        };

        // Act
        var client = _factory.Create(config);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(maxTime), client.Timeout);
    }

    private T GetHandler<T>(HttpClient client) where T : HttpMessageHandler
    {
        var handler = GetHandlerInner(client);
        if (handler is ResilienceHandler resilienceHandler)
        {
            return resilienceHandler.InnerHandler as T;
        }

        return handler as T;
    }
    
    private HttpMessageHandler? GetHandlerInner(HttpClient client)
    {
        var handlerField = typeof(HttpMessageInvoker).GetField("_handler",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return handlerField!.GetValue(client) as HttpMessageHandler;
    }
}
