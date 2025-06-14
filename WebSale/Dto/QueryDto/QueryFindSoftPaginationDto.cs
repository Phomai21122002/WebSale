namespace WebSale.Dto.QueryDto
{
    public class QueryFindSoftPaginationDto
    {
        public string? Name { get; set; }
        public string? SortBy { get; set; } = null;
        public bool isDecsending { get; set; } = false;
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
}
