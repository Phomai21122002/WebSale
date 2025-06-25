namespace WebSale.Dto.QueryDto
{
    public class QueryFindPaginationDto
    {
        public string? Name { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
}
