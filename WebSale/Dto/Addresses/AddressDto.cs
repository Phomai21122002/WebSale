namespace WebSale.Dto.Addresses
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public long? Code { get; set; }
        public bool IsDefault { get; set; }
    }
}
