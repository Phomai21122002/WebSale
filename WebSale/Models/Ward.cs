using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSale.Models
{
    public class Ward
    {
        [Key]
        [MaxLength(10)]
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [MaxLength(255)]
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [MaxLength(50)]
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [MaxLength(255)]
        [JsonPropertyName("name_with_type")]
        public required string Name_With_Type { get; set; }

        [MaxLength(50)]
        [JsonPropertyName("path")]
        public required string Path { get; set; }

        [MaxLength(255)]
        [JsonPropertyName("path_with_type")]
        public required string Path_With_Type { get; set; }

        [MaxLength(255)]
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [MaxLength(10)]
        [JsonPropertyName("parent_code")]
        public required string ParentCode { get; set; }

        public District? District { get; set; }
    }
}
