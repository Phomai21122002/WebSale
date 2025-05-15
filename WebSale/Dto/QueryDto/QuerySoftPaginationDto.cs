namespace WebSale.Dto.QueryDto
{
    public class QuerySoftPaginationDto
    {
        public bool isDecsending { get; set; } = false;
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
}
