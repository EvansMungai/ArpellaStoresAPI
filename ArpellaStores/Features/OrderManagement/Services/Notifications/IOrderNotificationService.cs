using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.SmsManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderNotificationService
{
    Task NofityCustomerAsync(Order order);
    Task NotifyOrderManagerAsync(Order order, List<string> phoneNumber);
}
