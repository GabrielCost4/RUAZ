using RUAZ.Domain.Enums;

namespace RUAZ.Application.Contracts
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public byte[] Image { get; set; } = [];
        public Guid OwnerId { get; set; }
        public ProductsCategory Category { get; set; }
        public StatusProduct Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
