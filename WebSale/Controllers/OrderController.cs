using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using WebSale.Dto.Orders;
using WebSale.Dto.QueryDto;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Models.Momo;
using WebSale.Models.Vnpay;
using WebSale.Respository;
using WebSale.Services.Momo;
using WebSale.Services.Vnpay;

namespace WebSale.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IVnPayService _vpnPayService;
        private readonly IMomoService _momoService;

        public OrderController(IMapper mapper, IOrderRepository orderRepository, ICartRepository cartRepository, IUserRepository userRepository, IOrderProductRepository orderProductRepository, IProductDetailRepository productDetailRepository, IVnPayService vnPayService, IMomoService momoService)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;   
            _userRepository = userRepository;
            _orderProductRepository = orderProductRepository;
            _productDetailRepository = productDetailRepository;
            _vpnPayService = vnPayService;
            _momoService = momoService;
        }

        [HttpGet("order")]
        public async Task<IActionResult> GetOrder([FromQuery] string inputUserId, [FromQuery] int orderId)
        {
            var status = new Status();
            try
            {
                if (inputUserId == null || orderId == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                if (!await _orderRepository.OrderExists(inputUserId, orderId))
                {
                    status.StatusCode = 400;
                    status.Message = "Order does not exists";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrderResultByUserId(inputUserId, orderId);
                if (order == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting order of user";
                    return BadRequest(status);
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] string inputUserId, [FromQuery] int inputStatus, [FromQuery] QueryFindPaginationDto queryOrders)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || inputStatus == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrdersResultByUserId(inputUserId, inputStatus, queryOrders);
                if (order == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting order of user";
                    return BadRequest(status);
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("admin/orders")]
        public async Task<IActionResult> GetAdminOrders([FromQuery] string inputUserId, [FromQuery] int inputStatus, [FromQuery] QueryFindSoftPaginationDto queryOrders)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || inputStatus == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrdersResultByAdmin(inputStatus, queryOrders);
                if (order == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting order of user";
                    return BadRequest(status);
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("ProductInOrders")]
        public async Task<IActionResult> GetOrderProductsCancel([FromQuery] string inputUserId, [FromQuery] int inputStatus, [FromQuery] QueryPaginationDto queryPaginationDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || inputStatus == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var orderProducts = await _orderProductRepository.GetProductOrdersResultCanceled(inputUserId, inputStatus, queryPaginationDto);
                if (orderProducts == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting order product of user";
                    return BadRequest(status);
                }
                return Ok(orderProducts);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromQuery] string inputUserId, [FromBody] CreateOrderDto createOrderDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || createOrderDto == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var user = await _userRepository.GetUser(userId);
                if (createOrderDto.CartsId == null || !createOrderDto.CartsId.Any())
                {
                    status.StatusCode = 400;
                    status.Message = "No cart items selected for order.";
                    return BadRequest(status);
                }
                var carts = await _cartRepository.GetCartsByCartsId(inputUserId, createOrderDto);

                if(carts.Count == 0)
                {
                    status.StatusCode = 400;
                    status.Message = "Cart not Exists";
                    return BadRequest(status);
                }
                
                var order = new Order
                {
                    Name = $"Order_{DateTime.Now:yyyyMMddHHmmss}",
                    Total = carts.Sum(cart => cart.Quantity * (cart.Product?.Price ?? 0)),
                    Status = (int)OrderStatus.Pending,
                    User = user,
                    PaymentMethod = createOrderDto.PaymentMethod.ToString(),
                    CreatedAt = DateTime.Now,
                };
                var newOrder = await _orderRepository.CreateOrder(order);
                
                var ordersProduct = carts.Select(c => new OrderProduct
                {
                    ProductId = c.ProductId,
                    OrderId = newOrder.Id,
                    Quantity = c.Quantity,
                    Product = c.Product,
                    Order = newOrder,
                    Status = (int)OrderStatus.Pending,
                    CreatedAt = DateTime.Now,
                }).ToList();

                if (newOrder == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating order of user";
                    return BadRequest(status);
                }
                var newOrderProduct = await _orderProductRepository.CreateOrderProduct(ordersProduct);

                if (OrderPaymentStatus.VnPay == createOrderDto.PaymentMethod) {
                    var paymentInfo = new PaymentInformationModel
                    {
                        OrderId = newOrder.Id,
                        Amount = newOrder.Total,
                        OrderDescription = $"Payment for order #{newOrder.Id}",
                        Name = user.FirstName + " " + user.LastName,
                        OrderType = "other"
                    };
                    var url = _vpnPayService.CreatePaymentUrl(paymentInfo, HttpContext);

                    return Ok(new { PaymentUrl = url });
                } else if(OrderPaymentStatus.MoMo == createOrderDto.PaymentMethod)
                {
                    var momoInfo = new OrderInfo
                    {
                        FullName = user.FirstName + " " + user.LastName,
                        Amount = newOrder.Total.ToString(),
                        OrderId = newOrder.Id.ToString(),
                        OrderInformation = $"Payment for order #{newOrder.Id}"
                    };
                    var resultMomo = await _momoService.CreatePaymentAsync(momoInfo);
                    return Ok(new { PaymentUrl = resultMomo.PayUrl });
                }
                else
                {
                    var cartsRemoved = carts.Where(c => c.IsSelectedForOrder).ToList();
                    if (!await _cartRepository.DeleteCarts(cartsRemoved))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while creating order of user";
                        return BadRequest(status);
                    }

                    var productDetailsId = cartsRemoved.Select(c => c.Product.ProductDetailId).ToList();
                    var productDetails = await _productDetailRepository.GetProductDetails(productDetailsId);

                    var productDetailsUpdated = productDetails.Select(pd =>
                    {
                        var matchingCart = cartsRemoved.FirstOrDefault(c => c.Product.ProductDetailId == pd.Id);
                        if (matchingCart != null)
                        {
                            pd.Quantity -= matchingCart.Quantity;
                            pd.Sold += matchingCart.Quantity;
                        }
                        return pd;
                    }).ToList();

                    if (!await _productDetailRepository.UpdateProductDetails(productDetailsUpdated))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating product detail";
                        return BadRequest(status);
                    }
                    return Ok(newOrder);
                }
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateOrder([FromQuery] string inputUserId, [FromQuery] int inputOrderId)
        {
            var status = new Status();
            try
            {
                if (inputUserId == null || !await _orderRepository.OrderExists(inputUserId, inputOrderId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrderByUserId(inputUserId, inputOrderId);
                var orderProducts = await _orderProductRepository.GetOrderProducts(inputUserId, inputOrderId);

                if (order.Status + 1 >= Enum.GetNames(typeof(OrderStatus)).Length)
                {
                    status.StatusCode = 400;
                    status.Message = "The order is no longer being updated!!!";
                    return BadRequest(status);
                }

                foreach (var op in orderProducts)
                {
                    if(op.Status == order.Status)
                    {
                        op.Status += 1;
                    }
                }

                var orderProductsUpdated = await _orderProductRepository.UpdateOrderProducts(orderProducts);
                if (orderProductsUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating order product of user";
                    return BadRequest(status);
                }
                order.Status += 1;

                var orderUpdated = await _orderRepository.UpdateOrder(order);
                if (orderUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating order of user";
                    return BadRequest(status);
                }

                return Ok(orderUpdated);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPatch("CancelProductOrder")]
        public async Task<IActionResult> CancelProductOrder([FromQuery] string inputUserId, [FromQuery] int inputOrderId, [FromQuery] int inputProductId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || !await _orderRepository.ProductOfOrderExists(inputUserId, inputOrderId, inputProductId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var orderProduct = await _orderProductRepository.GetOrderProduct(inputUserId, inputOrderId, inputProductId);

                if (orderProduct.Status + 1 >= Enum.GetNames(typeof(OrderStatus)).Length)
                {
                    status.StatusCode = 400;
                    status.Message = "The order product is no longer being cancel!!!";
                    return BadRequest(status);
                }
                orderProduct.Status = Enum.GetNames(typeof(OrderStatus)).Length;

                var orderProductUpdated = await _orderProductRepository.UpdateOrderProduct(orderProduct);
                if (orderProductUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating order product of user";
                    return BadRequest(status);
                }

                var productDetail = await _productDetailRepository.GetProductDetail(orderProduct.Product.ProductDetailId);
                productDetail.Quantity += orderProduct.Quantity;
                productDetail.Sold -= orderProduct.Quantity;
                if (!await _productDetailRepository.UpdateProductDetail(productDetail))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating product detail";
                    return BadRequest(status);
                }

                var orderProducts = await _orderProductRepository.GetOrderProducts(inputUserId, inputOrderId);
                var allCanceled = orderProducts.All(op => op.Status == Enum.GetNames(typeof(OrderStatus)).Length);

                if (allCanceled)
                {
                    var order = await _orderRepository.GetOrderByUserId(inputUserId, inputOrderId);
                    if (order != null)
                    {
                        order.Status = Enum.GetNames(typeof(OrderStatus)).Length;
                        var updatedOrder = await _orderRepository.UpdateOrder(order);
                        if (updatedOrder == null)
                        {
                            status.StatusCode = 500;
                            status.Message = "Failed to update order status";
                            return BadRequest(status);
                        }
                    }
                }

                return Ok(orderProductUpdated);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete("Soft-Delete")]
        public async Task<IActionResult> SoftDeleteOrder([FromQuery] string inputUserId, [FromQuery] int inputOrderId)
        {
            var status = new Status();
            try
            {
                if (inputUserId == null || !await _orderRepository.OrderExists(inputUserId, inputOrderId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrderByUserId(inputUserId, inputOrderId);

                if (order.Status == (int)OrderStatus.Completed)
                {
                    status.StatusCode = 400;
                    status.Message = "The order is no longer being soft  delete!!!";
                    return BadRequest(status);
                }
                order.Status = (int)OrderStatus.Cancelled;

                var orderUpdated = await _orderRepository.UpdateOrder(order);
                if (orderUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating order of user";
                    return BadRequest(status);
                }
               
                foreach (var op in order.OrderProducts)
                {
                    op.Status = (int)OrderStatus.Cancelled;
                    var orderProductsUpdated = await _orderProductRepository.UpdateOrderProduct(op);
                    if (orderProductsUpdated == null)
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating order product of user";
                        return BadRequest(status);
                    }
                    var productDetail = await _productDetailRepository.GetProductDetail(op.Product.ProductDetailId);
                    productDetail.Quantity += op.Quantity;
                    productDetail.Sold -= op.Quantity;
                    if (!await _productDetailRepository.UpdateProductDetail(productDetail))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating order detail";
                        return BadRequest(status);
                    }
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveOrder([FromQuery] string inputUserId, [FromQuery] int inputOrderId)
        {
            var status = new Status();
            try
            {
                if (inputUserId == null || !await _orderRepository.OrderExists(inputUserId, inputOrderId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrderByUserId(inputUserId, inputOrderId);

                if (!await _orderRepository.DeleteOrder(order))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while deleting order of user";
                    return BadRequest(status);
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }
    }
}
