using System.Net;
using Microsoft.EntityFrameworkCore;
using SoundParadise.Api.Controllers.Api;
using SoundParadise.Api.Data;
using SoundParadise.Api.Dto.CartItem;
using SoundParadise.Api.Dto.Order;
using SoundParadise.Api.Dto.Product;
using SoundParadise.Api.Dto.Requests;
using SoundParadise.Api.Interfaces;
using SoundParadise.Api.Models.Address;
using SoundParadise.Api.Models.Enums;
using SoundParadise.Api.Models.PaymentDetails;
using SoundParadise.Api.Services;
using SoundParadise.Web.Dto.Address;

namespace SoundParadise.Api.Models.Order;

/// <summary>
///     OrderModel CRUD.
/// </summary>
public class OrderCrud : IOrderCrud
{
    private readonly IAddressCrud _addressCrud;
    private readonly ICartItemCrud _cartItemCrud;
    private readonly ICommentCrud _commentCrud;
    private readonly SoundParadiseDbContext _context;
    private readonly ImageService _imageService;
    private readonly ILoggingService<OrderCrud> _loggingService;
    private readonly IPaymentService _paymentService;

    /// <summary>
    ///     OrderCrud Constructor.
    /// </summary>
    /// <param name="context">Db context</param>
    /// <param name="loggingService">Service for logging errors</param>
    /// <param name="cartItemCrud">CartItemCrud</param>
    /// <param name="paymentService">Payment service</param>
    /// <param name="commentCrud">Comment crud</param>
    /// <param name="imageService">Image service</param>
    /// <param name="addressCrud">AddressCrud</param>
    public OrderCrud(SoundParadiseDbContext context, ILoggingService<OrderCrud> loggingService,
        ICartItemCrud cartItemCrud, IPaymentService paymentService, ICommentCrud commentCrud, ImageService imageService,
        IAddressCrud addressCrud)
    {
        _context = context;
        _loggingService = loggingService;
        _cartItemCrud = cartItemCrud;
        _paymentService = paymentService;
        _commentCrud = commentCrud;
        _imageService = imageService;
        _addressCrud = addressCrud;
    }

    #region DELETE

