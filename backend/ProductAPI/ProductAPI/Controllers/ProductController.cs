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

        /// <summary>
        /// Retrieves all available colors
        /// </summary>
        /// <remarks>
        /// This endpoint returns all colors that can be associated with products.
        /// Each color includes an ID, name, and hex value.
        /// </remarks>
        /// <returns>A list of all colors</returns>
        /// <response code="200">Returns the list of colors</response>
        [HttpGet("colors")]
        public async Task<IActionResult> GetColors()
        {
            var colors = await _context.Colors.ToListAsync();
            return Ok(colors);
        }

        /// <summary>
        /// Retrieves all available product types
        /// </summary>
        /// <remarks>
        /// This endpoint returns all product types that can be assigned to products.
        /// Examples might include "Sofa", "Chair", "Table", etc.
        /// </remarks>
        /// <returns>A list of all product types</returns>
        /// <response code="200">Returns the list of product types</response>
        [HttpGet("product-types")]
        public async Task<IActionResult> GetProductTypes()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();
            return Ok(productTypes);
        }

        /// <summary>
        /// Retrieves a paginated list of all products
        /// </summary>
        /// <remarks>
        /// This endpoint returns products with their associated product type and colors.
        /// Products are ordered by creation date with the newest first.
        /// Pagination is supported through page and pageSize parameters.
        /// </remarks>
        /// <param name="page">The page number (default: 1)</param>
        /// <param name="pageSize">The number of items per page (default: 20)</param>
        /// <returns>A paginated list of products with metadata</returns>
        /// <response code="200">Returns the paginated list of products</response>
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

        /// <summary>
        /// Retrieves a specific product by ID
        /// </summary>
        /// <remarks>
        /// This endpoint returns detailed information about a product including
        /// its name, product type, and associated colors.
        /// </remarks>
        /// <param name="id">The ID of the product to retrieve</param>
        /// <returns>The requested product details</returns>
        /// <response code="200">Returns the product details</response>
        /// <response code="404">If the product is not found</response>
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

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <remarks>
        /// This endpoint creates a new product with the specified name, product type, and colors.
        /// All referenced product types and colors must exist in the database.
        /// 
        /// Sample request:
        ///
        ///     POST /api/products
        ///     {
        ///        "name": "LANDSKRONA",
        ///        "productTypeId": 1,
        ///        "colors": [1, 2],
        ///        "img": "image-url.jpg",
        ///        "description": "Comfortable sofa"
        ///     }
        ///
        /// </remarks>
        /// <param name="productQuery">The product details</param>
        /// <returns>The created product</returns>
        /// <response code="201">Returns the newly created product</response>
        /// <response code="400">If the request is invalid (missing product type or colors)</response>
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