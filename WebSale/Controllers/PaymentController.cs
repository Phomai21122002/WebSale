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

namespace WebSale.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : Controller
    {

        private readonly IVnPayService _vnPayService;
        private readonly IOrderRepository _orderRepository;
        public PaymentController(IVnPayService vnPayService, IOrderRepository orderRepository)
        {
            _vnPayService = vnPayService;
            _orderRepository = orderRepository;
        }
        [HttpPost("create")]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Ok(url);
        }
        
        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            var status = new Status();
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (!response.Success)
            {
                status.StatusCode = 400;
                status.Message = "Payment failed or invalid signature";
                return BadRequest(status);
            }
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
                order.Status = (int)OrderStatus.Pending;
                //order.TransactionId = response.TransactionId;
                //order.PaymentId = response.PaymentId;
                //order.DatePaid = DateTime.Now;

                var updateResult = await _orderRepository.UpdateOrder(order);
                if (updateResult == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Failed to update order after payment";
                    return BadRequest(status);
                }

                // Clear cart
                //var deleteCarts = await _cartRepository.DeleteCartsByUserId(order.UserId);
                // Update product quantity (optional, implement logic here)

                return Ok(new
                {
                    Message = "Payment successful",
                    OrderId = order.Id,
                    TransactionId = response.TransactionId
                });
            }

            status.StatusCode = 400;
            status.Message = "Payment was not successful";
            return BadRequest(status);
            //if(response.VnPayResponseCode == "00")
            //{
            //    var newVnpayInsert = new VnpayModel
            //    {
            //        OrderId = response.OrderId,
            //        PaymentMethod = response.PaymentMethod,
            //        OrderDescription = response.OrderDescription,
            //        TransactionId = response.TransactionId,
            //        PaymentId = response.PaymentId,
            //        DateCreated = DateTime.Now,
            //    };
            //    if (!await _vnPayService.CreateVnPayModel(newVnpayInsert)) {
            //        status.StatusCode = 500;
            //        status.Message = "Something went wrong saving VnPay Model";
            //        return BadRequest(status);
            //    }

            //    var PaymentMethod = response.PaymentMethod;
            //    var PaymentId = response.PaymentId;
            //}
        }
    }

}
