using System.Collections.Generic;
using Backend.Dtos;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Exceptions;
using Backend.Utils;
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
        public IActionResult GetProducts([FromQuery(Name = "sort")] string sort,
        [FromQuery(Name = "page")] int? page)
        {
            List<Product> products = _productService.GetProducts(filter: null, sort, page);
            if (products == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductsNotFound));
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public IActionResult SearchProducts([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            List<Product> products = _productService.GetProducts(filter, sort, page);
            if (products == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductsNotFound));
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult ProductsById(int id)
        {
            Product product = _productService.GetProductById(id);
            if (product == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
            return Ok(product);
        }

        [HttpPost]
        public IActionResult RegisterProduct([FromBody] Product newProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Product product = _productService.RegisterProduct(newProduct);
            return CreatedAtAction(nameof(ProductsById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] UpdateProduct updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (_productService.GetProductById(id) == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
            return Ok(_productService.UpdateProduct(id, updatedProduct));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (_productService.GetProductById(id) == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
            _productService.DeleteProduct(id);
            return NoContent();
        }

    }
}