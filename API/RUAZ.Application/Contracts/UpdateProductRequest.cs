using RUAZ.Domain.Enums;

namespace RUAZ.Application.Contracts
{
    public class UpdateProductRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ProductsCategory Category { get; set; }
        public byte[]? Image { get; set; }
    }
}
