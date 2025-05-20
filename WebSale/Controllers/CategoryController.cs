using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebSale.Dto.Categories;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Respository;

namespace WebSale.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageCategoryRepository _imageCategoryRepository;

        public CategoryController(IMapper mapper, ICategoryRepository categoryRepository, IImageCategoryRepository imageCategoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _imageCategoryRepository = imageCategoryRepository;
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetCategory([FromQuery] int categoryId)
        {
            var status = new Status();
            if (!await _categoryRepository.CategoryExists(categoryId))
            {
                status.StatusCode = 402;
                status.Message = "Category not exists";
                return BadRequest(status);
            }
            var category = await _categoryRepository.GetCategory(categoryId);
            return Ok(category);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategorys()
        {
            var status = new Status();
            var categories = await _categoryRepository.GetCategories();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryBase category)
        {
            var status = new Status();
            if (category == null)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required into fields";
                return BadRequest(status);
            }

            if (await _categoryRepository.CategoryNameExists(category.Name))
            {
                status.StatusCode = 409;
                status.Message = "Category already exists";
                return BadRequest(status);
            }

            try
            {
                var categoryMap = _mapper.Map<Category>(category);
                categoryMap.CreatedAt = DateTime.Now;

                var newCategory = await _categoryRepository.CreateCategory(categoryMap);
                if (newCategory == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating category";
                    return BadRequest(status); ;
                }

                if (category.Urls != null && category.Urls.Any())
                {
                    if (!await _imageCategoryRepository.CreateImagesCategory(newCategory.Id, category.Urls))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while creating image category";
                        return BadRequest(status);
                    }
                }

                return Ok(await _categoryRepository.GetCategory(newCategory.Id));
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromQuery] int idCategory, [FromBody] CategoryDto category)
        {
            var status = new Status();
            if (category == null || idCategory != category.Id)
            {
                status.StatusCode = 400;
                status.Message = "Please fill in all required into fields";
                return BadRequest(status);
            }

            if (!await _categoryRepository.CategoryExists(idCategory))
            {
                status.StatusCode = 409;
                status.Message = "Category does not exists";
                return BadRequest(status);
            }

            try
            {
                var categoryMap = _mapper.Map<Category>(category);
                categoryMap.UpdatedAt = DateTime.Now;

                var newCategory = await _categoryRepository.UpdateCategory(categoryMap);
                if (newCategory == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating category";
                    return BadRequest(status); ;
                }

                if (category.Urls != null && category.Urls.Any())
                {
                    if (!await _imageCategoryRepository.UpdateImagesCategory(newCategory.Id, category.Urls))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating image category";
                        return BadRequest(status);
                    }
                }

                return Ok(await _categoryRepository.GetCategory(idCategory));
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete("soft-delete")]
        public async Task<IActionResult> DelelteSoftCategory([FromQuery] int categoryId)
        {
            var status = new Status();
            try
            {
                if (!await _categoryRepository.CategoryExists(categoryId))
                {
                    status.StatusCode = 402;
                    status.Message = "Category does not exists";
                    return BadRequest(status);
                }
                var category = await _categoryRepository.GetCategory(categoryId);

                category.IsDeleted = true;
                category.DeletedAt = DateTime.Now;

                var categoryUpdated = await _categoryRepository.UpdateCategory(category);

                if (categoryUpdated == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating category";
                    return BadRequest(status);
                }

                return Ok(categoryUpdated);
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
