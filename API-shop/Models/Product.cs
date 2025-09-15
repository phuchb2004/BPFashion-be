using System.ComponentModel.DataAnnotations.Schema;

namespace API_shop.Models
{
    public class Product
    {
        public int productId { get; set; }
        public string productName { get; set; } = null!;
        public string description { get; set; } = null!;
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public string imageUrl { get; set; } = null!;
        public int categoryId { get; set; }
        public int brandId { get; set; }
        public string material { get; set; } = null!;
        public string size { get; set; } = null!;
        public DateTime createdAt { get; set; }
        public Brand brand { get; set; } = null!;
        public Category category { get; set; } = null!;
    }

    [Table("Brands")]
    public class Brand
    {
        public int brandId { get; set; }
        public string brandName { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("Categories")]
    public class Category
    {
        public int categoryId { get; set; }
        public string categoryName { get; set; } = null!;
        public string? description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
