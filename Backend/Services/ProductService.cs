using System.Collections.Generic;
using System.Linq;
using Backend.Data;
using Backend.Models;

namespace Backend.Services
{
    public class ProductService
    {
        private readonly ApplicationContext _applicationContext;

        public ProductService(ApplicationContext context)
        {
            _applicationContext = context;
            _applicationContext.Database.EnsureCreated();
        }

        public Product RegisterProduct(Product product)
        {
            _applicationContext.Products.Add(product);
            _applicationContext.SaveChanges();
            return product;
        }

        public Product UpdateProduct(int id, Product updatedProduct)
        {
            Product product = GetProductById(id);
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.StockQuantity = updatedProduct.StockQuantity;
            product.Description = updatedProduct.Description;
            _applicationContext.SaveChanges();
            return product;
        }

        public Product UpdateProductQuantity(int id, int amount)
        {
            Product product = GetProductById(id);
            if (product.StockQuantity < amount)
                return null;
            product.StockQuantity -= amount;
            _applicationContext.SaveChanges();
            return product;
        }

        public bool DeleteProduct(int id)
        {
            Product product = GetProductById(id);
            if (_applicationContext.Items.Any(item => item.Product.Id == id))
                return false;
            _applicationContext.Remove(product);
            _applicationContext.SaveChanges();
            return true;
        }

        public Product GetProductByName(string name)
        {
            return _applicationContext.Products.FirstOrDefault(product => product.Name == name);
        }

        public Product GetProductById(int id)
        {
            return _applicationContext.Products.FirstOrDefault(product => product.Id == id);
        }
        public List<Product> GetProducts()
        {
            return _applicationContext.Products.ToList();
        }
    }
}