﻿using ArpellaStores.Features.GoodsInformationManagement.Models;

namespace ArpellaStores.Features.GoodsInformationManagement.Services;

public interface IGoodsInformationService
{
    Task<IResult> GetGoodsInformation();
    Task<IResult> GetGoodInformation(string itemCode);
    Task<IResult> CreateGoodsInformation(Goodsinfo goodsinfo);
    Task<IResult> UpdateGoodsInfo(Goodsinfo update, string itemCode);
    Task<IResult> RemoveGoodsInfo(string itemCode);
}
