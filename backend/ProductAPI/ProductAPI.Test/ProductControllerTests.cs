using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProductAPI.Controllers;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Tests
{
    public class ProductControllerTests
    {
        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.ProductTypes.Add(new ProductType { Id = 1, Name = "Sofa" });
            context.Colors.AddRange(
                new Color { Id = 1, Name = "Blue", Hex = "#0000FF" },
                new Color { Id = 2, Name = "Red", Hex = "#FF0000" }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryContext();
            var logger = new NullLogger<ProductController>();
            var controller = new ProductController(context, logger);

            var result = await controller.GetProduct(999);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedResult_WhenValidRequest()
        {
            using var context = CreateInMemoryContext();
            var logger = new NullLogger<ProductController>();
            var controller = new ProductController(context, logger);
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                ProductTypeId = 1,
                Colors = new List<int> { 1, 2 }
            };

            var result = await controller.CreateProduct(request);

            result.Should().BeOfType<CreatedAtActionResult>();
            context.Products.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenInvalidProductType()
        {
            using var context = CreateInMemoryContext();
            var logger = new NullLogger<ProductController>();
            var controller = new ProductController(context, logger);
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                ProductTypeId = 999, // Invalid ID
                Colors = new List<int> { 1 }
            };

            var result = await controller.CreateProduct(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public async Task GetProduct_ReturnsProductInfo_WhenProductExists()
        {
            using var context = CreateInMemoryContext();
            var logger = new NullLogger<ProductController>();
            var controller = new ProductController(context, logger);

            // Add a test product
            var product = new Product
            {
                Name = "Test Product",
                ProductTypeId = 1,
                Description = "Test Description",
                Img = "test.jpg"
            };
            product.Colors.Add(context.Colors.First());
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var result = await controller.GetProduct(product.Id);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<ProductInfo>();

            var productInfo = okResult.Value as ProductInfo;
            productInfo!.Name.Should().Be("Test Product");
            productInfo.Description.Should().Be("Test Description");
            productInfo.ProductType.Name.Should().Be("Sofa");
            productInfo.Colors.Should().HaveCount(1);
        }
    }
}