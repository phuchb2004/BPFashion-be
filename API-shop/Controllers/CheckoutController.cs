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
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in request.CartItems)
                {
                    var variant = await _context.ProductVariants.FindAsync(item.variantId);
                    if (variant == null || variant.stockQuantity < item.quantity)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new { success = false, message = $"Sản phẩm với ID {item.variantId} không đủ số lượng." });
                    }
                }

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
                await _context.SaveChangesAsync();

                foreach (var item in request.CartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        orderId = order.orderId,
                        variantId = item.variantId,
                        quantity = item.quantity,
                        priceAtPurchase = item.priceAtPurchase
                    };
                    _context.OrderDetails.Add(orderDetail);

                    var variantToUpdate = await _context.ProductVariants.FindAsync(item.variantId);
                    variantToUpdate.stockQuantity -= item.quantity;
                }

                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.userId == request.userId);
                if (cart != null)
                {
                    var cartItems = _context.CartItems.Where(ci => ci.cartId == cart.cartId);
                    _context.CartItems.RemoveRange(cartItems);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { success = true, orderId = order.orderId, message = "Đặt hàng thành công" });
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xử lý đơn hàng: " + e.Message });
            }
        }
    }

    public class CheckoutRequest
    {
        public int userId { get; set; }
        public string shippingAddress { get; set; }
        public string paymentMethod { get; set; }
        public List<CheckoutItemDto> CartItems { get; set; }
    }

    public class CheckoutItemDto
    {
        public int variantId { get; set; }
        public int quantity { get; set; }
        public decimal priceAtPurchase { get; set; }
    }
}
