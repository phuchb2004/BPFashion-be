using System.ComponentModel.DataAnnotations.Schema;

namespace API_shop.Models
{
    public class Product
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string material { get; set; }
        public int? categoryId { get; set; }
        public DateTime createdAt { get; set; }
        public virtual Category category { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
