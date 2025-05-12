namespace ProductAPI.Models
{
    public class ProductColor
    {
        public required int ProductId { get; set; }
        public required int ColorId { get; set; }
        public Product Product { get; set; } = null!;
        public Color Color { get; set; } = null!;
    }
}
