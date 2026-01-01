using System.Net;

namespace rester.Abstractions;

public interface IWebProxyFactory
{
    WebProxy Create(string proxyAddress);
}