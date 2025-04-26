using AutoMapper;
using FPS_ReviewAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSale.Dto.ProductDetails;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Respository;

namespace WebSale.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IImageProductRepository _imageProductRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IMapper mapper ,IProductRepository productRepository, IProductDetailRepository productDetailRepository, IImageProductRepository imageProductRepository, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
            _imageProductRepository = imageProductRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("product")]
        public async Task<IActionResult> GetProduct([FromQuery] int productId)
        {
            var status = new Status();
            if (!await _productRepository.ProductExists(productId))
            {
                status.StatusCode = 402;
                status.Message = "Product not exists";
                return BadRequest(status);
            }
            var product = await _productRepository.GetProductResult(productId);
            return Ok(product);
        }

        [HttpGet("productBySlug")]
        public async Task<IActionResult> GetProductBySlug([FromQuery] string slugProduct)
        {
            var status = new Status();

            if (!await _productRepository.SlugExists(slugProduct))
            {
                status.StatusCode = 402;
                status.Message = "Product not exists";
                return BadRequest(status);
            }
            var productId = await _productRepository.GetIdProductBySlug(slugProduct);
            var product = await _productRepository.GetProductResult(productId.Value);

            return Ok(product);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var status = new Status();
            var products = await _productRepository.GetProducts();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromQuery] int categoryId, [FromBody] ProductDetailDto productDetailDto)
        {
            var status = new Status();
            try
            {
                if (productDetailDto == null) {
                    status.StatusCode = 400;
                    status.Message = "Please fill in all required into fields";
                    return BadRequest(status);
                }

                if (!await _categoryRepository.CategoryExists(categoryId))
                {
                    status.StatusCode = 402;
                    status.Message = "Category does not exists";
                    return BadRequest(status);
                }

                var productDetail = _mapper.Map<ProductDetail>(productDetailDto);

                var newProductDetail = await _productDetailRepository.CreateProductDetail(productDetail);
                if (newProductDetail == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating product detail";
                    return BadRequest(status);
                }

                var category = await _categoryRepository.GetCategory(categoryId);

                var product = _mapper.Map<Product>(productDetailDto);
                product.ProductDetail = newProductDetail;
                product.Category = category;
                product.ProductDetailId = newProductDetail.Id;
                product.GenerateSlug();
                if (await _productRepository.SlugExists(product.Slug))
                {
                    product.Slug = $"{product.Slug}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                }

                var newProduct = await _productRepository.CreateProduct(product);

                if (newProduct == null)
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while creating product";
                    return BadRequest(status);
                }

                if (productDetailDto.Urls != null && productDetailDto.Urls.Any())
                {
                    if (!await _imageProductRepository.CreateImageProduct(newProduct.Id, productDetailDto.Urls))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while creating image product";
                        return BadRequest(status);
                    }
                }

                return Ok(await _productRepository.GetProductResult(newProduct.Id));
            }
            catch (Exception ex) {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromQuery] int productId, [FromQuery] int categoryId, [FromBody] ProductDetailDto productDetailDto)
        {
            var status = new Status();
            try
            {
                if (productDetailDto == null)
                {
                    status.StatusCode = 400;
                    status.Message = "Please fill in all required into fields";
                    return BadRequest(status);
                }

                if (!await _categoryRepository.CategoryExists(categoryId) && !await _productRepository.ProductExists(productId))
                {
                    status.StatusCode = 402;
                    status.Message = "Does not exists";
                    return BadRequest(status);
                }

                var product = await _productRepository.GetProduct(productId);
                _mapper.Map(productDetailDto, product);

                if (product.Category.Id != categoryId)
                {
                    var category = await _categoryRepository.GetCategory(categoryId);
                    product.Category = category;
                }
                product.GenerateSlug();

                if (!await _productRepository.UpdateProduct(product))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating product";
                    return BadRequest(status);
                }

                if (productDetailDto.Urls != null && productDetailDto.Urls.Any())
                {
                    if (!await _imageProductRepository.UpdateImageProduct(productId, productDetailDto.Urls))
                    {
                        status.StatusCode = 500;
                        status.Message = "Something went wrong while updating image product";
                        return BadRequest(status);
                    }
                }

                var productDetail = await _productDetailRepository.GetProductDetail(product.ProductDetailId);
                _mapper.Map(productDetailDto, productDetail);

                //string jsonStringproductDetail = JsonSerializer.Serialize(productDetail, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve });
                //Console.WriteLine("productDetail");
                //Console.WriteLine(jsonStringproductDetail);

                if (!await _productDetailRepository.UpdateProductDetail(productDetail))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while updating product detail";
                    return BadRequest(status);
                }

                return Ok(await _productRepository.GetProductResult(productId));
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DelelteProduct([FromQuery] int productId)
        {
            var status = new Status();
            try
            {
                if (!await _productRepository.ProductExists(productId))
                {
                    status.StatusCode = 400;
                    status.Message = "Product does not exists";
                    return BadRequest(status);
                }

                var product = await _productRepository.GetProduct(productId);

                if (!await _imageProductRepository.DeleteImageProduct(product.ImageProducts) || !await _productDetailRepository.DeleteProductDetail(product.ProductDetail))
                {
                    status.StatusCode = 500;
                    status.Message = "Something went wrong while deleting product";
                    return BadRequest(status);
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                status.StatusCode = 500;
                status.Message = $"Internal Server Error: {ex.Message}";
                return BadRequest(status);
            }
        }

        //[HttpDelete("soft-delete")]
        //public async Task<IActionResult> DelelteSoftProduct([FromQuery] int productId)
        //{
        //    var status = new Status();
        //    try
        //    {
        //        if (!await _productRepository.ProductExists(productId))
        //        {
        //            status.StatusCode = 402;
        //            status.Message = "Product does not exists";
        //            return BadRequest(status);
        //        }
        //        var product = await _productRepository.GetProduct(productId);

        //        return Ok(await _productRepository.GetProduct(productId));
        //    }
        //    catch (Exception ex)
        //    {
        //        status.StatusCode = 500;
        //        status.Message = $"Internal Server Error: {ex.Message}";
        //        return BadRequest(status);
        //    }
        //}
    }
}
