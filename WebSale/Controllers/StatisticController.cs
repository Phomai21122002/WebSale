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

namespace WebSale.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IBillRepository _billRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public StatisticController(IMapper mapper, IBillRepository billRepository, IOrderRepository orderRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _billRepository = billRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistic()
        {
            var status = new Status();
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
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }
    }
}
