using RUAZ.Domain.Enums;

namespace RUAZ.Application.Contracts
{
    public class ProductFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public ProductsCategory? Category { get; set; }
        public StatusProduct? Status { get; set; }
    }
}
