using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Interfaces.Services;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Authorize(Roles = "Administrator")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService service)
        {
            _productService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery(Name = "sort")] string sort,
        [FromQuery(Name = "page")] int? page)
        {
            var searchRequest = new SearchProductRequest()
            {
                NameFilter = null,
                SortBy = sort,
                Page = page,
            };
            List<Product> products = await _productService.GetProducts(searchRequest);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchProducts([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            var searchRequest = new SearchProductRequest()
            {
                NameFilter = filter,
                SortBy = sort,
                Page = page
            };
            List<Product> products = await _productService.GetProducts(searchRequest);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> ProductsById(int id)
        {
            Product product = await _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProduct([FromBody] CreateProductRequest newProduct)
        {
            var product = new Product(newProduct);
            await _productService.RegisterProduct(product);
            return CreatedAtAction(nameof(ProductsById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest updatedProduct)
        {
            return Ok(await _productService.UpdateProduct(id, updatedProduct));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }

    }
}