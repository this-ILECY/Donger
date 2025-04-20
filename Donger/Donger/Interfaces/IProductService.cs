using Donger.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donger.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
        // You might later add methods like GetProductsBySubInvoiceIdAsync
    }
}