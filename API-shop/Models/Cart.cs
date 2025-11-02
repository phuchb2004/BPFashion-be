using System.ComponentModel.DataAnnotations.Schema;

namespace API_shop.Models
{
    public class Cart
    {
        public int cartId { get; set; }
        public int userId { get; set; }
        public DateTime createAt { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<CartItem> cartItems { get; set; }
    }

    public class CartItem
    {
        public int cartItemId { get; set; }

        public int cartId { get; set; }
        [ForeignKey("cartId")]
        public virtual Cart cart { get; set; }

        public int variantId { get; set; }

        [ForeignKey("variantId")]
        public virtual ProductVariant ProductVariant { get; set; }

        public int quantity { get; set; }
    }
}
