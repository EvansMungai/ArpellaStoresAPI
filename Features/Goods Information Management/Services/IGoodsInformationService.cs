using ArpellaStores.Features.Goods_Information_Management.Models;

namespace ArpellaStores.Services;

public interface IGoodsInformationService
{
    Task<IResult> GetGoodsInformation();
    Task<IResult> GetGoodInformation(string itemCode);
    Task<IResult> CreateGoodsInformation(Goodsinfo goodsinfo);
    Task<IResult> UpdateGoodsInfo(Goodsinfo update, string itemCode);
    Task<IResult> RemoveGoodsInfo(string itemCode);
}
