namespace ProductAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Name { get; set; }
        public string Img { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; } = null!;
        public List<Color> Colors { get; set; } = new List<Color>();
    }
}
