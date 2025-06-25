using ArpellaStores.Extensions;
using ArpellaStores.Features.GoodsInformationManagement.Models;

namespace ArpellaStores.Features.GoodsInformationManagement.Endpoints;

public class GoodInfoRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapGoodInfoRoutes(app);
    }
    public void MapGoodInfoRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Goods Info");
        app.MapGet("/goodsinfo", (GoodsInfoHandler handler) => handler.GetGoodsInformation()).Produces(200).Produces(404).Produces<List<Goodsinfo>>();
        app.MapGet("/goodsinfo/{itemCode}", (GoodsInfoHandler handler, string itemCode) => handler.GetGoodInformation(itemCode)).Produces(200).Produces(404).Produces<Goodsinfo>();
        app.MapPost("/goodsinfo", (GoodsInfoHandler handler, Goodsinfo goodsinfo) => handler.CreateGoodsInformation(goodsinfo)).Produces<Goodsinfo>();
        app.MapPut("/goodsinfo/{itemCode}", (GoodsInfoHandler handler, Goodsinfo goodsinfo, string itemCode) => handler.UpdateGoodsInformation(goodsinfo, itemCode)).Produces<Goodsinfo>();
        app.MapDelete("/goodsinfo/{itemCode}", (GoodsInfoHandler handler, string itemCode) => handler.RemoveGoodsInformation(itemCode)).Produces(200).Produces(404).Produces<Goodsinfo>();
    }
}
