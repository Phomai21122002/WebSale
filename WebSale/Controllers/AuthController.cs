using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebSale.Dto.Auth;
using WebSale.Dto.Email;
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
        private readonly IEmailService _emailService;
        public AuthController(ILoginRepository loginRepository, IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository, ITokenService tokenService, IEmailService emailService)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _emailService = emailService;
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
                status.StatusCode = 403;
                status.Message = "Invalid user";
                return BadRequest(status);
            }

            if (!owner.ConfirmEmail) {
                owner.Code = new Random().Next(100000, 999999);
                if (!await _userRepository.UpdateUser(owner)) {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong updating User";
                    return BadRequest(status);
                }
                var message = new Message(new string[] { loginDto.Email }, "Mã xác nhận tài khoản", $"Mã xác nhận của bạn là: {owner.Code}");
                _emailService.SendEmail(message);

                return Ok(new
                {
                    isConfirmEmail = owner.ConfirmEmail,
                    message = "Please verify your email to activate your account."
                });
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
            var loginSuccess = new
            {
                isConfirmEmail = owner.ConfirmEmail,
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
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            var user = await _loginRepository.CheckUserByEmail(registrationDto?.Email);
            if (user != null)
            {
                status.StatusCode = 409;
                status.Message = "Valid user";
                return BadRequest(status);
            }

            registrationDto.Password = HashPassword.HashPass(registrationDto.Password);

            var userMap = _mapper.Map<User>(registrationDto);
            userMap.url = "";
            userMap.Code = new Random().Next(100000, 999999);
            userMap.TwoFactorEnabled = true;
            userMap.Role = await _roleRepository.GetRole(idRole);
            userMap.CreatedAt = DateTime.Now;

            if (!await _userRepository.CreateUser(userMap))
            {
                status.StatusCode = 500;
                status.Message = "saving fail";
                return BadRequest(status);
            }

            var message = new Message(new string[] { userMap.Email }, "Mã xác nhận tài khoản", $"Mã xác nhận của bạn là: {userMap.Code}");
            _emailService.SendEmail(message);

            status.StatusCode = 200;
            status.Message = "User created successfully";
            return Ok(status);
        }

        [HttpPost("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] int code)
        {
            var status = new Status();
            if (string.IsNullOrEmpty(email) || code == 0)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            var owner = await _userRepository.CheckCode(email, code);

            if (owner == null)
            {
                status.StatusCode = 404;
                status.Message = "Invalid email and code";
                return BadRequest(status);
            }

            owner.ConfirmEmail = true;

            if (!await _userRepository.UpdateUser(owner))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating User";
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

        [HttpGet("google-login")]
        public IActionResult LoginWithGoogle([FromQuery] string returnUrl = "/")
        {
            var redirectUrl = Url.Action("GoogleResponse", new { returnUrl });

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse([FromQuery] string returnUrl = "/")
        {
            Console.WriteLine($"User logged in via Google:)");

            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
                return Unauthorized("Xác thực Google không thành công.");
            Console.WriteLine($"User logged in via Google");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // 👉 Nếu muốn tạo user tại đây, có thể làm:
            // var user = await _userManager.FindByEmailAsync(email);
            // if (user == null) { tạo mới user tại đây... }

            Console.WriteLine($"User logged in via Google: {name} ({email})");

            return Redirect(returnUrl);
        }
    }
    
}
