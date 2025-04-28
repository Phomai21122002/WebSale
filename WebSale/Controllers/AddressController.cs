using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSale.Dto.Addresses;
using WebSale.Dto.Carts;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IAddressUserRepository _addressUserRepository;

        public AddressController(IMapper mapper, IUserRepository userRepository, IAddressRepository addressRepository, IAddressUserRepository addressUserRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _addressUserRepository = addressUserRepository;
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses([FromQuery] string inputUserId)
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
                var addresses = await _addressRepository.GetAddressesByUserId(userId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("provinves")]
        public async Task<IActionResult> GetProvinces([FromQuery] string inputUserId)
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
                var provinces = await _addressRepository.GetProvinces();
                return Ok(provinces);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts([FromQuery] string inputUserId, [FromQuery] string codeParent)
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
                var districts = await _addressRepository.GetDistricstByParentCode(codeParent);
                return Ok(districts);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpGet("wards")]
        public async Task<IActionResult> GetWards([FromQuery] string inputUserId, [FromQuery] string codeParent)
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
                var wards = await _addressRepository.GetWardsByParentCode(codeParent);
                return Ok(wards);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromQuery] string inputUserId, [FromBody] AddressCreateDto addressDto)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (addressDto == null || userId != inputUserId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }
                var user = await _userRepository.GetUser(userId);
                var addressMap = _mapper.Map<Address>(addressDto);
                var newAddress = await _addressRepository.CreateAddress(addressMap);
                if(newAddress == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating address";
                    return BadRequest(status);
                }

                var addressUser = new UserAddress
                {
                    IsDefault = false,
                    User = user,
                    Address = newAddress,
                };
                if(!await _addressUserRepository.CreateUserAddress(addressUser))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating address user";
                    return BadRequest(status);
                }

                return Ok(newAddress);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromQuery] string inputUserId, [FromQuery] int addressId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (addressId == null || userId != inputUserId || !await _addressUserRepository.UserAddressExists(inputUserId, addressId))
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }
                var user = await _userRepository.GetUser(userId);
                var userAddress = await _addressUserRepository.GetUserAddress(inputUserId, addressId);
                if (userAddress.IsDefault == true) {
                    return Ok(userAddress);
                }

                var allUserAddress = await _addressUserRepository.GetAllUserAddress(userId);
                foreach (var addr in allUserAddress)
                {
                    addr.IsDefault = false;
                }
                
                if (!await _addressUserRepository.UpdateAllUserAddress(allUserAddress))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating address user";
                    return BadRequest(status);
                }

                userAddress.IsDefault = true;
                var updateUserAddress = await _addressUserRepository.UpdateUserAddress(userAddress);
                if(updateUserAddress == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating address user";
                    return BadRequest(status);
                }

                return Ok(updateUserAddress);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.InnerException?.Message ?? ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveAddress([FromQuery] string inputUserId, [FromQuery] int addressId)
        {
            var status = new Status();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (addressId == null || userId != inputUserId)
                {
                    status.StatusCode = 400;
                    status.Message = "Please complete all required fields with accurate and complete information.";
                    return BadRequest(status);
                }
                var address = await _addressRepository.GetAddress(userId, addressId);
                var removeAddress = await _addressRepository.DeleteAddress(address);
                if (removeAddress == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while removing address of user";
                    return BadRequest(status);
                }

                return Ok(removeAddress);
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
