using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using Backend.Models.Exceptions;
using Backend.Services;
using BackendTest.Fixtures;
using Xunit;

namespace BackendTest.Services
{
    public class ProductServiceTests : IClassFixture<GenericFixture<ProductService>>
    {
        private readonly GenericFixture<ProductService> _fixture;
        public ProductServiceTests(GenericFixture<ProductService> fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(90)]
        public async Task GetProductById_PassedProductId_ReturnAProductWithGivenId(int productId)
        {
            // When
            var product = await _fixture.Service.GetProductById(productId);
            // Then
            Assert.Equal(productId, product.Id);
        }

        [Fact]
        public async Task GetProductById_PassedNonExistingProductId_ThrowsRegisterNotFoundException()
        {
            int productId = _fixture.Context.Products.Count() + 1;
            // When
            var act = async () => await _fixture.Service.GetProductById(productId);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData("asc")]
        [InlineData("desc")]
        public async Task GetProducts_WithSortingInSearchRequest_ReturnsAllProductsSorted(string sortBy)
        {
            // Given
            var searchRequest = new SearchProductRequest()
            {
                NameFilter = null,
                SortBy = sortBy,
                Page = null
            };
            // When
            var products = await _fixture.Service.GetProducts(searchRequest);
            // Then
            if (sortBy == "asc")
                Assert.Equal<List<Product>>(_fixture.Context.Products.OrderBy(p => p.Name).ToList(), products);
            if (sortBy == "desc")
                Assert.Equal<List<Product>>(_fixture.Context.Products.OrderByDescending(p => p.Name).ToList(), products);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task GetProducts_WithPageInSearchRequest_ReturnsTenSpecificProducts(int page)
        {
            // Given
            var searchRequest = new SearchProductRequest()
            {
                NameFilter = null,
                SortBy = null,
                Page = page
            };
            // When
            var products = await _fixture.Service.GetProducts(searchRequest);
            // Then
            Assert.Equal<List<Product>>(_fixture.Context.Products.Skip(10 * (page - 1)).Take(10).ToList(), products);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetProducts_WithFilterNameInSearchRequest_ReturnsFilteredProductsByName(int productId)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);
            var nameFilter = product.Name.Substring(1, 3);
            var searchRequest = new SearchProductRequest()
            {
                NameFilter = nameFilter,
                SortBy = null,
                Page = null
            };
            // When
            var products = await _fixture.Service.GetProducts(searchRequest);
            // Then
            Assert.Equal<List<Product>>(_fixture.Context.Products.Where(p => p.Name.Contains(nameFilter)).ToList(), products);
        }

        [Fact]
        public async Task RegisterProduct_WhenCalled_CreateANewProductInDb()
        {
            // Given
            var product = new Product
            {
                Id = _fixture.Context.Products.Count() + 1,
                Name = "CreateProduct",
                Description = "Created product description",
                StockQuantity = 10,
                Price = 200,
            };
            // When
            await _fixture.Service.RegisterProduct(product);
            // Then
            Assert.Equal<Product>(_fixture.Context.Products.Find(product.Id), product);
        }

        [Theory]
        [InlineData(50)]
        public async Task RegisterProduct_WhenProductNameAlreadyExists_ThrowRegisteredProductException(int productId)
        {
            // Given
            var existingProduct = _fixture.Context.Products.Find(productId);
            var creatingProduct = new Product()
            {
                Id = _fixture.Context.Products.Count() + 1,
                Name = existingProduct.Name,
                StockQuantity = 10,
                Price = 200,
            };
            // When
            var act = async () => await _fixture.Service.RegisterProduct(creatingProduct);
            // Then
            await Assert.ThrowsAsync<RegisteredProductException>(act);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(40)]
        [InlineData(80)]
        public async Task UpdateProduct_WhenCalled_UpdateProductInDb(int productId)
        {
            // Given
            var updateProductRequest = new UpdateProductRequest()
            {
                Name = "UpdateProduct" + productId,
                StockQuantity = 10,
                Description = "Updated product description",
                Price = 200,
            };
            // When
            var updatedProduct = await _fixture.Service.UpdateProduct(productId, updateProductRequest);
            // Then
            Assert.Equal<Product>(_fixture.Context.Products.Find(productId), updatedProduct);
        }

