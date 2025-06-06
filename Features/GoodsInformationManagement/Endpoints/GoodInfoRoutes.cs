using ArpellaStores.Extensions;
using ArpellaStores.Features.GoodsInformationManagement.Models;
using ArpellaStores.Features.GoodsInformationManagement.Services;

namespace ArpellaStores.Features.GoodsInformationManagement.Endpoints;

public class GoodInfoRoutes : IRouteRegistrar
{
    private readonly IGoodsInformationService _goodsInformationService;
    public GoodInfoRoutes(IGoodsInformationService goodsInformationService)
    {
        _goodsInformationService = goodsInformationService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapGoodInfoRoutes(app);
    }
    public void MapGoodInfoRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Goods Info");
        app.MapGet("/goodsinfo", () => this._goodsInformationService.GetGoodsInformation()).Produces(200).Produces(404).Produces<List<Goodsinfo>>();
        app.MapGet("/goodsinfo/{itemCode}", (string itemCode) => this._goodsInformationService.GetGoodInformation(itemCode)).Produces(200).Produces(404).Produces<Goodsinfo>();
        app.MapPost("/goodsinfo", (Goodsinfo goodsinfo) => this._goodsInformationService.CreateGoodsInformation(goodsinfo)).Produces<Goodsinfo>();
        app.MapPut("/goodsinfo/{itemCode}", (Goodsinfo goodsinfo, string itemCode) => this._goodsInformationService.UpdateGoodsInfo(goodsinfo, itemCode)).Produces<Goodsinfo>();
        app.MapDelete("/goodsinfo/{itemCode}", (string itemCode) => this._goodsInformationService.RemoveGoodsInfo(itemCode)).Produces(200).Produces(404).Produces<Goodsinfo>();
    }
}
