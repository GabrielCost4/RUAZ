using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;

namespace RUAZ.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            ProductsCategory? category,
            StatusProduct? status);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
