using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebSale.Dto.Orders;
using WebSale.Models;
using WebSale.Models.Vnpay;
using WebSale.Services.Vnpay;
using static System.Net.Mime.MediaTypeNames;

namespace WebSale.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : Controller
    {

        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }
        [HttpPost("create")]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Ok(url);
        }
        
        [HttpGet]
        public async Task<IActionResult> PaymentCallback([FromQuery] string inputUserId, [FromBody] CreateOrderDto createOrderDto)
        {
            var status = new Status();
            var response = _vnPayService.PaymentExecute(Request.Query);
            Console.WriteLine(inputUserId);
            Console.WriteLine(JsonSerializer.Serialize(createOrderDto, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles }));

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

            return Ok(response);
        }
    }

}
