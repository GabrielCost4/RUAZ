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
            ValidateInput(request.Title, request.Price, request.Stock, request.Category);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Price = request.Price,
                Stock = request.Stock,
                Category = request.Category,
                Image = request.Image ?? [],
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            product.SyncStatusFromStock();

            await productRepository.AddAsync(product);
            return MapToResponse(product);
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
                Items = items.Select(MapToResponse).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id)
        {
            var product = await GetExistingProductAsync(id);
            return MapToResponse(product);
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, Guid currentUserId)
        {
            var product = await GetExistingProductAsync(id);

            if (product.OwnerId != currentUserId)
            {
                throw new ForbiddenException("Apenas o criador do produto pode editá-lo.");
            }

            ValidateInput(request.Title, request.Price, request.Stock, request.Category);

            product.Title = request.Title.Trim();
            product.Description = request.Description?.Trim() ?? string.Empty;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.Category = request.Category;
            if (request.Image is not null)
            {
                product.Image = request.Image;
            }
            product.UpdatedAt = DateTime.UtcNow;
            product.SyncStatusFromStock();

            await productRepository.UpdateAsync(product);
            return MapToResponse(product);
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

        private static void ValidateInput(string title, decimal price, int stock, ProductsCategory category)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new BadRequestException("O título do produto é obrigatório.");
            }

            if (price < 0)
            {
                throw new BadRequestException("O preço do produto não pode ser negativo.");
            }

            if (stock < 0)
            {
                throw new BadRequestException("O estoque do produto não pode ser negativo.");
            }

            if (!Enum.IsDefined(category))
            {
                throw new BadRequestException("Categoria do produto inválida.");
            }
        }

        private static ProductResponse MapToResponse(Product product) => new()
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Image = product.Image,
            OwnerId = product.OwnerId,
            Category = product.Category,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
