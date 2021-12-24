using System;
using System.Collections.Generic;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public IActionResult AllProducts([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            try
            {
                List<Product> products = _productService.GetProducts(filter, sort, page);
                if (products == null)
                    return NotFound("No registered products matches on the database.");
                return Ok(products);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult ProductsById(int id)
        {
            try
            {
                Product product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound("Product not registered on the database with this ID.");
                return Ok(product);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public IActionResult RegisterProduct([FromBody] Product newProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            if (_productService.GetProductByName(newProduct.Name) != null)
                return Conflict("Product already registered on the database with this name.");
            Product product = _productService.RegisterProduct(newProduct);
            return CreatedAtAction(nameof(ProductsById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            if (_productService.GetProductById(id) == null)
                return NotFound("Product not registered on the database with this ID.");
            Product product = _productService.GetProductByName(updatedProduct.Name);
            if (product != null && product.Id != id)
                return Conflict("Product already registered on the database with this name.");
            product = _productService.UpdateProduct(id, updatedProduct);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (_productService.GetProductById(id) == null)
                return NotFound("Product not registered on the database with this ID.");
            if (_productService.DeleteProduct(id))
                return NoContent();
            else
                return UnprocessableEntity("Product belongs to registered chart items, deletion is forbidden.");
        }

    }
}