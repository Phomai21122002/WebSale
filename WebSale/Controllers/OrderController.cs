using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using WebSale.Dto.Orders;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Respository;

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

        public OrderController(IMapper mapper, IOrderRepository orderRepository, ICartRepository cartRepository, IUserRepository userRepository, IOrderProductRepository orderProductRepository, IProductDetailRepository productDetailRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;   
            _userRepository = userRepository;
            _orderProductRepository = orderProductRepository;
            _productDetailRepository = productDetailRepository;
        }

        [HttpGet("order")]
        public async Task<IActionResult> GetOrder([FromQuery] string inputUserId, [FromQuery] int orderId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId)
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

                var order = await _orderRepository.GetOrderByUserId(inputUserId, orderId);
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
        public async Task<IActionResult> GetOrders([FromQuery] string inputUserId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrdersByUserId(inputUserId);
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
                };
                var newOrder = await _orderRepository.CreateOrder(order);
                
                var ordersProduct = carts.Select(c => new OrderProduct
                {
                    ProductId = c.ProductId,
                    OrderId = newOrder.Id,
                    Quantity = c.Quantity,
                    Product = c.Product,
                    Order = newOrder,
                }).ToList();

                if (newOrder == null || !await _cartRepository.DeleteCarts(carts))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating order of user";
                    return BadRequest(status);
                }

                var newOrderProduct = await _orderProductRepository.CreateOrderProduct(ordersProduct);

                var productDetailsId = carts.Select(c => c.Product.ProductDetailId).ToList();
                var productDetails = _productDetailRepository.GetProductDetails(productDetailsId);

                var productDetailsUpdated = productDetails.Result.Select(pd =>
                {
                    var matchingCart = carts.FirstOrDefault(c => c.Product.ProductDetailId == pd.Id);
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || !await _orderRepository.OrderExists(inputUserId, inputOrderId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrderByUserId(inputUserId, inputOrderId);

                if(order.Status + 1 >= Enum.GetNames(typeof(OrderStatus)).Length)
                {
                    status.StatusCode = 400;
                    status.Message = "The order is no longer being updated!!!";
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

        [HttpDelete("Soft-Delete")]
        public async Task<IActionResult> SoftDeleteOrder([FromQuery] string inputUserId, [FromQuery] int inputOrderId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || !await _orderRepository.OrderExists(inputUserId, inputOrderId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var order = await _orderRepository.GetOrderByUserId(inputUserId, inputOrderId);

                if (order.Status + 1 >= Enum.GetNames(typeof(OrderStatus)).Length)
                {
                    status.StatusCode = 400;
                    status.Message = "The order is no longer being soft  delete!!!";
                    return BadRequest(status);
                }
                order.Status = Enum.GetNames(typeof(OrderStatus)).Length;


                var orderUpdated = await _orderRepository.UpdateOrder(order);
                if (orderUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating order of user";
                    return BadRequest(status);
                }
                var productDetailId = order.OrderProducts.First().Product.ProductDetailId;
                var productDetail = await _productDetailRepository.GetProductDetail(productDetailId);

                productDetail.Quantity += order.OrderProducts.First().Quantity;
                productDetail.Sold -= order.OrderProducts.First().Quantity;

                if(!await _productDetailRepository.UpdateProductDetail(productDetail))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating order detail";
                    return BadRequest(status);
                }
                return Ok(productDetail);
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != inputUserId || !await _orderRepository.OrderExists(inputUserId, inputOrderId))
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
