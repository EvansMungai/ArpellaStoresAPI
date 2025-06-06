using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.FinalPriceManagement.Models;

namespace ArpellaStores.Features.FinalPriceManagement.Services;

public class FlashsaleService : IFlashsaleService
{
    private readonly ArpellaContext _context;
    public FlashsaleService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetFlashSales()
    {
        List<Flashsale> flashsales = _context.Flashsales.ToList();
        return flashsales == null || flashsales.Count == 0 ? Results.NotFound("No Flashsales Found") : Results.Ok(flashsales);
    }
    public async Task<IResult> GetFlashSale(int flashSaleId)
    {
        Flashsale? flashSale = _context.Flashsales.SingleOrDefault(f => f.FlashSaleId == flashSaleId);
        return flashSale == null ? Results.NotFound($"Flashsale with Id = {flashSaleId} was not found") : Results.Ok(flashSale);

    }
    public async Task<IResult> CreateFlashSale(Flashsale flashsale)
    {
        Flashsale flashsale1 = new Flashsale
        {
            ProductId = flashsale.ProductId,
            DiscountValue = flashsale.DiscountValue,
            StartTime = flashsale.StartTime,
            EndTime = flashsale.EndTime,
            IsActive = flashsale.IsActive
        };
        try
        {
            _context.Flashsales.Add(flashsale1);
            await _context.SaveChangesAsync();
            return Results.Ok(flashsale1);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
    public async Task<IResult> UpdateFlashSale(Flashsale update, int id)
    {
        Flashsale? retrievedCoupon = _context.Flashsales.SingleOrDefault(f => f.FlashSaleId.Equals(id));
        if (retrievedCoupon != null)
        {
            retrievedCoupon.ProductId = update.ProductId;
            retrievedCoupon.DiscountValue = update.DiscountValue;
            retrievedCoupon.StartTime = update.StartTime;
            retrievedCoupon.EndTime = update.EndTime;
            retrievedCoupon.IsActive = update.IsActive;
            try
            {
                _context.Flashsales.Update(retrievedCoupon);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedCoupon);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Flashsale of Id = {id} was not found");
        }
    }
    public async Task<IResult> RemoveFlashsale(int id)
    {
        Flashsale? coupon = _context.Flashsales.SingleOrDefault(f => f.FlashSaleId == id);
        if (coupon != null)
        {
            try
            {
                _context.Flashsales.Remove(coupon);
                await _context.SaveChangesAsync();
                return Results.Ok(coupon);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Flashsale of Id ={id} was not found");
        }
    }
}
