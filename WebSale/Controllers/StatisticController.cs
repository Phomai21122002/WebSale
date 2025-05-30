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
using WebSale.Dto.Bills;
using WebSale.Respository;

namespace WebSale.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IBillRepository _billRepository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public StatisticController(IMapper mapper, IBillRepository billRepository, IOrderRepository orderRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, IBillDetailRepository billDetailRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _billRepository = billRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _billDetailRepository = billDetailRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistic()
        {
            try
            {
                var totalProduct = await _productRepository.TotalProduct();
                var totalCategory = await _categoryRepository.TotalCategory();
                var totalOredr = await _orderRepository.TotalOrder();
                var totalUser = await _userRepository.TotalUser();
                var totalSales = await _billRepository.TotalSales();

                var resultStatistic = new StatisticDashBoard
                {
                    TotalSales = totalSales,
                    TotalCategories = totalCategory,
                    TotalOrders = totalOredr,
                    TotalProducts = totalProduct,
                    TotalUsers = totalUser,
                };

                return Ok(resultStatistic);
            }
            catch (Exception ex)
            {
                var status = new Status
                {
                    StatusCode = 500,
                    Message = $"Internal Server Error: {ex.Message}"
                };
                return StatusCode(500, status);
            }
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            try
            {
                var data = await _billDetailRepository.GetMonthlyRevenueAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
