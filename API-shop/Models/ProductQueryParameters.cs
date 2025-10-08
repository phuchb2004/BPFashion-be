namespace API_shop.Models
{
    public class ProductQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool IsSortAscending { get; set; } = true;
    }
}
