using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using WebSale.Dto.Roles;
using WebSale.Interfaces;
using WebSale.Models;

namespace WebSale.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IMapper _mapper;
        private IRoleRepository _roleRepository;
        public RoleController(IRoleRepository roleRepository, IMapper mapper) {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            var status = new Status();
            if (roleDto == null)
            {
                status.StatusCode = 402;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            if (await _roleRepository.RoleExists(roleDto.Name)) {
                status.StatusCode = 422;
                status.Message = "Role already exists";
                return BadRequest(status);
            }

            var roleMap = _mapper.Map<Role>(roleDto);
            roleMap.CreatedAt = DateTime.Now;

            if (!await _roleRepository.CreateRole(roleMap)) {
                status.StatusCode = 500;
                status.Message = "Something went wrong with creating";
                return BadRequest(status);
            }

            return Ok(roleMap);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var role = await _roleRepository.GetRoles();
            return Ok(role);
        }

        [HttpGet("role")]
        public async Task<IActionResult> GetRole([FromQuery] int idRole)
        {
            var status = new Status();

            if (!await _roleRepository.RoleIdExists(idRole))
            {
                status.StatusCode = 402;
                status.Message = "Role not exists";
                return BadRequest(status);
            }

            var role = await _roleRepository.GetRole(idRole);
            return Ok(role);
        }

        [HttpPut("role")]
        public async Task<IActionResult> UpdateRole([FromQuery] int idRole, [FromBody] RoleUpdateDto roleUpdate)
        {
            var status = new Status();

            if (roleUpdate == null || idRole != roleUpdate.Id || !ModelState.IsValid)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required info fields";
                return BadRequest(status);
            }

            if (!await _roleRepository.RoleIdExists(idRole))
            {
                status.StatusCode = 402;
                status.Message = "Role not exists";
                return BadRequest(status);
            }

            var roleMap = _mapper.Map<Role>(roleUpdate);
            roleMap.UpdatedAt = DateTime.Now;

            if(!await _roleRepository.UpdateRole(roleMap))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong updating Role";
                return BadRequest(status);
            }
            var roleUpdated = await _roleRepository.GetRole(idRole);
            return Ok(roleUpdated);
        }

        [HttpDelete("role")]
        public async Task<IActionResult> DeleteRole([FromQuery] int idRole)
        {
            var status = new Status();

            if (!await _roleRepository.RoleIdExists(idRole))
            {
                status.StatusCode = 402;
                status.Message = "Role not exists";
                return BadRequest(status);
            }

            var role = await _roleRepository.GetRole(idRole);
            role.DeletedAt = DateTime.Now;


            if (!await _roleRepository.DeleteRole(role))
            {
                status.StatusCode = 500;
                status.Message = "Something went wrong deleting Role";
                return BadRequest(status);
            }
            return Ok(role);
        }
    }
}
