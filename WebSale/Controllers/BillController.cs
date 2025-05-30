using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSale.Dto.Orders;
using WebSale.Models.Momo;
using WebSale.Models.Vnpay;
using WebSale.Models;
using AutoMapper;
using WebSale.Interfaces;
using WebSale.Services.Momo;
using WebSale.Services.Vnpay;
using WebSale.Dto.QueryDto;

namespace WebSale.Controllers
{
    [Route("api/bill")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBillRepository _billRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        
        public BillController(IMapper mapper, IBillRepository billRepository, IBillDetailRepository billDetailRepository, IOrderProductRepository orderProductRepository, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _billDetailRepository = billDetailRepository;
            _orderRepository = orderRepository;
            _billRepository = billRepository;
            _orderProductRepository = orderProductRepository;
        }

        [HttpGet("bill")]
        public async Task<IActionResult> GetBill([FromQuery] string inputUserId, [FromQuery] int billId)
        {
            var status = new Status();
            try
            {
                if (inputUserId == null || !await _billRepository.BillExists(inputUserId, billId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var bill = await _billRepository.GetResultBillByUserId(inputUserId, billId);
                if (bill == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting bill of user";
                    return BadRequest(status);
                }
                return Ok(bill);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("bills")]
        public async Task<IActionResult> GetBills([FromQuery] string inputUserId, [FromQuery] QueryPaginationDto queryPaginationDto)
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

                var bills = await _billRepository.GetResultsBillByUserId(inputUserId, queryPaginationDto);

                if (bills == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting bills of user";
                    return BadRequest(status);
                }
                return Ok(bills);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("admin/bills")]
        public async Task<IActionResult> GetAdminBills([FromQuery] QueryPaginationDto queryPaginationDto)
        {
            var status = new Status();
            try
            {
                var bills = await _billRepository.GetResultsBill(queryPaginationDto);

                if (bills == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting bills";
                    return BadRequest(status);
                }
                return Ok(bills);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill([FromQuery] string inputUserId, [FromQuery] int inputOrderId)
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

                var orderProductsNoCancel = orderProducts
                .Where(op => op.Status != (int)OrderStatus.Cancelled)
                .ToList();

                var bill = new Bill
                {
                    NameOrder = order.Name,
                    PaymentMethod = order.PaymentMethod,
                    User = order.User,
                    Vnpay = order.Vnpay,
                    CreatedAt = DateTime.Now
                };

                var newBill = await _billRepository.CreateBill(bill);

                if (newBill == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating bill of user";
                    return BadRequest(status);
                }

                var billDetails = orderProductsNoCancel.Select(op => new BillDetail
                {
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    Slug = op.Product.Slug,
                    Description = op.Product.ProductDetail.Description,
                    DescriptionDetail = op.Product.ProductDetail.DescriptionDetail,
                    Quantity = op.Quantity,
                    Bill = newBill,
                }).ToList();

                var newBillDetails = await _billDetailRepository.CreateBillDetail(billDetails);

                if (newBillDetails == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating bill detail of user";
                    return BadRequest(status);
                }

                if(orderProducts.Count == orderProductsNoCancel.Count)
                {
                    if (!await _orderRepository.DeleteOrder(order))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while deleting order";
                        return BadRequest(status);
                    }
                }
                else
                {
                    if (!await _orderProductRepository.DeleteOrderProducts(orderProductsNoCancel))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while deleting order product";
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
                }
                return Ok(bill);

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
