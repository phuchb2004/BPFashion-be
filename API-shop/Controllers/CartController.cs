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
                product = new 
                {
                    productName = ci.ProductVariant.Product.productName,
                    imageUrl = ci.ProductVariant.Product.imageUrl,
                    price = ci.ProductVariant.price,
                    color = ci.ProductVariant.Color != null ? ci.ProductVariant.Color.colorName : "",
                    size = ci.ProductVariant.Size != null ? ci.ProductVariant.Size.sizeName : "",
                    variantId = ci.ProductVariant.variantId,
                    stockQuantity = ci.ProductVariant.stockQuantity
                }
            });

            return Ok(result);
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] AddToCartDto dto)
        {
            // Validate input
            if (dto.quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng phải lớn hơn 0." });
            }

            // Find variant and check if exists
            var variant = _context.ProductVariants.Find(dto.variantId);
            if (variant == null)
            {
                return BadRequest(new { message = "Sản phẩm không tồn tại." });
            }

            // Find or create cart
            var cart = _context.Carts.FirstOrDefault(c => c.userId == dto.userId);
            if (cart == null)
            {
                cart = new Cart { userId = dto.userId, createAt = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            // Check if item already exists in cart
            var existingItem = _context.CartItems
                .FirstOrDefault(ci => ci.cartId == cart.cartId && ci.variantId == dto.variantId);

            int totalQuantity;
            if (existingItem != null)
            {
                // If item exists, calculate total quantity after adding
                totalQuantity = existingItem.quantity + dto.quantity;
            }
            else
            {
                // If new item, total quantity is the requested quantity
                totalQuantity = dto.quantity;
            }

            // Check if stock is sufficient for total quantity
            if (variant.stockQuantity < totalQuantity)
            {
                var availableStock = variant.stockQuantity - (existingItem?.quantity ?? 0);
                if (availableStock <= 0)
                {
                    return BadRequest(new { message = "Sản phẩm đã hết hàng." });
                }
                return BadRequest(new { 
                    message = $"Không đủ số lượng. Chỉ còn {availableStock} sản phẩm trong kho." 
                });
            }

            // Update or add item
            if (existingItem != null)
            {
                existingItem.quantity = totalQuantity;
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

            return Ok(new { message = "Thêm sản phẩm vào giỏ hàng thành công" });
        }

        [HttpPut("UpdateCart/{cartItemId}")]
        public IActionResult UpdateQuantity(int cartItemId, [FromBody] int quantity)
        {
            var item = _context.CartItems
                .Include(ci => ci.ProductVariant)
                .FirstOrDefault(ci => ci.cartItemId == cartItemId);
            
            if (item == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            // If quantity is 0 or negative, remove item
            if (quantity <= 0)
            {
                _context.Remove(item);
                _context.SaveChanges();
                return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng" });
            }

            // Check if variant still exists and has enough stock
            if (item.ProductVariant == null)
            {
                return BadRequest(new { message = "Sản phẩm không còn tồn tại." });
            }

            // Check stock availability
            if (item.ProductVariant.stockQuantity < quantity)
            {
                return BadRequest(new { 
                    message = $"Không đủ số lượng. Chỉ còn {item.ProductVariant.stockQuantity} sản phẩm trong kho." 
                });
            }

            // Update quantity
            item.quantity = quantity;
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
