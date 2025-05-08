using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebSale.Models
{
    public class ProductDetail : BaseTime
    {
        private static readonly string DescriptionFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        [Key]
        public int Id { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? DescriptionDetail { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }
        [MaxLength(255)]
        public string? Tag { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Sold must be non-negative")]
        public int Sold { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Product? Product { get; set; }

        public void SaveDescriptionToFile()
        {
            Directory.CreateDirectory(DescriptionFolder);

            var sanitizedFileName = Regex.Replace(Description.ToLower().Trim(), @"[^a-z0-9\s-]", "")
                .Replace(" ", "_")
                .Replace("--", "_");

            var uniqueIdentifier = Guid.NewGuid();
            string fileName = $"{sanitizedFileName}_{uniqueIdentifier}.txt";

            string filePath = Path.Combine(DescriptionFolder, fileName);

            File.WriteAllText(filePath, DescriptionDetail);

            DescriptionDetail = fileName;
        }

        public string GetDescriptionFromFile()
        {
            string filePath = Path.Combine(DescriptionFolder, DescriptionDetail);

            if (!File.Exists(filePath))
                return string.Empty;

            return File.ReadAllText(filePath);
        }

        public bool UpdateDescriptionFile(string descriptionDetail, string path)
        {
            var descriptionDetailFilePath = Path.Combine(DescriptionFolder, path);

            if (File.Exists(descriptionDetailFilePath))
            {
                File.WriteAllText(descriptionDetailFilePath, descriptionDetail);
                return true;
            }
            return false;
        }
    }
}
