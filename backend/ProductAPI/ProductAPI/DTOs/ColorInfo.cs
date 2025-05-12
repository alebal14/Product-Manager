namespace ProductAPI.DTOs
{
    public record ColorInfo
    {
        public required string Name { get; set; }
        public required string Hex { get; set; }
    }
}
