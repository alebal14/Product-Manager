using System.Text.Json.Serialization;

namespace ProductAPI.Models
{
    public class Color
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Name { get; set; }
        public required string Hex { get; set; }

        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
