using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;

namespace ArpellaStores.Extensions.ServiceHandlers;

public static class KestrelConfiguration
{
    public static void ConfigureCustomHttps(this KestrelServerOptions options)
    {
        var certPath = Environment.GetEnvironmentVariable("HTTPS_PFX_PATH");
        var certPwd = Environment.GetEnvironmentVariable("HTTPS_PFX_PASSWORD");

        options.ListenAnyIP(8081, listenOptions =>
        {
            try
            {
                var cert = new X509Certificate2(certPath, certPwd, X509KeyStorageFlags.Exportable);
                listenOptions.UseHttps(cert);
            }
            catch (Exception ex) { Console.WriteLine($"Failed to load certificate: {ex.Message}"); }
        });

    }
}
