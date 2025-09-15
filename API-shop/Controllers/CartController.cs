using API_shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public IActionResult GetCart(int userId)
        {
            var cart = _context.Carts
                .Include(c => c.cartItems)
                .ThenInclude(ci => ci.product)
                .FirstOrDefault(c => c.userId == userId);

            if (cart == null)
            {
                return Ok(new List<object>());
            }

            var result = cart.cartItems.Select(ci => new
            {
                ci.cartItemId,
                ci.productId,
                ci.quantity,
                Product = new
                {
                    ci.product.productId,
                    ci.product.productName,
                    ci.product.description,
                    ci.product.price,
                    ci.product.stockQuantity,
                    ci.product.imageUrl,
                    ci.product.categoryId,
                    ci.product.brandId,
                    ci.product.material,
                    ci.product.size,
                    ci.product.createdAt
                }
            });
            return Ok(result);
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] CartItemDto dto)
        {
            var cart = _context.Carts
                .FirstOrDefault(c => c.userId == dto.userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    userId = dto.userId,
                    createAt = DateTime.Now,
                    cartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var existingItem = _context.Set<CartItem>()
                .FirstOrDefault(ci => ci.cartId == cart.cartId && ci.productId == dto.productId);

            if (existingItem != null)
            {
                existingItem.quantity += dto.quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    cartId = cart.cartId,
                    productId = dto.productId,
                    quantity = dto.quantity
                };
                _context.Add(newItem);
            }
            _context.SaveChanges();

            return Ok(new { message = "Thêm sản phẩm thành công" });
        }

        [HttpPut("UpdateCart/{cartItemId}")]
        public IActionResult UpdateQuantity(int cartItemId, [FromBody] int quantity)
        {
            var item = _context.Set<CartItem>().Find(cartItemId);
            if (item == null)
            {
                return NotFound();
            }
            
            item.quantity = quantity;
            _context.SaveChanges();

            return Ok(new { message = "Cập nhật số lượng thành công" });
        }

        [HttpDelete("DeleteItem/{cartItemId}")]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var item = _context.Set<CartItem>().Find(cartItemId);
            if (item == null)
            {
                return NotFound();
            }

            _context.Remove(item);
            _context.SaveChanges();

            return Ok(new { message = "Xóa sản phẩm thành công" });
        }

    }

    public class CartItemDto
    {
        public int userId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
    }
}
