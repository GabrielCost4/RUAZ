using Microsoft.EntityFrameworkCore;
using RUAZ.Application.Interfaces;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;
using RUAZ.Infrastructure.Data;

namespace RUAZ.Infrastructure.Repositories
{
    public class ProductRepository(
        AppDbContext _context
    ) : IProductRepository
    {
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            ProductsCategory? category,
            StatusProduct? status)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
