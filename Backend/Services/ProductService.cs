using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Backend.Dtos;
using Backend.Interfaces.Services;
using Backend.Interfaces.Repositories;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Exceptions;
using Backend.Utils;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> GetProductByName(string name)
        {
            try
            {
                return await _unitOfWork.ProductRepository.GetBy(product => product.Name == name);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<Product> GetProductById(int id)
        {
            try
            {
                return await _unitOfWork.ProductRepository.GetBy(product => product.Id == id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<List<Product>> GetProducts(string filter, string sort, int? queryPage)
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

                var products = await _unitOfWork.ProductRepository.Get(predicateFilter, orderBy, null, queryPage);
                return products.ToList();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<Product> RegisterProduct(Product product)
        {
            try
            {
                if (await GetProductByName(product.Name) != null)
                    throw new RegisteredProductException(ErrorUtils.GetMessage(ErrorType.UniqueProductName));
                _unitOfWork.ProductRepository.Insert(product);
                await _unitOfWork.Commit();
                return product;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<Product> UpdateProduct(int id, UpdateProductRequest updateProductRequest)
        {
            try
            {
                Product productUpdate = await GetProductByName(updateProductRequest.Name);
                if (productUpdate != null && productUpdate?.Id != id)
                    throw new RegisteredProductException(ErrorUtils.GetMessage(ErrorType.UniqueProductName));
                var product = await GetProductById(id);
                product.Name = string.IsNullOrEmpty(productUpdate.Name) ? product.Name : productUpdate.Name;
                product.Price = productUpdate.Price == null ? product.Price : productUpdate.Price.Value;
                product.StockQuantity = productUpdate.StockQuantity == null ? product.StockQuantity : productUpdate.StockQuantity;
                product.Description = productUpdate.Description == null ? product.Description : productUpdate.Description;
                await _unitOfWork.Commit();
                return product;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<Product> UpdateProductQuantity(int id, int amount)
        {
            try
            {
                Product product = await GetProductById(id);
                if (product.StockQuantity < amount)
                    throw new OutOfStockException(ErrorUtils.GetMessage(ErrorType.InsufficientProductInStock));
                product.StockQuantity -= amount;
                await _unitOfWork.Commit();
                return product;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task DeleteProduct(int id)
        {
            try
            {
                Product product = await GetProductById(id);
                if (await _unitOfWork.ChartItemRepository.GetBy(item => item.Product.Id == id) != null)
                    throw new NotAllowedDeletionException(ErrorUtils.GetMessage(ErrorType.DeleteProductInRegisteredOrder));
                _unitOfWork.ProductRepository.Delete(id);
                await _unitOfWork.Commit();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}