using System.Net;
using rester.Abstractions;

namespace rester.Rest;

public class WebProxyFactory : IWebProxyFactory
{
    public WebProxy Create(string proxyAddress)
    {
        // https://user:password@example.com:2345
        var address = string.Empty;
        var user = string.Empty;
        var password = string.Empty;

        var separatorIndex = proxyAddress.IndexOf("@", StringComparison.InvariantCulture);
        var indexPrefix = proxyAddress.IndexOf("://", StringComparison.InvariantCulture);
        if (separatorIndex < 0)
        {
            if (indexPrefix > 0)
            {
                address = proxyAddress;
            }
            else
            {
                address = "https://" + proxyAddress;
            }
        }
        else
        {
            var prefix = indexPrefix > 0 ? proxyAddress.Substring(0, indexPrefix + 3) : "https://";
            address = prefix + proxyAddress.Substring(separatorIndex + 1);
            var credIndex = indexPrefix > 0 ? indexPrefix + 3 : 0;
            var credsSegments = proxyAddress.Substring(credIndex, separatorIndex - credIndex).Split(":");
            if (credsSegments.Length == 2)
            {
                user = credsSegments[0].Trim();
                password = credsSegments[1].Trim();
            }
        }
        
        var proxy = new WebProxy(new Uri(address));

        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
        {
            proxy.Credentials = new NetworkCredential(user, password);
        }

        return proxy;
    }
}