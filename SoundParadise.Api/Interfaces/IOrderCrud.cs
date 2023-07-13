using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Dto.Order;
using SoundParadise.Api.Dto.Requests;
using SoundParadise.Api.Models.Order;

namespace SoundParadise.Api.Interfaces;

/// <summary>
///     OrderCrud implements that interface.
/// </summary>
public interface IOrderCrud
{
    bool DeleteComment(Guid orderId);
    RequestResult CreateOrder(OrderModel order);
    CreateOrderResult CreateOrderAuth(OrderDto order);
    RequestResult ConfirmOrder(dynamic response);
    List<OrderModel> GetAllOrders();
    List<OrderHistoryDto> GetOrdersByUserId(Guid userId);
    OrderDto GetOrderById(Guid orderId, Guid userId);
}