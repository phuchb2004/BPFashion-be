using API_shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Checkout")]
        public IActionResult Checkout([FromBody] CheckoutRequest request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = new Order
                {
                    userId = request.userId,
                    orderDate = DateTime.Now,
                    totalAmount = request.CartItems.Sum(item => item.priceAtPurchase * item.quantity),
                    status = "Chờ xác nhận",
                    shippingAddress = request.shippingAddress,
                    paymentMethod = request.paymentMethod,
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                var orderDetails = request.CartItems.Select(item => new OrderDetail
                {
                    orderId = order.orderId,
                    productId = item.productId,
                    quantity = item.quantity,
                    priceAtPurchase = item.priceAtPurchase
                }).ToList();

                _context.OrderDetails.AddRange(orderDetails);

                var cart = _context.Carts
                    .FirstOrDefault(c => c.userId == request.userId);

                if (cart != null)
                {
                    var cartItems = _context.CartItems
                        .Where(ci => ci.cartId == cart.cartId)
                        .ToList();

                    _context.CartItems.RemoveRange(cartItems);
                }

                _context.SaveChanges();
                transaction.Commit();

                return Ok(new
                {
                    success = true,
                    orderId = order.orderId,
                    message = "Đặt hàng thành công"
                });
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xãy ra khi xử lý đơn hàng: " + e.Message
                });
            }
        }
    }

    public class CheckoutRequest
    {
        public int userId { get; set; }
        public string shippingAddress { get; set; }
        public string paymentMethod { get; set; }
        public List<CartItemsDto> CartItems { get; set; }
    }

    public class CartItemsDto
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public decimal priceAtPurchase { get; set; }
    }
}
