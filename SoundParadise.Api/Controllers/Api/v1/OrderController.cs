using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundParadise.Api.Constants;
using SoundParadise.Api.Dto.Order;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Order;
using Swashbuckle.AspNetCore.Annotations;

namespace SoundParadise.Api.Controllers.Api.v1;

/// <summary>
///     Order API v1 controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
[BaseRoute(ApiRoutes.Order.Base)]
public class OrderController : BaseApiController
{
    private readonly IOrderCrud _orderCrud;

    /// <summary>
    ///     Constructor for OrderController v1.
    /// </summary>
    /// <param name="orderCrud"></param>
    /// <param name="cartCrud"></param>
    public OrderController(IOrderCrud orderCrud)
    {
        _orderCrud = orderCrud;
    }

    #region GET

    /// <summary>
    ///     This method is used to get orders of the authenticated user.
    /// </summary>
    /// <returns>List of all orders of the authenticated user.</returns>
    [Authorize]
    [HttpGet(ApiRoutes.GetAll)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns the list of orders of the authenticated user",
        typeof(List<OrderModel>))]
    public IActionResult GetOrders()
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });

        var userId = Guid.Parse(value);
        var orders = _orderCrud.GetOrdersByUserId(userId);

        return !orders.Any()
            ? NotFound(new { error = "The user does not have any orders." })
            : Ok(orders);
    }

    /// <summary>
    ///     Gets the order by ID
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns>Returns the OrderModel</returns>
    [Authorize]
    [HttpPost(ApiRoutes.Order.GetOrder)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Get order by ID", typeof(OrderModel))]
    public IActionResult GetOrder([FromQuery] Guid orderId)
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });
        var userId = Guid.Parse(value);
        var order = _orderCrud.GetOrderById(orderId, userId);
        return order == null!
            ? StatusCode((int)HttpStatusCode.NotFound, new { error = "Order not found." })
            : Ok(order);
    }


    /// <summary>
    ///     This method is used to create an order for an unauthenticated user.
    /// </summary>
    /// <param name="order"></param>
    /// <returns>The success message response.</returns>
    [AllowAnonymous]
    [HttpPost(ApiRoutes.Order.PostOrder)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Order wad created for the user.", typeof(string))]
    public IActionResult PostOrder([FromBody] OrderModel order)
    {
        var result = _orderCrud.CreateOrder(order);
        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion

    #region POST

    /// <summary>
    ///     This method is used to create an order for an authenticated user.
    /// </summary>
    /// <param name="order"></param>
    /// <returns>The success message response.</returns>
    [Authorize]
    [HttpPost(ApiRoutes.Order.PostOrderAuth)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Order was created for the authenticated user.", typeof(string))]
    public IActionResult PostOrderAuth([FromBody] OrderDto order)
    {
        var value = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value == null) return BadRequest(new { error = "User is not authenticated." });
        var userId = Guid.Parse(value);
        order.UserId = userId;
        var result = _orderCrud.CreateOrderAuth(order);
        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus,
                new { success = result.Message, orderId = result.Id, checkoutUrl = result.CheckoutUrl })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    /// <summary>
    ///     Confirming order
    /// </summary>
    /// <param name="data"></param>
    /// <returns>Order was confirmed</returns>
    [AllowAnonymous]
    [HttpPost(ApiRoutes.Order.ConfirmOrder)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Callback endpoint for confirming order", typeof(string))]
    public IActionResult ConfirmOrder(dynamic data)
    {
        var result = _orderCrud.ConfirmOrder(data);
        return result.IsSuccess
            ? StatusCode((int)result.HttpStatus, new { success = result.Message })
            : StatusCode((int)result.HttpStatus, new { error = result.Message });
    }

    #endregion
}