        [Fact]
        public async Task UpdateProduct_WhenCalledWithAnNonExistingProductId_ThrowsRegisterNotFoundException()
        {
            // Given
            var updateProductRequest = new UpdateProductRequest();
            var productId = _fixture.Context.Products.Count() + 1;
            // When
            var act = async () => await _fixture.Service.UpdateProduct(productId, updateProductRequest);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateProduct_WhenCalledWithAnExistingNameForDifferentId_ThrowsRegisteredProductException(int productId)
        {
            // Given
            var updateProductRequest = new UpdateProductRequest()
            {
                Name = _fixture.Context.Products.Find(productId).Name,
            };
            // When
            var act = async () => await _fixture.Service.UpdateProduct(productId - 1, updateProductRequest);
            // Then
            await Assert.ThrowsAsync<RegisteredProductException>(act);
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateProduct_WhenCalledWithAnExistingNameForSameId_UpdateProductInDb(int productId)
        {
            // Given
            var updateProductRequest = new UpdateProductRequest()
            {
                Name = _fixture.Context.Products.Find(productId).Name,
                StockQuantity = 100,
                Price = 150,
                Description = "Update product description"
            };
            // When
            var updatedProduct = await _fixture.Service.UpdateProduct(productId, updateProductRequest);
            // Then
            Assert.Equal<Product>(_fixture.Context.Products.Find(productId), updatedProduct);
        }

        [Theory]
        [InlineData(10, -5)]
        public async Task UpdateProductQuantity_WhenCalled_UpdateProductQuantityInDb(int productId, int quantity)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);
            var initialQuantity = product.StockQuantity;
            // When
            await _fixture.Service.UpdateProductQuantity(productId, quantity);
            // Then
            Assert.Equal(initialQuantity - quantity, _fixture.Context.Products.Find(productId).StockQuantity);
        }

        [Fact]
        public async Task UpdateProductQuantity_WhenCalledWithAnNonExistingProductId_ThrowsRegisterNotFoundException()
        {
            // Given
            var productId = _fixture.Context.Products.Count() + 1;
            // When
            var act = async () => await _fixture.Service.UpdateProductQuantity(productId, 1);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateProductQuantity_WhenStockQuantityIsNotEnough_ThrowsOutOfStockException(int productId)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);
            // When
            var act = async () => await _fixture.Service.UpdateProductQuantity(productId, product.StockQuantity.Value + 1);
            // Then
            await Assert.ThrowsAsync<OutOfStockException>(act);
        }

        [Fact]
        public async Task DeleteProduct_WhenCalled_DeleteUserFromDb()
        {
            // Given
            var productId = _fixture.Context.Products.Count() + 1;
            var productToDelete = new Product()
            {
                Id = productId,
                Name = "Deleting Product",
                StockQuantity = 10,
                Price = 200
            };
            _fixture.Context.Products.Add(productToDelete);
            _fixture.Context.SaveChanges();
            // When
            await _fixture.Service.DeleteProduct(productId);
            // Then
            Assert.True(!_fixture.Context.Products.Any(product => productId == product.Id));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task DeleteProduct_WhenProductIsInAnCartItem_ThrowsNotAllowedDeletionException(int itemId)
        {
            // Given
            var productId = _fixture.Context.Items.Find(itemId).ProductId;
            // When
            var act = async () => await _fixture.Service.DeleteProduct(productId.Value);
            // Then
            await Assert.ThrowsAsync<NotAllowedDeletionException>(act);
        }

        [Fact]
        public async Task DeleteProduct_WhenCalledWithAnNonExistingProductId_ThrowsRegisterNotFoundException()
        {
            // Given
            var productId = _fixture.Context.Products.Count() + 1;
            // When
            var act = async () => await _fixture.Service.DeleteProduct(productId);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }
    }
}