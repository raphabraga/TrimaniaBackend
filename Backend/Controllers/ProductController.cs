using System;
using System.Collections.Generic;
using System.Net;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Exceptions;
using Backend.Models.ViewModels;
using Backend.Utils;
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
        public IActionResult GetProducts([FromQuery(Name = "sort")] string sort,
        [FromQuery(Name = "page")] int? page)
        {
            try
            {
                List<Product> products = _productService.GetProducts(filter: null, sort, page);
                if (products == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductsNotFound));
                return Ok(products);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(new ErrorMessage(e, HttpStatusCode.NotFound));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public IActionResult SearchProducts([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            try
            {
                List<Product> products = _productService.GetProducts(filter, sort, page);
                if (products == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductsNotFound));
                return Ok(products);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(new ErrorMessage(e, HttpStatusCode.NotFound));
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
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
                return Ok(product);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(new ErrorMessage(e, HttpStatusCode.NotFound));
            }
        }

        [HttpPost]
        public IActionResult RegisterProduct([FromBody] Product newProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                Product product = _productService.RegisterProduct(newProduct);
                return CreatedAtAction(nameof(ProductsById), new { id = product.Id }, product);
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (RegisteredProductException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(new ErrorMessage(e, HttpStatusCode.UnprocessableEntity));
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] UpdateProduct updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (_productService.GetProductById(id) == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
                return Ok(_productService.UpdateProduct(id, updatedProduct));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (RegisteredProductException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(new ErrorMessage(e, HttpStatusCode.UnprocessableEntity));
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(new ErrorMessage(e, HttpStatusCode.NotFound));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                if (_productService.GetProductById(id) == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
                _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (NotAllowedDeletionException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(new ErrorMessage(e, HttpStatusCode.UnprocessableEntity));
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(new ErrorMessage(e, HttpStatusCode.NotFound));
            }
        }

    }
}