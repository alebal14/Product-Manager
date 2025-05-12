using System.Text.Json.Serialization;

namespace ProductAPI.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string Name { get; set; }
    }
}
