﻿using System.ComponentModel.DataAnnotations;
using WebSale.Dto.Categories;
using WebSale.Dto.ProductDetails;

namespace WebSale.Dto.Products
{
    public class ProductResultDto : ProductDetailResultDto
    {
        public CategoryDto? category { get; set; }
    }
}
