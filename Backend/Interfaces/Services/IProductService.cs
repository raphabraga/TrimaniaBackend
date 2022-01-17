using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;

namespace Backend.Interfaces.Services
{
    public interface IProductService
    {
        public abstract Product RegisterProduct(Product product);
        public abstract Product UpdateProduct(int id, UpdateProduct updateProduct);
        public abstract Product UpdateProductQuantity(int id, int amount);
        public abstract void DeleteProduct(int id);
        public abstract Product GetProductByName(string name);
        public abstract Product GetProductById(int id);
        public abstract List<Product> GetProducts(string filter, string sort, int? page);
    }
}