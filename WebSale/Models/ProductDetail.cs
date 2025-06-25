using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
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
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Sold must be non-negative")]
        public int Sold { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Product? Product { get; set; }

        public void SaveDescriptionToFile()
        {
            Directory.CreateDirectory(DescriptionFolder);

            string rawName = string.IsNullOrWhiteSpace(Description) ? "productdetail" : Description.ToLower().Trim();

            string sanitized = RemoveDiacritics(rawName);

            sanitized = Regex.Replace(sanitized, @"[^a-z0-9\s-_]", ""); 
            sanitized = Regex.Replace(sanitized, @"\s+", "_"); 

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }

            var uniqueIdentifier = Guid.NewGuid();
            string fileName = $"{sanitized}_{uniqueIdentifier}.txt";

            string filePath = Path.Combine(DescriptionFolder, fileName);
            Console.WriteLine(DescriptionDetail);
            File.WriteAllText(filePath, DescriptionDetail ?? "");

            DescriptionDetail = fileName;
        }

        public string GetDescriptionFromFile()
        {
            if (string.IsNullOrWhiteSpace(DescriptionFolder) || string.IsNullOrWhiteSpace(DescriptionDetail))
            {
                return string.Empty;
            }
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
                Console.WriteLine(descriptionDetailFilePath);
                File.WriteAllText(descriptionDetailFilePath, descriptionDetail);
                return true;
            }
            return false;
        }

        private string RemoveDiacritics(string text)
        {
            return new string(text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}
