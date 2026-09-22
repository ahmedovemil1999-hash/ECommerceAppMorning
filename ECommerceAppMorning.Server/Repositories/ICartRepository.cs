using ECommerceAppMorning.Server.Entities;

namespace ECommerceAppMorning.Server.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<CartItem?> GetCartItemAsync(int userId, int productId);
        Task AddCartAsync(Cart cart);
        void RemoveCartItem(CartItem cartItem);
        void RemoveCartItems(IEnumerable<CartItem> cartItems);
        Task SaveChangesAsync();
    }
}