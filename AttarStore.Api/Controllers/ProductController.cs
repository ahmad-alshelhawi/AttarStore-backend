using AttarStore.Entities;
using AttarStore.Models;
using AttarStore.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AttarStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            return Ok(_mapper.Map<ProductMapper[]>(products));
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(_mapper.Map<ProductMapper>(product));
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductCreateMapper productCreateMapper)
        {
            if (productCreateMapper == null)
                return BadRequest("Invalid product data.");

            var product = _mapper.Map<Product>(productCreateMapper);
            await _productRepository.AddProduct(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, _mapper.Map<ProductMapper>(product));
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateMapper productUpdateMapper)
        {
            if (productUpdateMapper == null || id != productUpdateMapper.Id)
                return BadRequest("Invalid product data.");

            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            _mapper.Map(productUpdateMapper, existingProduct);
            await _productRepository.UpdateProduct(existingProduct);

            return NoContent();
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            await _productRepository.DeleteProduct(id);
            return NoContent();
        }
    }
}
