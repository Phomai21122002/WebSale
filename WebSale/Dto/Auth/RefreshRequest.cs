namespace WebSale.Dto.Auth
{
    public class RefreshRequest
    {
        public string ExpiredToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
