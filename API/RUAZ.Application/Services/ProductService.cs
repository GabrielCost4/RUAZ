using Mapster;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Interfaces;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;

namespace RUAZ.Application.Services
{
    public class ProductService(
        IProductRepository productRepository
    )
    {
        public async Task<ProductResponse> CreateAsync(CreateProductRequest request, Guid ownerId)
        {
            var product = request.Adapt<Product>();
            product.Id = Guid.NewGuid();
            product.Title = request.Title.Trim();
            product.Description = request.Description?.Trim() ?? string.Empty;
            product.Image = request.Image ?? [];
            product.OwnerId = ownerId;
            product.CreatedAt = DateTime.UtcNow;
            product.SetPrice(request.Price);
            product.SetStock(request.Stock);

            await productRepository.AddAsync(product);
            return product.Adapt<ProductResponse>();
        }

        public async Task<PagedResult<ProductResponse>> GetAllAsync(ProductFilterRequest filter)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => filter.PageSize
            };

            var (items, totalCount) = await productRepository.GetPagedAsync(
                page,
                pageSize,
                filter.Category,
                filter.Status);

            return new PagedResult<ProductResponse>
            {
                Items = items.Adapt<List<ProductResponse>>(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id)
        {
            var product = await GetExistingProductAsync(id);
            return product.Adapt<ProductResponse>();
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, Guid currentUserId)
        {
            var product = await GetExistingProductAsync(id);

            if (product.OwnerId != currentUserId)
            {
                throw new ForbiddenException("Apenas o criador do produto pode editá-lo.");
            }

            product.Title = request.Title.Trim();
            product.Description = request.Description?.Trim() ?? string.Empty;
            product.Category = request.Category;
            if (request.Image is not null)
            {
                product.Image = request.Image;
            }
            product.SetPrice(request.Price);
            product.SetStock(request.Stock);
            product.UpdatedAt = DateTime.UtcNow;

            await productRepository.UpdateAsync(product);
            return product.Adapt<ProductResponse>();
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId, TypeActivity currentUserRole)
        {
            var product = await GetExistingProductAsync(id);

            var isOwner = product.OwnerId == currentUserId;
            var isAdmin = currentUserRole == TypeActivity.Admin;

            if (!isOwner && !isAdmin)
            {
                throw new ForbiddenException("Você não tem permissão para excluir este produto.");
            }

            await productRepository.DeleteAsync(product);
        }

        private async Task<Product> GetExistingProductAsync(Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                throw new NotFoundException("Produto não encontrado!");
            }

            return product;
        }
    }
}
