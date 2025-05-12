namespace ProductAPI.DTOs
{
    public record ProductInfo : ProductListItem
    {
        public string Description { get; set; } = string.Empty;
    }
}
