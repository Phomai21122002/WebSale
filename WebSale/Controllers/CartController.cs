using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSale.Dto.Carts;
using WebSale.Dto.Orders;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public CartController(IMapper mapper, ICartRepository cartRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _mapper = mapper;
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        [HttpGet("cart")]
        public async Task<IActionResult> GetCart([FromQuery] string inputUserId, [FromQuery] int cartId)
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

                if (!await _cartRepository.CartExists(cartId))
                {
                    status.StatusCode = 400;
                    status.Message = "Cart does not exists";
                    return BadRequest(status);
                }

                var cart = await _cartRepository.GetCartByUserId(inputUserId, cartId);
                if (cart == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting cart of user";
                    return BadRequest(status);
                }
                return Ok(cart);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("carts")]
        public async Task<IActionResult> GetCarts([FromQuery] string inputUserId)
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

                var cart = await _cartRepository.GetCartsByUserId(inputUserId);
                if (cart == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while getting cart of user";
                    return BadRequest(status);
                }
                return Ok(cart);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartDto cartDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (cartDto == null || userId != cartDto?.UserId || !await _productRepository.ProductExists(cartDto.ProductId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var user = await _userRepository.GetUser(userId);
                var product = await _productRepository.GetProduct(cartDto.ProductId);
                var cart = await _cartRepository.GetCartByIdProduct(userId, cartDto.ProductId);

                if (cart == null) {
                    var cartMap = new Cart
                    {
                        Quantity = cartDto.Quantity,
                        IsSelectedForOrder = true,
                        User = user,
                        Product = product,
                        CreatedAt = DateTime.Now,
                    };

                    cart = await _cartRepository.CreateCart(cartMap);

                    if (cart == null)
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while creating cart";
                        return BadRequest(status);
                    }
                }
                else
                {
                    cart.Quantity += cartDto.Quantity;
                    cart.UpdatedAt = DateTime.Now;

                    if (!await _cartRepository.UpdateCart(cart))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating cart";
                        return BadRequest(status);
                    }
                }

                var resultCreateCart = await _cartRepository.GetCartByUserId(userId, cart.Id);
                return Ok(resultCreateCart);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart([FromBody] CartUpdateDto cartUpdateDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (cartUpdateDto == null || userId != cartUpdateDto?.UserId || !await _cartRepository.CartExists(cartUpdateDto.CartId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var cart = await _cartRepository.GetCart(userId, cartUpdateDto.CartId);
                if (cart == null)
                {
                    status.StatusCode = 404;
                    status.Message = "Cart not found.";
                    return BadRequest(status);
                }
                cart.Quantity = cartUpdateDto.Quantity;
                cart.IsSelectedForOrder = cartUpdateDto.IsSelectedForOrder;
                cart.UpdatedAt = DateTime.Now;

                if (!await _cartRepository.UpdateCart(cart))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating cart";
                    return BadRequest(status);
                }
                var resultUpdateCart = await _cartRepository.GetCartByUserId(userId, cart.Id);
                return Ok(resultUpdateCart);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.InnerException?.Message ?? ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPatch("UpdateCartsOrder")]
        public async Task<IActionResult> UpdateCartsOrder([FromBody] CartUpdateOrderDto cartUpdateOrderDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (cartUpdateOrderDto == null || userId != cartUpdateOrderDto?.UserId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }

                var userCarts = await _cartRepository.GetCarts(userId);
                if (userCarts == null || !userCarts.Any())
                {
                    status.StatusCode = 404;
                    status.Message = "No carts found for this user.";
                    return BadRequest(status);
                }
                var selectedIds = cartUpdateOrderDto.CartsId ?? new List<int>();
                foreach (var cart in userCarts)
                {
                    if (selectedIds.Contains(cart.Id))
                    {
                        cart.IsSelectedForOrder = !cart.IsSelectedForOrder;
                        cart.UpdatedAt = DateTime.Now;
                    }
                }

                if (!await _cartRepository.UpdateCarts(userCarts))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating carts.";
                    return BadRequest(status);
                }
                return Ok(userCarts);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.InnerException?.Message ?? ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCart([FromBody] CartDeleteDto cartDeleteDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (cartDeleteDto == null || userId != cartDeleteDto?.UserId || cartDeleteDto.CartsId == null || !cartDeleteDto.CartsId.Any())
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }
                var carts = await _cartRepository.GetCartsByCartsId(cartDeleteDto.UserId, cartDeleteDto.CartsId);

                if (carts.Count == 0)
                {
                    status.StatusCode = 400;
                    status.Message = "Cart not Exists";
                    return BadRequest(status);
                }

                if (!await _cartRepository.DeleteCarts(carts))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while deleting cart";
                    return BadRequest(status);
                }
               
                return Ok(carts);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.InnerException?.Message ?? ex.Message}";
                return BadRequest(status);
            }
        }
    }
}
