using ArpellaStores.Features.Final_Price_Management.Models;

namespace ArpellaStores.Features.Final_Price_Management;

public interface IFlashsaleService
{
    Task<IResult> GetFlashSales();
    Task<IResult> GetFlashSale(int flashSaleId);
    Task<IResult> CreateFlashSale(Flashsale flashsale);
    Task<IResult> UpdateFlashSale(Flashsale update, int id);
    Task<IResult> RemoveFlashsale(int id);
}
