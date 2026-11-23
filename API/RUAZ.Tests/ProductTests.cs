using Moq;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Interfaces;
using RUAZ.Application.Services;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;

namespace RUAZ.Tests
{
    public class ProductTests
    {
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly ProductService _sut;

        public ProductTests()
        {
            _sut = new ProductService(_productRepository.Object);
        }

        [Fact]
        public async Task Create_ValidProduct_SetsOwnerAndAvailableStatus()
        {
            var ownerId = Guid.NewGuid();
            Product? saved = null;

            _productRepository
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => saved = p)
                .Returns(Task.CompletedTask);

            var result = await _sut.CreateAsync(new CreateProductRequest
            {
                Title = "  Tênis Street  ",
                Description = "  Edição limitada ",
                Price = 299.90m,
                Stock = 5,
                Category = ProductsCategory.Streetwear,
                Image = [1, 2, 3]
            }, ownerId);

            Assert.NotNull(saved);
            Assert.Equal(ownerId, saved.OwnerId);
            Assert.Equal("Tênis Street", saved.Title);
            Assert.Equal(StatusProduct.Available, saved.Status);
            Assert.Equal(StatusProduct.Available, result.Status);
            Assert.Equal(ownerId, result.OwnerId);
            _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task Create_ZeroStock_SetsExhaustedStatus()
        {
            var ownerId = Guid.NewGuid();

            _productRepository
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.CreateAsync(new CreateProductRequest
            {
                Title = "Jaqueta",
                Description = "Preta",
                Price = 150m,
                Stock = 0,
                Category = ProductsCategory.Fashion
            }, ownerId);

            Assert.Equal(StatusProduct.Exhausted, result.Status);
            Assert.Empty(result.Image);
        }

        [Theory]
        [InlineData("", 10, 1, ProductsCategory.Fashion)]
        [InlineData("   ", 10, 1, ProductsCategory.Fashion)]
        [InlineData("Camisa", -1, 1, ProductsCategory.Fashion)]
        [InlineData("Camisa", 10, -1, ProductsCategory.Fashion)]
        [InlineData("Camisa", 10, 1, (ProductsCategory)999)]
        public async Task Create_InvalidInput_ThrowsBadRequestException(
            string title,
            decimal price,
            int stock,
            ProductsCategory category)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _sut.CreateAsync(new CreateProductRequest
                {
                    Title = title,
                    Price = price,
                    Stock = stock,
                    Category = category
                }, Guid.NewGuid()));

            _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_NormalizesPaginationAndForwardsFilters()
        {
            var products = new List<Product>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Short",
                    Price = 89.90m,
                    Stock = 3,
                    Category = ProductsCategory.Sportive,
                    Status = StatusProduct.Available,
                    OwnerId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            };

            _productRepository
                .Setup(r => r.GetPagedAsync(1, 100, ProductsCategory.Sportive, StatusProduct.Available))
                .ReturnsAsync((products, 1));

            var result = await _sut.GetAllAsync(new ProductFilterRequest
            {
                Page = 0,
                PageSize = 500,
                Category = ProductsCategory.Sportive,
                Status = StatusProduct.Available
            });

            Assert.Equal(1, result.Page);
            Assert.Equal(100, result.PageSize);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Short", result.Items[0].Title);
        }

        [Fact]
        public async Task GetById_Existing_ReturnsProduct()
        {
            var id = Guid.NewGuid();
            var product = new Product
            {
                Id = id,
                Title = "Boné",
                Price = 59.90m,
                Stock = 2,
                Category = ProductsCategory.Streetwear,
                Status = StatusProduct.Available,
                OwnerId = Guid.NewGuid()
            };

            _productRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(product);

            var result = await _sut.GetByIdAsync(id);

            Assert.Equal(id, result.Id);
            Assert.Equal("Boné", result.Title);
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _productRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id));
        }

        [Fact]
        public async Task Update_ByOwner_UpdatesProductAndStatus()
        {
            var ownerId = Guid.NewGuid();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Antigo",
                Description = "Desc antiga",
                Price = 100m,
                Stock = 5,
                Image = [9, 9],
                Category = ProductsCategory.Fashion,
                Status = StatusProduct.Available,
                OwnerId = ownerId
            };

            _productRepository
                .Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);
            _productRepository
                .Setup(r => r.UpdateAsync(product))
                .Returns(Task.CompletedTask);

            var result = await _sut.UpdateAsync(product.Id, new UpdateProductRequest
            {
                Title = "Novo Título",
                Description = "Nova Desc",
                Price = 120m,
                Stock = 0,
                Category = ProductsCategory.Sportive,
                Image = null
            }, ownerId);

            Assert.Equal("Novo Título", result.Title);
            Assert.Equal(120m, result.Price);
            Assert.Equal(0, result.Stock);
            Assert.Equal(StatusProduct.Exhausted, result.Status);
            Assert.Equal([9, 9], result.Image);
            Assert.NotNull(result.UpdatedAt);
            _productRepository.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task Update_ByNonOwner_ThrowsForbiddenException()
        {
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Produto",
                OwnerId = ownerId
            };

            _productRepository
                .Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _sut.UpdateAsync(product.Id, new UpdateProductRequest
                {
                    Title = "Alterado",
                    Price = 10m,
                    Stock = 1,
                    Category = ProductsCategory.Fashion
                }, otherUserId));

            _productRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ByOwner_DeletesProduct()
        {
            var ownerId = Guid.NewGuid();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Produto",
                OwnerId = ownerId
            };

            _productRepository
                .Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);
            _productRepository
                .Setup(r => r.DeleteAsync(product))
                .Returns(Task.CompletedTask);

            await _sut.DeleteAsync(product.Id, ownerId, TypeActivity.User);

            _productRepository.Verify(r => r.DeleteAsync(product), Times.Once);
        }

        [Fact]
        public async Task Delete_ByAdminOnOtherUsersProduct_DeletesProduct()
        {
            var ownerId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Produto de terceiro",
                OwnerId = ownerId
            };

            _productRepository
                .Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);
            _productRepository
                .Setup(r => r.DeleteAsync(product))
                .Returns(Task.CompletedTask);

            await _sut.DeleteAsync(product.Id, adminId, TypeActivity.Admin);

            _productRepository.Verify(r => r.DeleteAsync(product), Times.Once);
        }

        [Theory]
        [InlineData(TypeActivity.User)]
        [InlineData(TypeActivity.Store)]
        public async Task Delete_ByNonOwnerNonAdmin_ThrowsForbiddenException(TypeActivity role)
        {
            var ownerId = Guid.NewGuid();
            var otherId = Guid.NewGuid();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Produto de terceiro",
                OwnerId = ownerId
            };

            _productRepository
                .Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _sut.DeleteAsync(product.Id, otherId, role));

            _productRepository.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}
