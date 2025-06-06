using System.Reflection;

namespace ArpellaStores.Extensions;

public static class FeatureServiceRegistration
{
    public static void RegisterFeatureServices(this IServiceCollection services)
    {
        var methodName = "RegisterApplicationServices";
        var featureAssemblies = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetMethods().Any(m => m.Name == methodName && m.IsStatic));

        foreach (var feature in featureAssemblies)
        {
            var method = feature.GetMethod(methodName);
            method?.Invoke(null, new object[] { services });
        }
    }
}
