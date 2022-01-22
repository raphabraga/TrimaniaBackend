using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;
using System.Threading.Tasks;

namespace Backend.Interfaces.Services
{
    public interface IProductService
    {
        public abstract Task<Product> RegisterProduct(Product product);
        public abstract Task<Product> UpdateProduct(int id, UpdateProductRequest updateProduct);
        public abstract Task<Product> UpdateProductQuantity(int id, int amount);
        public abstract Task DeleteProduct(int id);
        public abstract Task<Product> GetProductByName(string name);
        public abstract Task<Product> GetProductById(int id);
        public abstract Task<List<Product>> GetProducts(string filter, string sort, int? page);
    }
}