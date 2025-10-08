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
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(c => c.cartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Color)
                .Include(c => c.cartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Size)
                .FirstOrDefault(c => c.userId == userId);

            if (cart == null)
            {
                return Ok(new List<object>());
            }

            var result = cart.cartItems.Select(ci => new
            {
                ci.cartItemId,
                ci.variantId,
                ci.quantity,
                ProductVariant = new
                {
                    ci.ProductVariant.Product.productName,
                    ci.ProductVariant.Product.imageUrl,
                    ci.ProductVariant.price,
                    Color = ci.ProductVariant.Color.colorName,
                    Size = ci.ProductVariant.Size.sizeName,
                }
            });
            return Ok(result);
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] AddToCartDto dto)
        {
            var variant = _context.ProductVariants.Find(dto.variantId);
            if (variant == null || variant.stockQuantity < dto.quantity)
            {
                return BadRequest(new { message = "Sản phẩm không tồn tại hoặc không đủ số lượng." });
            }

            var cart = _context.Carts.FirstOrDefault(c => c.userId == dto.userId);

            if (cart == null)
            {
                cart = new Cart { userId = dto.userId, createAt = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var existingItem = _context.CartItems
                .FirstOrDefault(ci => ci.cartId == cart.cartId && ci.variantId == dto.variantId);

            if (existingItem != null)
            {
                existingItem.quantity += dto.quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    cartId = cart.cartId,
                    variantId = dto.variantId,
                    quantity = dto.quantity
                };
                _context.CartItems.Add(newItem);
            }
            _context.SaveChanges();

            return Ok(new { message = "Thêm sản phẩm thành công" });
        }

        [HttpPut("UpdateCart/{cartItemId}")]
        public IActionResult UpdateQuantity(int cartItemId, [FromBody] int quantity)
        {
            var item = _context.CartItems.Find(cartItemId);
            if (item == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                _context.Remove(item);
            }
            else
            {
                item.quantity = quantity;
            }
            _context.SaveChanges();

            return Ok(new { message = "Cập nhật số lượng thành công" });
        }

        [HttpDelete("DeleteItem/{cartItemId}")]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var item = _context.CartItems.Find(cartItemId);
            if (item == null)
            {
                return NotFound();
            }

            _context.Remove(item);
            _context.SaveChanges();

            return Ok(new { message = "Xóa sản phẩm thành công" });
        }

    }

    public class AddToCartDto
    {
        public int userId { get; set; }
        public int variantId { get; set; }
        public int quantity { get; set; }
    }
}
