using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;

namespace ArpellaStores.Extensions.ServiceHandlers;

public static class KestrelConfiguration
{
    public static void ConfigureCustomHttps(this KestrelServerOptions options)
    {
        var certPath = Environment.GetEnvironmentVariable("HTTPS_PFX_PATH");

        options.ListenAnyIP(8081, listenOptions =>
        {
            var cert = new X509Certificate2(certPath, "", X509KeyStorageFlags.Exportable);
            listenOptions.UseHttps(cert);
        });

    }
}
