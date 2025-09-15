namespace API_shop.Models
{
    public class Cart
    {
        public int cartId {  get; set; }
        public int userId { get; set; }
        public DateTime createAt { get; set; }
        public ICollection<CartItem> cartItems { get; set; }
    }

    public class CartItem
    {
        public int cartItemId { get; set; }
        public int cartId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public Cart cart { get; set; }
        public Product product { get; set; }
    }
}
