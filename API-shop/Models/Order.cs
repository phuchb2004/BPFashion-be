namespace API_shop.Models
{
    public class Order
    {
        public int orderId { get; set; }
        public int userId { get; set; }
        public DateTime orderDate { get; set; }
        public decimal totalAmount { get; set; }
        public string status { get; set; }
        public string shippingAddress { get; set; }
        public string paymentMethod { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        public int orderDetailId { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public decimal priceAtPurchase { get; set; }
        public virtual Order Order { get; set; }
    }
}
