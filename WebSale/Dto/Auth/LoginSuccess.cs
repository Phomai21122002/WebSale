using System.ComponentModel.DataAnnotations;

namespace WebSale.Dto.Auth
{
    public class LoginSuccess
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string? Phone { get; set; }
    }
}
