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
        public long? Phone {  get; set; }
    }
}