    /// <summary>
    ///     OrderCrud Constructor.
    /// </summary>
    /// <param name="orderId">Order Id</param>
    /// <returns>True if succes delete, false if not</returns>
    public bool DeleteComment(Guid orderId)
    {
        try
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return false;

            _context.Orders.Remove(order);

            _context.SaveChanges();
            return !_context.Orders.Any(u => u.Id == orderId);
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while deleting order in {nameof(OrderCrud)}.{nameof(DeleteComment)}");
            return false;
        }
    }

    #endregion

    #region CREATE

    /// <summary>
    ///     Create order.
    /// </summary>
    /// <param name="order">Order Model</param>
    /// <returns>RequestResult object</returns>
    public RequestResult CreateOrder(OrderModel order)
    {
        try
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RequestResult.Success("Order created");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while creating order";
            _loggingService.LogException(ex,
                $"{error} in {nameof(OrderCrud)}.{nameof(CreateOrder)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Create order auth.
    /// </summary>
    /// <param name="order">OrderDto</param>
    /// <returns>CreateOrderResult object</returns>
    public CreateOrderResult CreateOrderAuth(OrderDto order)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var cart = _context.Carts.Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == order.UserId);

            var cartItems = _cartItemCrud.GetCartItemsByCartId(cart?.Id);

            if (cart?.CartItems is not { Count: > 0 })
                return CreateOrderResult.Error("Cart is empty");

            var totalOrderPrice = _cartItemCrud.GetTotalPriceOfOrderAuth(cart.CartItems);

            if (totalOrderPrice == 0)
                return CreateOrderResult.Error("Total order price is 0");

            var orderModel = new OrderModel
            {
                Id = Guid.NewGuid(),
                CustomerName = order.CustomerName,
                CustomerSurname = order.CustomerSurname,
                Comment = order.Comment,
                UserId = order.UserId,
                OrderDate = DateTime.Now,
                CartItems = cartItems,
                TotalPrice = totalOrderPrice,
                PaymentType = order.PaymentType,
                DeliveryOption = order.DeliveryOption,
                PhoneNumber = order.PhoneNumber
            };

            if (order.AddressId != null && order.AddressId != Guid.Empty)
            {
                var address = _addressCrud.GetAddressById(order.AddressId, order.UserId);

                if (address == null!)
                    return CreateOrderResult.Error("Invalid address ID");

                orderModel.Address = address;
            }
            else if (order.Address != null)
            {
                orderModel.Address = new AddressModel
                {
                    City = order.Address.City,
                    PostOfficeAddress = order.Address.PostOfficeAddress,
                    DeliveryOption = order.DeliveryOption,
                    UserId = order.UserId
                };
            }
            else
            {
                return CreateOrderResult.Error("Address is required");
            }

            cart.CartItems.Clear();

            switch (order.PaymentType)
            {
                case PaymentTypeEnum.CardPayment:
                    var description = $"Оплата заказу №{orderModel.Id:N}";
                    orderModel.OrderStatus = OrderStatusEnum.Pending;

                    var result = _paymentService.Checkout(orderModel.Id, description, orderModel.TotalPrice,
                        PaymentProvider.Fondy);
                    if (result.IsSuccess)
                    {
                        orderModel.CheckoutUrl = result.Message;
                        _context.Orders.Add(orderModel);
                        _context.SaveChanges();
                        transaction.Commit();
                        return CreateOrderResult.Success("Order created", orderModel.Id, HttpStatusCode.Created,
                            result.Message);
                    }

                    transaction.Rollback();
                    return CreateOrderResult.Error(result.Message);

                case PaymentTypeEnum.CashOnDelivery:
                    orderModel.OrderStatus = OrderStatusEnum.Processing;
                    _context.Orders.Add(orderModel);
                    _context.SaveChanges();
                    transaction.Commit();
                    return CreateOrderResult.Success("Order created", orderModel.Id, HttpStatusCode.Created);

                default:
                    transaction.Rollback();
                    return CreateOrderResult.Error("Payment type is not valid");
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            const string error = "An error occurred while creating the order";
            _loggingService.LogException(ex, $"{error} in {nameof(OrderCrud)}.{nameof(CreateOrderAuth)}");
            return CreateOrderResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    ///     Confirm order.
    /// </summary>
    /// <param name="response">Response</param>
    /// <returns>RequestResult object</returns>
    public RequestResult ConfirmOrder(dynamic response)
    {
        try
        {
            if (response == null)
                return RequestResult.Error("Response is null");

            string responseStatus = response.response_status;

            if (responseStatus != "success")
                return RequestResult.Error("Response status is not success");

            string orderStatus = response.order_status;
            Guid orderId = Guid.Parse(response.order_id.ToString());
            if (orderId == Guid.Empty)
                return RequestResult.Error("Order id is empty");
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return RequestResult.Error("Order not found", HttpStatusCode.NotFound);
            if (orderStatus != "approved")
            {
                order.OrderStatus = OrderStatusEnum.Canceled;
            }
            else
            {
                string currency = response.currency;
                string maskedCard = response.masked_card;
                decimal amount = response.amount;
                string cardType = response.card_type;
                int transactionId = response.payment_id;

                var payment = new PaymentModel
                {
                    Currency = currency,
                    MaskedCard = maskedCard,
                    Amount = amount,
                    CardType = cardType,
                    TransactionId = transactionId,
                    OrderId = orderId
                };
                order.IsPaid = true;
                order.Payment = payment;
                order.CheckoutUrl = null;
                order.OrderStatus = OrderStatusEnum.Processing;
            }

            _context.Orders.Update(order);
            _context.SaveChanges();
            return RequestResult.Success("Order confirmed");
        }
        catch (Exception ex)
        {
            const string error = "An error occurred while confirming the order";
            _loggingService.LogException(ex, $"{error} in {nameof(OrderCrud)}.{nameof(ConfirmOrder)}");
            return RequestResult.Error(error, HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region READ

    /// <summary>
    ///     Get all orders.
    /// </summary>
    /// <returns>List OrderModel object</returns>
    public List<OrderModel> GetAllOrders()
    {
        try
        {
            var orders = _context.Orders.ToList();
            return !orders.Any()
                ? Enumerable.Empty<OrderModel>().ToList()
                : orders;
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all orders in {nameof(OrderCrud)}.{nameof(GetAllOrders)}");
            return Enumerable.Empty<OrderModel>().ToList();
        }
    }

    /// <summary>
    ///     Get orders by user id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>List of OrderHistoryDto object</returns>
    public List<OrderHistoryDto> GetOrdersByUserId(Guid userId)
    {
        try
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .AsSplitQuery()
                .Where(o => o.UserId == userId)
                .ToList();

            if (orders == null! || !orders.Any())
                return Enumerable.Empty<OrderHistoryDto>().ToList();

            return orders.Select(order => new OrderHistoryDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                OrderStatus = order.OrderStatus,
                CheckoutUrl = order.CheckoutUrl,
                OrderItems = order.CartItems.Select(ci => new OrderItemDtoProduct
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    SellerName = "SoundParadise",
                    Product = new ProductDto
                    {
                        Id = ci.Product.Id,
                        Name = ci.Product.Name,
                        Price = ci.Product.Price,
                        ImagePath = _imageService.GetImageUrl(
                            ci.Product?.Images?.FirstOrDefault()?.Path ?? string.Empty),
                        CommentsCount = _commentCrud.GetCommentsCount(ci.Product.Id),
                        Rating = ci.Product.Rating,
                        IsNew = ci.Product.IsNew
                    }
                }).ToList()
            }).ToList();
        }
        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting all user's orders in {nameof(OrderCrud)}.{nameof(GetOrdersByUserId)}");
            return Enumerable.Empty<OrderHistoryDto>().ToList();
        }
    }

    /// <summary>
    ///     Get orders by Id.
    /// </summary>
    /// <param name="orderId">Order Id</param>
    /// <param name="userId">User Id</param>
    /// <returns>OrderModel object</returns>
    public OrderDto GetOrderById(Guid orderId, Guid userId)
    {
        try
        {
            var order = _context.Orders
                .Include(o => o.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .AsSplitQuery()
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order != null)
                return new OrderDto
                {
                    CustomerName = order?.CustomerName ?? string.Empty,
                    CustomerSurname = order?.CustomerSurname ?? string.Empty,
                    PhoneNumber = order?.PhoneNumber ?? string.Empty,
                    AddressId = order?.DeliveryAddressId,
                    Address = new AddressDto
                    {
                        Id = order.Address.Id,
                        PostOfficeAddress = (order.Address.PostOfficeAddress ?? null) ?? string.Empty,
                        City = (order.Address.City ?? null) ?? string.Empty
                    },
                    DeliveryOption = order.DeliveryOption,
                    DeliveryType = order.DeliveryType,
                    PaymentType = order.PaymentType,
                    CheckoutUrl = order.CheckoutUrl ?? string.Empty,
                    OrderStatus = order.OrderStatus,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    OrderItems = order.CartItems?.Select(ci =>
                    {
                        if (ci.Product != null)
                            return new OrderItemDtoProduct
                            {
                                Id = ci.Id,
                                ProductId = ci.ProductId,
                                Quantity = ci.Quantity,
                                SellerName = "SoundParadise",
                                Product = new ProductDto
                                {
                                    Id = ci.Product.Id,
                                    Name = ci.Product.Name,
                                    Price = ci.Product.Price,
                                    ImagePath = _imageService.GetImageUrl(ci.Product?.Images?.FirstOrDefault()?.Path ??
                                                                          string.Empty),
                                    CommentsCount = _commentCrud.GetCommentsCount(ci.Product.Id),
                                    Rating = ci.Product.Rating,
                                    IsNew = ci.Product.IsNew
                                }
                            };
                        return null;
                    }).ToList()
                };

            return null!;
        }

        catch (Exception ex)
        {
            _loggingService.LogException(ex,
                $"An error occurred while getting order by id in {nameof(OrderCrud)}.{nameof(GetOrderById)}");
            return null!;
        }
    }

    #endregion
}