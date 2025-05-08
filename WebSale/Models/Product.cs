using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WebSale.Data;
using WebSale.Interfaces;

namespace WebSale.Models
{
    public class Product : BaseTime
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }
        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Price must be non-negative")]
        public long Price { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Slug { get; set; }
        public int ProductDetailId { get; set; }

        public ICollection<Cart>? Carts { get; set; }
        public ICollection<OrderProduct>? OrderProducts { get; set; }
        public ICollection<ImageProduct>? ImageProducts { get; set; }
        public ICollection<FeedBack>? FeedBacks { get; set; }
        public Category? Category { get; set; }
        public ProductDetail? ProductDetail { get; set; }

        public void GenerateSlug()
        {
            Slug = Regex.Replace(RemoveDiacritics(Name.ToLower().Trim()), @"[^a-z0-9\s-]", "")
                .Replace(" ", "-");
        }

        private string RemoveDiacritics(string text)
        {
            return new string(text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}
