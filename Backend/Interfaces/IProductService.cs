using System.Collections.Generic;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface IProductService
    {
        public abstract Product RegisterProduct(Product product);
        public abstract Product UpdateProduct(int id, Product updatedProduct);
        public abstract Product UpdateProductQuantity(int id, int amount);
        public abstract bool DeleteProduct(int id);
        public abstract Product GetProductByName(string name);
        public abstract Product GetProductById(int id);
        public abstract List<Product> GetProducts();
    }
}