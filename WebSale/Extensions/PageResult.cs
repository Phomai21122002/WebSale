namespace WebSale.Extensions
{
    public class PageResult<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage => PageNumber * PageSize < TotalCount;
        public List<T> Datas { get; set; } = new();
    }
}
