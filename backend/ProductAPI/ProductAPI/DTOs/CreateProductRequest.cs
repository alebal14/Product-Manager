using System.ComponentModel.DataAnnotations;

namespace ProductAPI.DTOs

{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Name { get; set; }

        [Url]
        [StringLength(500)]
        public string Img { get; set; } = string.Empty;

        [StringLength(700)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public required int ProductTypeId { get; set; }

        [Required]
        [MinLength(1)]
        public required List<int> Colors { get; set; }
    }
}