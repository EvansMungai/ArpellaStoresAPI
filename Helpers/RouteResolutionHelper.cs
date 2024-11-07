namespace ArpellaStores.Helpers
{
    public class RouteResolutionHelper : IRouteResolutionHelper
    {
        public void addMappings(WebApplication app)
        {
            app.MapGet("/", () => "Hello World!");
        }
    }
}
