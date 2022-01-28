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
        public ProductService() { }

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<Product> GetProductByName(string name)
        {
            return await _unitOfWork.ProductRepository.GetBy(product => product.Name == name);
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetBy(product => product.Id == id);
            if (product == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
            return product;
        }
        public async Task<List<Product>> GetProducts(SearchProductRequest searchRequest)
        {
            Expression<Func<Product, bool>> predicateFilter = null;
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null;
            if (!string.IsNullOrEmpty(searchRequest.NameFilter))
                predicateFilter = product => product.Name.Contains(searchRequest.NameFilter);
            if (searchRequest.SortBy == "asc")
                orderBy = q => q.OrderBy(product => product.Name);
            else if (searchRequest.SortBy == "desc")
                orderBy = q => q.OrderByDescending(product => product.Name);

            var products = await _unitOfWork.ProductRepository.Get(predicateFilter, orderBy, null, searchRequest.Page);
            return products.ToList();
        }

        public async Task<Product> RegisterProduct(Product product)
        {
            if (await GetProductByName(product.Name) != null)
                throw new RegisteredProductException(ErrorUtils.GetMessage(ErrorType.UniqueProductName));
            _unitOfWork.ProductRepository.Insert(product);
            await _unitOfWork.Commit();
            return product;
        }

        public async Task<Product> UpdateProduct(int id, UpdateProductRequest updateProductRequest)
        {
            var product = await GetProductById(id);
            Product productUpdate = await GetProductByName(updateProductRequest.Name);
            if (productUpdate != null && productUpdate?.Id != id)
                throw new RegisteredProductException(ErrorUtils.GetMessage(ErrorType.UniqueProductName));
            product.Name = string.IsNullOrEmpty(updateProductRequest.Name) ? product.Name : updateProductRequest.Name;
            product.Price = updateProductRequest.Price == null ? product.Price : updateProductRequest.Price.Value;
            product.StockQuantity = updateProductRequest.StockQuantity == null ? product.StockQuantity : updateProductRequest.StockQuantity;
            product.Description = updateProductRequest == null ? product.Description : updateProductRequest.Description;
            await _unitOfWork.Commit();
            return product;
        }

        public async Task<Product> UpdateProductQuantity(int id, int amount)
        {
            var product = await GetProductById(id);
            if (product.StockQuantity < amount)
                throw new OutOfStockException(ErrorUtils.GetMessage(ErrorType.InsufficientProductInStock));
            product.StockQuantity -= amount;
            await _unitOfWork.Commit();
            return product;
        }

        public async Task DeleteProduct(int id)
        {
            Product product = await GetProductById(id);
            if (await _unitOfWork.ChartItemRepository.GetBy(item => item.Product.Id == id) != null)
                throw new NotAllowedDeletionException(ErrorUtils.GetMessage(ErrorType.DeleteProductInRegisteredOrder));
            _unitOfWork.ProductRepository.Delete(id);
            await _unitOfWork.Commit();
        }
    }
}