using System.ComponentModel.DataAnnotations;

namespace API_shop.Models
{
    public class ProductVariant
    {
        [Key]
        public int variantId { get; set; }
        public int productId { get; set; }
        public int colorId { get; set; }
        public int sizeId { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public virtual Product Product { get; set; }
        public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }
        public string? imageUrl { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
