using ArpellaStores.Extensions;
using ArpellaStores.Features.GoodsInformationManagement.Models;
using ArpellaStores.Features.GoodsInformationManagement.Services;

namespace ArpellaStores.Features.GoodsInformationManagement.Endpoints;

public class GoodsInfoHandler : IHandler
{
    public static string RouteName => "Goods Information Management";
    private readonly IGoodsInformationService _goodsInformationService;
    public GoodsInfoHandler(IGoodsInformationService goodsInformationService)
    {
        _goodsInformationService = goodsInformationService;
    }

    public Task<IResult> GetGoodsInformation() => _goodsInformationService.GetGoodsInformation();
    public Task<IResult> GetGoodInformation(string itemCode) => _goodsInformationService.GetGoodInformation(itemCode);
    public Task<IResult> CreateGoodsInformation(Goodsinfo goodsinfo) => _goodsInformationService.CreateGoodsInformation(goodsinfo);
    public Task<IResult> UpdateGoodsInformation(Goodsinfo update, string itemCode) => _goodsInformationService.UpdateGoodsInfo(update, itemCode);
    public Task<IResult> RemoveGoodsInformation(string itemCode) => _goodsInformationService.RemoveGoodsInfo(itemCode);
}
