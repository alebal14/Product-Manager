namespace ProductAPI.DTOs
{
    public record ProductListItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Img { get; set; } = string.Empty;
        public required ProductTypeInfo ProductType { get; set; }
        public required List<ColorInfo> Colors { get; set; }
    }
}
