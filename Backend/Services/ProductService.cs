using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Backend.Data;
using Backend.Interfaces.Services;
using Backend.Interfaces.UnitOfWork;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Exceptions;
using Backend.Models.ViewModels;
using Backend.Utils;

namespace Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Product GetProductByName(string name)
        {
            try
            {
                return _unitOfWork.ProductRepository.GetBy(product => product.Name == name);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public Product GetProductById(int id)
        {
            try
            {
                return _unitOfWork.ProductRepository.GetBy(product => product.Id == id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public List<Product> GetProducts(string filter, string sort, int? queryPage)
        {
            try
            {
                Expression<Func<Product, bool>> predicateFilter = null;
                Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null;
                if (!string.IsNullOrEmpty(filter))
                    predicateFilter = product => product.Name.Contains(filter);
                if (sort == "asc")
                    orderBy = q => q.OrderBy(product => product.Name);
                else if (sort == "desc")
                    orderBy = q => q.OrderByDescending(product => product.Name);

                return _unitOfWork.ProductRepository.Get(predicateFilter, orderBy, null, queryPage).ToList();
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
                    throw new RegisteredProductException(ErrorMessage.GetMessage(ErrorType.UniqueProductName));
                _unitOfWork.ProductRepository.Insert(product);
                _unitOfWork.Commit();
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
                    throw new RegisteredProductException(ErrorMessage.GetMessage(ErrorType.UniqueProductName));
                product = GetProductById(id);
                product.Name = string.IsNullOrEmpty(updateProduct.Name) ? product.Name : updateProduct.Name;
                product.Price = updateProduct.Price == null ? product.Price : updateProduct.Price.Value;
                product.StockQuantity = updateProduct.StockQuantity == null ? product.StockQuantity : updateProduct.StockQuantity;
                product.Description = updateProduct.Description == null ? product.Description : updateProduct.Description;
                _unitOfWork.Commit();
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
                    throw new OutOfStockException(ErrorMessage.GetMessage(ErrorType.InsufficientProductInStock));
                product.StockQuantity -= amount;
                _unitOfWork.Commit();
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
                if (_unitOfWork.ChartItemRepository.GetBy(item => item.Product.Id == id) != null)
                    throw new NotAllowedDeletionException(ErrorMessage.GetMessage(ErrorType.DeleteProductInRegisteredOrder));
                _unitOfWork.ProductRepository.Delete(id);
                _unitOfWork.Commit();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}