namespace API_shop.Models
{
    public class Category
    {
        public int categoryId { get; set; }
        public string categoryName { get; set; } = null!;
        public string? description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
