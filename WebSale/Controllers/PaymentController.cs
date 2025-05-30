using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebSale.Dto.Orders;
using WebSale.Models;
using WebSale.Models.Vnpay;
using WebSale.Services.Vnpay;
using static System.Net.Mime.MediaTypeNames;
using WebSale.Interfaces;
using WebSale.Services.Momo;
using System.Security.Claims;
using Azure;

namespace WebSale.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        public PaymentController(IMomoService momoService, IVnPayService vnPayService, IOrderRepository orderRepository, ICartRepository cartRepository, IProductDetailRepository productDetailRepository)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productDetailRepository = productDetailRepository;
        }

        [HttpGet("momo")]
        public async Task<IActionResult> PaymentMomoCallback()
        {
            var status = new Status();
            var res = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;
            var orderId = int.Parse(requestQuery["orderId"]);
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                status.StatusCode = 404;
                status.Message = "Order not found";
                return NotFound(status);
            }
            if (requestQuery["resultCode"] != 0)
            {
                var newMomoInfo = new MomoModel
                {
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = double.Parse(requestQuery["Amount"]),
                    OrderInfo = requestQuery["OrderInfo"],
                    DatePaid = DateTime.Now,
                };
                var resultModelMomo = await _momoService.AddMomoModel(newMomoInfo);
                if (resultModelMomo == null) {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while saving momo model";
                    return NotFound(status);
                }

                order.Status = (int)OrderStatus.Pending;
                order.Momo = resultModelMomo;
                order.IsPayment = true;
                order.CreatedAt = DateTime.Now;

                var updateResult = await _orderRepository.UpdateOrder(order);
                if (updateResult == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Failed to update order after payment";
                    return BadRequest(status);
                }

                var carts = await _cartRepository.GetCarts(order?.User.Id);
                var cartsRemoved = carts.Where(c => c.IsSelectedForOrder).ToList();
                if (!await _cartRepository.DeleteCarts(cartsRemoved))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while deleting carts of user";
                    return BadRequest(status);
                }

                var productDetailsId = cartsRemoved.Select(c => c.Product.ProductDetailId).ToList();
                var productDetails = _productDetailRepository.GetProductDetails(productDetailsId);

                var productDetailsUpdated = productDetails.Result.Select(pd =>
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

                return Ok(new
                {
                    Success = true,
                    Message = "Payment successful",
                    OrderId = order.Id,
                    OrderDescription = resultModelMomo.OrderInfo,
                    Amount = order.Total
                });
            }
            if (!await _orderRepository.DeleteOrder(order))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong while deleting order";
                return BadRequest(status);
            }
            return Ok(new
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Payment was not successful",
                });
        }
        
        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            var status = new Status();
            var response = _vnPayService.PaymentExecute(Request.Query);
           
            var orderId = int.Parse(response.OrderId);
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                status.StatusCode = 404;
                status.Message = "Order not found";
                return NotFound(status);
            }

            if (response.VnPayResponseCode == "00")
            {
                var newVnpayInsert = new VnpayModel
                {
                    OrderId = response.OrderId,
                    PaymentMethod = response.PaymentMethod,
                    OrderDescription = response.OrderDescription,
                    TransactionId = response.TransactionId,
                    PaymentId = response.PaymentId,
                    DateCreated = DateTime.Now,
                };
                Console.WriteLine(JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles }));

                var vnpayModel = await _vnPayService.CreateVnPayModel(newVnpayInsert);
                if (vnpayModel == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong saving VnPay Model";
                    return BadRequest(status);
                }

                order.Status = (int)OrderStatus.Pending;
                order.Vnpay = vnpayModel;
                order.IsPayment = true;
                order.CreatedAt = DateTime.Now;

                var updateResult = await _orderRepository.UpdateOrder(order);
                if (updateResult == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Failed to update order after payment";
                    return BadRequest(status);
                }
                
                var carts = await _cartRepository.GetCarts(order?.User.Id);
                var cartsRemoved = carts.Where(c=>c.IsSelectedForOrder).ToList();
                if (!await _cartRepository.DeleteCarts(cartsRemoved))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while deleting carts of user";
                    return BadRequest(status);
                }

                var productDetailsId = cartsRemoved.Select(c => c.Product.ProductDetailId).ToList();
                var productDetails = _productDetailRepository.GetProductDetails(productDetailsId);

                var productDetailsUpdated = productDetails.Result.Select(pd =>
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

                return Ok(new
                {
                    Success = true,
                    Message = "Payment successful",
                    OrderId = order.Id,
                    TransactionId = response.TransactionId,
                    OrderDescription = vnpayModel.OrderDescription,
                    Amount = order.Total
                });
            }

            if (!await _orderRepository.DeleteOrder(order))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong while deleting order";
                return BadRequest(status);
            }

            return Ok(new
            {
                Success = false,
                StatusCode = 400,
                Message = "Payment was not successful",
            });

        }
    }
}
