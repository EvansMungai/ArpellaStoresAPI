using ArpellaStores.Features.InventoryManagement.Services;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.SmsManagement.Models;
using ArpellaStores.Features.SmsManagement.Services;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IProductRepository _repo;
    private readonly ISmsTemplateRepository _smsTemplateRepo;
    private readonly ISmsService _smsService;
    public OrderNotificationService(IProductRepository repo, ISmsTemplateRepository smsTemplateRepo, ISmsService smsService)
    {
        _repo = repo;
        _smsTemplateRepo = smsTemplateRepo;
        _smsService = smsService;
    }
    public async Task NofityCustomerAsync(Order order)
    {
        var rebuildItems = order.Orderitems.Select(item => new Orderitem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        }).ToList();

        var formattedItems = await FormatOrderItemsAsync(rebuildItems);

        var template = await _smsTemplateRepo.GetSmsTemplateAsync("CustomerOrderCreationMessage");

        var message = template.Content
            .Replace("{orderId}", order.Orderid.ToString())
            .Replace("{orderItems}", formattedItems);

        await _smsService.SendQuickSMSAsync(message, order.PhoneNumber);
    }
    public async Task NotifyOrderManagerAsync(Order order, string phoneNumber)
    {
        var rebuildItems = order.Orderitems.Select(item => new Orderitem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        }).ToList();

        var formattedItems = await FormatOrderItemsAsync(rebuildItems);

        var template = await _smsTemplateRepo.GetSmsTemplateAsync("OrderManagerOrderCreationMessage");

        var message = template.Content
            .Replace("{orderId}", order.Orderid.ToString())
            .Replace("{orderItems}", formattedItems);

        await _smsService.SendQuickSMSAsync(message, phoneNumber);
    }
    #region Helpers
    public async Task<string> FormatOrderItemsAsync(List<Orderitem> items)
    {
        var lines = new List<string>();

        foreach (var item in items)
        {
            var product = await _repo.GetProductByIdAsync(item.ProductId);
            var line = $"{product.Name} - {item.Quantity}";
            lines.Add(line);
        }
        return string.Join("\n", lines);
    }
    #endregion
}
