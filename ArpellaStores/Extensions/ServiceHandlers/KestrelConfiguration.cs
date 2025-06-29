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
                if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(certPwd)) throw new InvalidOperationException("Certificate path or password is missing.");

                var cert = new X509Certificate2(certPath, certPwd, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                listenOptions.UseHttps(cert);
                Console.WriteLine("[Kestrel] HTTPS endpoint configured successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Kestrel] Failed to load cert from '{certPath}'");
                Console.WriteLine($"[Kestrel] Password length: {certPwd?.Length}");
                Console.WriteLine($"[Kestrel] Exception: {ex}");
            }
        });

    }
}
