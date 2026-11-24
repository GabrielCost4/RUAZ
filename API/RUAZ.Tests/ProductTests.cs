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

        [Fact]
        public async Task GetAll_NormalizesPaginationAndForwardsFilters()
        {
            var item = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Short",
                Category = ProductsCategory.Sportive,
                OwnerId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            item.SetPrice(89.90m);
            item.SetStock(3);

            var products = new List<Product> { item };

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
            Assert.Equal(89.90m, result.Items[0].Price);
            Assert.Equal(3, result.Items[0].Stock);
            Assert.Equal(StatusProduct.Available, result.Items[0].Status);
        }

        [Fact]
        public async Task GetById_Existing_ReturnsProduct()
        {
            var id = Guid.NewGuid();
            var product = new Product
            {
                Id = id,
                Title = "Boné",
                Category = ProductsCategory.Streetwear,
                OwnerId = Guid.NewGuid()
            };
            product.SetPrice(59.90m);
            product.SetStock(2);

            _productRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(product);

            var result = await _sut.GetByIdAsync(id);

            Assert.Equal(id, result.Id);
            Assert.Equal("Boné", result.Title);
            Assert.Equal(59.90m, result.Price);
            Assert.Equal(2, result.Stock);
            Assert.Equal(StatusProduct.Available, result.Status);
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
                Image = [9, 9],
                Category = ProductsCategory.Fashion,
                OwnerId = ownerId
            };
            product.SetPrice(100m);
            product.SetStock(5);

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
        public async Task Update_ByNonOwner_IncludingAdmin_ThrowsForbiddenException()
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
        public async Task Update_NotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _productRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _sut.UpdateAsync(id, new UpdateProductRequest
                {
                    Title = "Produto",
                    Price = 10m,
                    Stock = 1,
                    Category = ProductsCategory.Fashion
                }, Guid.NewGuid()));
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

        [Fact]
        public async Task Delete_NotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _productRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _sut.DeleteAsync(id, Guid.NewGuid(), TypeActivity.Admin));
        }

        [Fact]
        public void Product_SetPrice_Negative_ThrowsArgumentOutOfRangeException()
        {
            var product = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => product.SetPrice(-0.01m));
        }

        [Fact]
        public void Product_SetStock_Negative_ThrowsArgumentOutOfRangeException()
        {
            var product = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => product.SetStock(-1));
        }

        [Fact]
        public void Product_SetStock_UpdatesStatusAutomatically()
        {
            var product = new Product();

            product.SetStock(3);
            Assert.Equal(3, product.Stock);
            Assert.Equal(StatusProduct.Available, product.Status);

            product.SetStock(0);
            Assert.Equal(0, product.Stock);
            Assert.Equal(StatusProduct.Exhausted, product.Status);
        }
    }
}
