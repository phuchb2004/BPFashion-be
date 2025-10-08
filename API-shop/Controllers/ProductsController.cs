using API_shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products
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
                    Variants = p.ProductVariants.Select(v => new
                    {
                        v.variantId,
                        v.price,
                        v.stockQuantity,
                        Color = v.Color.colorName,
                        Size = v.Size.sizeName
                    })
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products
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
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}