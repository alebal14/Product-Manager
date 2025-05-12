using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("colors")]
        public async Task<IActionResult> GetColors()
        {
            var colors = await _context.Colors.ToListAsync();
            return Ok(colors);
        }

        [HttpGet("product-types")]
        public async Task<IActionResult> GetProductTypes()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();
            return Ok(productTypes);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var query = _context.Products
                 .Include(p => p.ProductType)
                 .Include(p => p.Colors)
                 .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Img = p.Img,
                    ProductType = new ProductTypeInfo
                    {
                        Name = p.ProductType.Name
                    },
                    Colors = p.Colors.Select(c => new ColorInfo
                    {
                        Name = c.Name,
                        Hex = c.Hex
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new { data = products, totalCount, page, pageSize });
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var productInfo = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductInfo
                {
                    Id = p.Id,
                    Name = p.Name,
                    Img = p.Img,
                    Description = p.Description,
                    ProductType = new ProductTypeInfo
                    {
                        Name = p.ProductType.Name
                    },
                    Colors = p.Colors.Select(c => new ColorInfo
                    {
                        Name = c.Name,
                        Hex = c.Hex
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return productInfo == null ? NotFound() : Ok(productInfo);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest productQuery)
        {

            var productTypeExists = await _context.ProductTypes
            .AnyAsync(pt => pt.Id == productQuery.ProductTypeId);

            if (!productTypeExists)
            {
                _logger.LogError($"Product type with ID {productQuery.ProductTypeId} not found");
                return BadRequest($"Product type with ID {productQuery.ProductTypeId} not found");
            }

            var colors = await _context.Colors
                .Where(c => productQuery.Colors.Contains(c.Id))
                .ToListAsync();

            if (colors.Count != productQuery.Colors.Count)
            {
                var foundIds = colors.Select(c => c.Id).ToList();
                var missingIds = productQuery.Colors.Except(foundIds).ToList();
                _logger.LogError($"Colors with IDs {string.Join(", ", missingIds)} not found");
                return BadRequest($"Colors with IDs {string.Join(", ", missingIds)} not found");
            }

            var newProduct = new Product
            {
                Name = productQuery.Name,
                ProductTypeId = productQuery.ProductTypeId,
                Img = productQuery.Img,
                Description = productQuery.Description,
                Colors = colors
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
        }
    }
}