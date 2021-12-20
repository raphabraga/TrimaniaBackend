using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("products")]
    [Authorize(Roles = "Administrator")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService service)
        {
            _productService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_productService.GetProducts());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetProductsById(int id)
        {
            return Ok(_productService.GetProductById(id));
        }

        [HttpPost]
        public IActionResult RegisterProduct([FromBody] Product product)
        {
            return Ok(_productService.RegisterProduct(product));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (!_productService.DeleteProduct(id))
                return BadRequest("There is no product registered with this ID on the database");
            return Ok("The product was successfully deleted from the database.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            Product product = _productService.UpdateProduct(id, updatedProduct);
            if (product == null)
                return BadRequest("There is no product registered with this ID on the database");
            return Ok("The product was successfully updated on the database.");
        }
    }
}