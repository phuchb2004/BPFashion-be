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
                .Include(p => p.brand)
                .Include(p => p.category)
                .Select(p => new
                {
                    p.productId,
                    p.productName,
                    p.description,
                    p.price,
                    p.stockQuantity,
                    p.imageUrl,
                    p.material,
                    p.size,
                    p.createdAt,
                    BrandName = p.brand.brandName,
                    CategoryName = p.category.categoryName
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products
                .Include(p => p.brand)
                .Include(p => p.category)
                .Where(p => p.productId == id)
                .Select(p => new
                {
                    p.productId,
                    p.productName,
                    p.description,
                    p.price,
                    p.stockQuantity,
                    p.imageUrl,
                    p.material,
                    p.size,
                    p.createdAt,
                    BrandName = p.brand.brandName,
                    CategoryName = p.category.categoryName
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}
