using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Data;
using Backend.Interfaces;
using Backend.Models;
using Backend.Models.Exceptions;
using Backend.Models.ViewModels;
using Backend.Utils;

namespace Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationContext _applicationContext;

        public ProductService(ApplicationContext context)
        {
            _applicationContext = context;
            try
            {
                _applicationContext.Database.EnsureCreated();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public Product GetProductByName(string name)
        {
            try
            {
                return _applicationContext.Products.FirstOrDefault(product => product.Name == name);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public Product GetProductById(int id)
        {
            return _applicationContext.Products.FirstOrDefault(product => product.Id == id);
        }
        public List<Product> GetProducts(string filter, string sort, int? queryPage)
        {
            int perPage = 10;
            try
            {
                List<Product> products = _applicationContext.Products.ToList();
                if (!string.IsNullOrEmpty(filter))
                    products = products.Where(product => product.Name.CaseInsensitiveContains(filter)).ToList();
                if (sort == "asc")
                    products = products.OrderBy(product => product.Name).ToList();
                else if (sort == "des")
                    products = products.OrderByDescending(product => product.Name).ToList();
                int page = queryPage.GetValueOrDefault(1) == 0 ? 1 : queryPage.GetValueOrDefault(1);
                products = products.Skip(perPage * (page - 1)).Take(perPage).ToList();
                return products;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public Product RegisterProduct(Product product)
        {
            try
            {
                if (GetProductByName(product.Name) != null)
                    throw new RegisteredProductException("Product already registered on the database with this name.");
                _applicationContext.Products.Add(product);
                _applicationContext.SaveChanges();
                return product;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public Product UpdateProduct(int id, UpdateProduct updateProduct)
        {
            try
            {
                Product product = GetProductByName(updateProduct.Name);
                if (product != null && product.Id != id)
                    throw new RegisteredProductException("Product already registered on the database with this name.");
                product = GetProductById(id);
                product.Name = string.IsNullOrEmpty(updateProduct.Name) ? product.Name : updateProduct.Name;
                product.Price = updateProduct.Price == null ? product.Price : updateProduct.Price.Value;
                product.StockQuantity = updateProduct.StockQuantity == null ? product.StockQuantity : updateProduct.StockQuantity;
                product.Description = updateProduct.Description == null ? product.Description : updateProduct.Description;
                _applicationContext.SaveChanges();
                return product;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public Product UpdateProductQuantity(int id, int amount)
        {
            try
            {
                Product product = GetProductById(id);
                if (product.StockQuantity < amount)
                    throw new OutOfStockException("The quantity ordered exceed the number of the product in stock");
                product.StockQuantity -= amount;
                _applicationContext.SaveChanges();
                return product;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                Product product = GetProductById(id);
                if (_applicationContext.Items.Any(item => item.Product.Id == id))
                    throw new NotAllowedDeletionException("Product belongs to one or more registered chart items. Deletion is forbidden");
                _applicationContext.Remove(product);
                _applicationContext.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}