using API_shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetProductsByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var query = _context.Products
                .Where(p => p.categoryId == categoryId)
                .Include(p => p.category)
        .Include(p => p.ProductVariants)
          .ThenInclude(v => v.Color)
        .Include(p => p.ProductVariants)
          .ThenInclude(v => v.Size)
        .Select(p => new
        {
            p.productId,
            p.productName,
            p.description,
            p.imageUrl,
            p.material,
            p.createdAt,
            CategoryName = p.category.categoryName,
            p.price,

            Variants = p.ProductVariants.Select(v => new
            {
                v.variantId,
                v.price,
                v.stockQuantity,
                Color = v.Color.colorName,
                Size = v.Size.sizeName
            })
        });

            var products = await query.ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found for this category.");
            }

            return Ok(products);
        }

        [HttpGet("GetProductsPaged")]
        public async Task<IActionResult> GetProductsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var query = _context.Products
                .Include(p => p.category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Size)
                .Select(p => new
                {
                    p.productId,
                    p.productName,
                    p.description,
                    p.imageUrl,
                    p.material,
                    p.createdAt,
                    CategoryName = p.category.categoryName,
                    p.price,

                    Variants = p.ProductVariants.Select(v => new
                    {
                        v.variantId,
                        v.price,
                        v.stockQuantity,
                        Color = v.Color.colorName,
                        Size = v.Size.sizeName
                    })
                });

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { products = products, totalCount = totalCount });
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Where(p => p.productId == id)
                .Include(p => p.category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Size)
                .Select(p => new
                {
                    p.productId,
                    p.productName,
                    p.description,
                    p.imageUrl,
                    p.material,
                    p.createdAt,
                    CategoryName = p.category.categoryName,
                    p.price,

                    Variants = p.ProductVariants.Select(v => new
                    {
                        v.variantId,
                        v.price,
                        v.stockQuantity,
                        colorId = v.colorId,
                        ColorName = v.Color.colorName,
                        sizeId = v.sizeId,
                        SizeName = v.Size.sizeName
                    })
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}