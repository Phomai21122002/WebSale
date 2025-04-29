using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebSale.Dto.Auth;
using WebSale.Extensions;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Respository;

namespace WebSale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITokenService _tokenService;
        public AuthController(ILoginRepository loginRepository, IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository, ITokenService tokenService)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var status = new Status();
            if (loginDto == null)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            var owner = await _loginRepository.CheckUserByEmail(loginDto.Email);
            if (owner == null || !await _loginRepository.CheckPassword(HashPassword.HashPass(loginDto.Password)))
            {
                status.StatusCode = 400;
                status.Message = "Valid user";
                return BadRequest(status);
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, owner.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, owner.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, owner.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, owner.LastName ?? string.Empty),
                new Claim(ClaimTypes.Role, owner?.Role?.Name ?? string.Empty),
            };

            var tokens = _tokenService.GetToken(claims);
            var refreshToken = _tokenService.GetRefreshToken();
            var loginSuccess = new LoginSuccess
            {
                Token = tokens.Token,
                RefreshToken = refreshToken,
                Expiration = tokens.Expiration,
                Email = owner?.Email,
                FirstName = owner?.FirstName,
                LastName = owner?.LastName,
                Phone = owner?.Phone
            };

            return Ok(loginSuccess);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromQuery] int idRole, [FromBody] RegisterDto registrationDto)
        {

            var status = new Status();
            if (registrationDto == null)
            {
                status.StatusCode = 0;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            var user = await _loginRepository.CheckUserByEmail(registrationDto?.Email);
            if (user != null)
            {
                status.StatusCode = 0;
                status.Message = "Valid user";
                return BadRequest(status);
            }

            registrationDto.Password = HashPassword.HashPass(registrationDto.Password);

            var userMap = _mapper.Map<User>(registrationDto);
            userMap.url = "";
            userMap.Role = await _roleRepository.GetRole(idRole);

            if (!await _userRepository.CreateUser(userMap))
            {
                status.StatusCode = 500;
                status.Message = "savin fail";
                return BadRequest(status);
            }

            return Ok(userMap);
        }
    }
    
}
