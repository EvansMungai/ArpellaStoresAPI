using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IOrderHelper _helper;

    public OrderService(IOrderRepository repo, IOrderHelper helper)
    {
        _repo = repo;
        _helper = helper;
    }

    public async Task<IResult> CreateOrder(Order orderDetails)
    {
        if (await _repo.ExistsAsync(orderDetails.Orderid))
            return Results.Conflict($"OrderID {orderDetails.Orderid} already exists");


        switch (orderDetails.OrderPaymentType)
        {
            case "Mpesa":
                {
                    var total = await _repo.CalculateTotalOrderCost(orderDetails);
                    var order = _helper.BuildNewOrder(orderDetails, total);
                    return await _helper.HandleMpesaOrders(order);
                    break;
                }
            case "Hybrid":
                {
                    var order = _helper.BuildNewOrder(orderDetails, orderDetails.Total);
                    return await _helper.HandleHybridOrders(order, order.Total);
                    break;
                }
            default:
                {
                    var order = _helper.BuildNewOrder(orderDetails, orderDetails.Total);
                    return await _helper.HandleCashOrders(order);
                    break;
                }
        }
    }
    public async Task<IResult> GetOrders()
    {
        var orders = await _repo.GetAllOrdersAsync();
        if (orders == null || orders.Count == 0)
            return Results.NotFound("No Orders found");

        var formatted = orders.Select(o => new
        {
            o.Orderid,
            o.UserId,
            o.Status,
            o.Total,
            o.Latitude,
            o.Longitude,
            Orderitem = o.Orderitems.Select(
                oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    Product = new
                    {
                        oi.Product.Name,
                        oi.Product.Price
                    }
                })
        });
        return Results.Ok(formatted);
    }

    public async Task<IResult> GetOrder(string id)
    {
        var order = await _repo.GetOrderByIdAsync(id);
        if (order == null)
            return Results.NotFound($"No order with ID = {id} was found");

        var formatted = new
        {
            order.Orderid,
            order.UserId,
            order.Status,
            order.Total,
            order.Latitude,
            order.Longitude,
            OrderItems = order.Orderitems.Select(oi => new
            {
                oi.OrderId,
                oi.ProductId,
                oi.Quantity,
                Product = new
                {
                    oi.Product.Id,
                    oi.Product.Name,
                    oi.Product.Price
                }
            })
        };
        return Results.Ok(formatted);
    }
    public async Task<IResult> GetOrderByUsername(string username)
    {
        var orders = await _repo.GetOrdersByUsernameAsync(username);
        if (orders == null)
            return Results.NotFound($"No order with username = {username} was found");

        var formatted = orders.Select(o => new
        {
            o.Orderid,
            o.UserId,
            o.Status,
            o.Total,
            o.Latitude,
            o.Longitude,
            Orderitem = o.Orderitems.Select(
                oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    Product = new
                    {
                        oi.Product.Name,
                        oi.Product.Price
                    }
                })
        });
        return Results.Ok(formatted);
    }
    public async Task<IResult> GetPagedOrders(int pageNumber, int pageSize)
    {
        var orders = await _repo.GetPagedOrdersAsync(pageNumber, pageSize);
        if (orders == null || orders.Count == 0)
            return Results.NotFound("No Orders found");

        var formatted = orders.Select(o => new
        {
            o.Orderid,
            o.UserId,
            o.Status,
            o.Total,
            o.Latitude,
            o.Longitude,
            Orderitem = o.Orderitems.Select(
                oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    Product = new
                    {
                        oi.Product.Name,
                        oi.Product.Price
                    }
                })
        });
        return Results.Ok(formatted);
    }
    public async Task<IResult> UpdateOrderStatus(string status, string id)
    {
        try
        {
            await _repo.UpdateOrderStatusAsync(status, id);
            return Results.Ok($"Updated Order status for order {id} to {status}");
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> RemoveOrder(string id)
    {
        var existing = await _repo.GetOrderByIdAsync(id);
        if (existing == null)
            return Results.BadRequest($"Order with ID = {id} was not found.");

        try
        {
            await _repo.RemoveOrderAsync(id);
            return Results.Ok(existing);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
