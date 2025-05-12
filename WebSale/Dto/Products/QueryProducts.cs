namespace WebSale.Dto.Products
{
    public class QueryProducts
    {
        public string Name { get; set; } = string.Empty;
        public string? SortBy { get; set; } = null;
        public bool isDecsending { get; set; } = false;
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
}
