using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSale.Dto.Auth;
using WebSale.Dto.Orders;
using WebSale.Dto.QueryDto;
using WebSale.Dto.Users;
using WebSale.Extensions;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Respository;

namespace WebSale.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IBillRepository _billRepository;

        public UserController(IMapper mapper, IUserRepository userRepository, ILoginRepository loginRepository, IRoleRepository roleRepository, ITokenService tokenService, ICartRepository cartRepository, IOrderRepository orderRepository, IOrderProductRepository orderProductRepository, IProductDetailRepository productDetailRepository, IBillRepository billRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _productDetailRepository = productDetailRepository;
            _billRepository = billRepository;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser([FromQuery] QueryFindSoftPaginationDto queryUsers)
        {
            var users = await _userRepository.GetUsers(queryUsers);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser([FromQuery] string id)
        {
            var status = new Status();
            if(!await _userRepository.UserExists(id))
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }
            var user = await _userRepository.GetUser(id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var status = new Status();
            var idUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!await _userRepository.UserExists(idUser))
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }
            var user = await _userRepository.GetResultUser(idUser);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromQuery] string userId, [FromBody] UserBaseDto updateUser)
        {
            var status = new Status();
            var idUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (updateUser == null)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }
            if (!await _userRepository.UserExists(userId) && idUser != userId)
            {
                status.StatusCode = 402;
                status.Message = "User does not exists";
                return BadRequest(status);
            }
            var user = await _userRepository.GetUser(userId);

            _mapper.Map(updateUser, user);

            if (!await _userRepository.UpdateUser(user))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating User";
                return BadRequest(status);
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPut("user")]
        public async Task<IActionResult> UpdateUser([FromQuery] string userId, [FromBody] UserUpdateDto updateUser)
        {
            var status = new Status();
            if (updateUser == null)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }
            if (!await _userRepository.UserExists(userId))
            {
                status.StatusCode = 402;
                status.Message = "User does not exists";
                return BadRequest(status);
            }
            if (!await _roleRepository.RoleIdExists(updateUser.IdRole))
            {
                status.StatusCode = 402;
                status.Message = "Role does not exists";
                return BadRequest(status);
            }
            var user = await _userRepository.GetUser(userId);
            var role = await _roleRepository.GetRole(updateUser.IdRole);

            _mapper.Map(updateUser, user);
            user.Role = role;

            if (!await _userRepository.UpdateUser(user))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating User";
                return BadRequest(status);
            }
            return Ok(user);
        }

        [Authorize]
        [HttpPatch("change-password")]
        public async Task<IActionResult> UpdatePassword([FromQuery] string userId, [FromBody] UserChangePassWordDto changePassUser)
        {
            var status = new Status();
            if (changePassUser == null)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }
            if (!await _loginRepository.CheckPassword(HashPassword.HashPass(changePassUser.Password)))
            {
                status.StatusCode = 400;
                status.Message = "Change password fail";
                return BadRequest(status);
            }

            user.Password = HashPassword.HashPass(changePassUser.NewPassword);

            if (!await _userRepository.UpdateUser(user))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating User";
                return BadRequest(status);
            }

            return Ok(user);
        }

        [HttpPatch("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDto resetPassword)
        {
            var status = new Status();
            if (resetPassword == null)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(resetPassword.Token);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }

            user.Password = HashPassword.HashPass(resetPassword.NewPassword);

            if (!await _userRepository.UpdateUser(user))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating User";
                return BadRequest(status);
            }

            return Ok(user);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)
        {
            var status = new Status();
            if (!await _userRepository.UserExists(userId))
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }

            var carts = await _cartRepository.GetCarts(userId);
            if (carts != null && carts.Count > 0)
            {
                if (!await _cartRepository.DeleteCarts(carts))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong deleting carts of user";
                    return BadRequest(status);
                }
            }

            var orders = await _orderRepository.GetOrders(userId);
            if (orders != null && orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    if(order.Status != (int)OrderStatus.Completed)
                    {
                        foreach (var op in order.OrderProducts)
                        {
                            if(op.Status != (int)OrderStatus.Cancelled)
                            {
                                var productDetail = await _productDetailRepository.GetProductDetail(op.Product.ProductDetailId);
                                if (productDetail != null)
                                {
                                    productDetail.Quantity += op.Quantity;
                                    productDetail.Sold -= op.Quantity;

                                    if (!await _productDetailRepository.UpdateProductDetail(productDetail))
                                    {
                                        status.StatusCode = 500;
                                        status.Message = "Something went wrong while updating product detail";
                                        return BadRequest(status);
                                    }
                                }
                            }
                            var orderProductUpdated = await _orderProductRepository.DeleteOrderProduct(op);
                            if (orderProductUpdated == null)
                            {
                                status.StatusCode = 500;
                                status.Message = "Something went wrong while deleting order product of user";
                                return BadRequest(status);
                            }
                        }
                    }
                }
            }

            var removeOrders = await _orderRepository.DeleteOrders(orders);
            if (removeOrders == null)
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong while deleting orders of user";
                return BadRequest(status);
            }

            var bills = await _billRepository.GetBills(userId);
            foreach(var bill in bills)
            {
                bill.User = null;
                if(!await _billRepository.UpdateBill(bill))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong updating bill";
                    return BadRequest(status);
                }
            }

            var user = await _userRepository.GetUser(userId);

            if (!await _userRepository.DeleteUser(user))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong deleting User";
                return BadRequest(status);
            }

            return Ok(user);
        }

    }
}
