using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSale.Dto.Auth;
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

        public UserController(IMapper mapper, IUserRepository userRepository, ILoginRepository loginRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _loginRepository = loginRepository;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userRepository.GetUsers();
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
            var user = await _userRepository.GetUser(idUser);
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
            if (!await _userRepository.UserExists(userId))
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }
            var user = await _userRepository.GetUser(userId);
            if (user == null || !await _loginRepository.CheckPassword(HashPassword.HashPass(changePassUser.Password)))
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

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)
        {
            var status = new Status();
            var admin = User.FindFirst(ClaimTypes.Role)?.Value;
            if(admin != "Admin")
            {
                status.StatusCode = 403;
                status.Message = "You do not have permission";
                return BadRequest(status);
            }
            if (!await _userRepository.UserExists(userId))
            {
                status.StatusCode = 402;
                status.Message = "User not exists";
                return BadRequest(status);
            }
            var user = await _userRepository.GetUser(userId);

            if (!await _userRepository.DeleteUser(user))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating User";
                return BadRequest(status);
            }

            return Ok(user);
        }

    }
}
