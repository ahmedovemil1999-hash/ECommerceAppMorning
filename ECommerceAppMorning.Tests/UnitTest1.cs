using ECommerceAppMorning.Server.DTOs.Cart;
using ECommerceAppMorning.Server.Entities;
using ECommerceAppMorning.Server.Repositories;
using ECommerceAppMorning.Server.Services;
using Moq;
using Xunit;

namespace ECommerceAppMorning.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _cartService = new CartService(_cartRepoMock.Object);
        }

        [Fact]
        public async Task GetCartAsync_ShouldReturnEmptyCart_WhenCartDoesNotExist()
        {
            int userId = 1;
            _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId))
                         .ReturnsAsync((Cart?)null);

            var result = await _cartService.GetCartAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }

        [Fact]
        public async Task AddToCartAsync_ShouldReturnBadRequest_WhenQuantityIsZeroOrNegative()
        {
            var dto = new AddToCartDto { ProductId = 1, Quantity = 0 };

            var (success, errorMessage) = await _cartService.AddToCartAsync(1, dto);

            Assert.False(success);
            Assert.Equal("Quantity must be greater than zero.", errorMessage);
        }

        [Fact]
        public async Task AddToCartAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var dto = new AddToCartDto { ProductId = 99, Quantity = 1 };
            _cartRepoMock.Setup(r => r.GetProductByIdAsync(dto.ProductId))
                         .ReturnsAsync((Product?)null);

            var (success, errorMessage) = await _cartService.AddToCartAsync(1, dto);

            Assert.False(success);
            Assert.Equal("Product not found.", errorMessage);
        }

        [Fact]
        public async Task AddToCartAsync_ShouldAddNewItem_WhenCartAndStockAreValid()
        {
            int userId = 1;
            var dto = new AddToCartDto { ProductId = 1, Quantity = 2 };
            var product = new Product { Id = 1, Stock = 10 };
            var cart = new Cart { UserId = userId, Items = new List<CartItem>() };

            _cartRepoMock.Setup(r => r.GetProductByIdAsync(dto.ProductId))
                         .ReturnsAsync(product);
            _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId))
                         .ReturnsAsync(cart);

            var (success, errorMessage) = await _cartService.AddToCartAsync(userId, dto);

            Assert.True(success);
            Assert.Empty(errorMessage);
            Assert.Single(cart.Items);
            _cartRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}