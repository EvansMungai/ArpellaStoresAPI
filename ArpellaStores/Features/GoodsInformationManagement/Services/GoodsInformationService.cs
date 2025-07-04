using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.GoodsInformationManagement.Models;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;


namespace ArpellaStores.Features.GoodsInformationManagement.Services;

public class GoodsInformationService : IGoodsInformationService
{
    private readonly ArpellaContext _context;
    public GoodsInformationService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetGoodsInformation()
    {
        var goodsInfo = await _context.Goodsinfos.Select(g => new { g.ItemCode, g.ItemDescription, g.UnitMeasure, g.TaxRate }).AsNoTracking().ToListAsync();
        return goodsInfo == null || goodsInfo.Count == 0 ? Results.NotFound("No Goods info was found.") : Results.Ok(goodsInfo);
    }
    public async Task<IResult> GetGoodInformation(string itemCode)
    {
        var goodsInfo = await _context.Goodsinfos.Where(g => g.ItemCode == itemCode).Select(g => new { g.ItemCode, g.ItemDescription, g.UnitMeasure, g.TaxRate }).AsNoTracking().SingleOrDefaultAsync();
        return goodsInfo == null ? Results.NotFound($"No Goods information for the item code={itemCode} was found") : Results.Ok(goodsInfo);
    }
    public async Task<IResult> CreateGoodsInformation(Goodsinfo goodsinfo)
    {
        var local = _context.Goodsinfos.Local.FirstOrDefault(g => g.Id == goodsinfo.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Goodsinfos.SingleOrDefaultAsync(g => g.ItemCode == goodsinfo.ItemCode);
        if (existing != null)
            return Results.Conflict($"Goods info with item code={goodsinfo.ItemCode} exists.");
        var newGoodsInfo = new Goodsinfo
        {
            ItemCode = goodsinfo.ItemCode,
            ItemDescription = goodsinfo.ItemDescription,
            UnitMeasure = goodsinfo.UnitMeasure,
            TaxRate = goodsinfo.TaxRate
        };
        try
        {
            _context.Goodsinfos.Add(newGoodsInfo);
            await _context.SaveChangesAsync();
            return Results.Ok(newGoodsInfo);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> UpdateGoodsInfo(Goodsinfo update, string itemCode)
    {
        var local = _context.Goodsinfos.Local.FirstOrDefault(g => g.Id == update.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        Goodsinfo? retrievedGoodsInfo = await _context.Goodsinfos.SingleOrDefaultAsync(g => g.ItemCode == itemCode);

        if (retrievedGoodsInfo != null)
        {
            retrievedGoodsInfo.ItemCode = update.ItemCode;
            retrievedGoodsInfo.ItemDescription = update.ItemDescription;
            retrievedGoodsInfo.UnitMeasure = update.UnitMeasure;
            retrievedGoodsInfo.TaxRate = update.TaxRate;
            try
            {
                _context.Goodsinfos.Update(retrievedGoodsInfo);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedGoodsInfo);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
        }
        else
        {
            return Results.NotFound($"GoodsInfo with itemcode = {itemCode} was not found");
        }
    }
    public async Task<IResult> RemoveGoodsInfo(string itemCode)
    {
        var local = _context.Goodsinfos.Local.FirstOrDefault(g => g.ItemCode == itemCode);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        Goodsinfo? retrievedGoodsinfo = await  _context.Goodsinfos.SingleOrDefaultAsync(g => g.ItemCode == itemCode);

        if (retrievedGoodsinfo != null)
        {
            try
            {
                _context.Goodsinfos.Remove(retrievedGoodsinfo);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedGoodsinfo);
            }
            catch (Exception ex)
            {
                return Results.Problem("Exception: " + ex.InnerException?.Message ?? ex.Message);
            }

        }
        else { return Results.NotFound($"Goodsinfo with itemcode = {itemCode} was not found"); }
    }
}
