using AttarStore.Entities;
using AttarStore.Models;
using AttarStore.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AttarStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository productRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ProductMapper[]>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            if (products == null || !products.Any())
                return NotFound("No products found.");

            return Ok(_mapper.Map<ProductMapper[]>(products));
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { status = "Product not found" });

            return Ok(_mapper.Map<ProductMapper>(product));
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateMapper productCreate, IFormFile? imageFile)
        {
            if (productCreate == null)
                return BadRequest("Invalid product data.");

            var product = _mapper.Map<Product>(productCreate);

            // Save image if provided
            if (imageFile != null)
            {
                product.ImageUrl = await SaveImageAsync(imageFile);
            }

            await _productRepository.AddProduct(product);
            var productDto = _mapper.Map<ProductCreateMapper>(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productDto);
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateMapper productUpdate, IFormFile? imageFile)
        {
            if (productUpdate == null)
                return BadRequest("Invalid product data.");

            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound("Product not found.");

            _mapper.Map(productUpdate, existingProduct);

            // Save new image if provided
            if (imageFile != null)
            {
                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    DeleteImage(existingProduct.ImageUrl);
                }

                existingProduct.ImageUrl = await SaveImageAsync(imageFile);
            }

            await _productRepository.UpdateProduct(existingProduct);
            return NoContent();
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            // Delete the image if it exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                DeleteImage(product.ImageUrl);
            }

            await _productRepository.DeleteProduct(id);
            return NoContent();
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file.");

            // Ensure it's an image
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/images/{uniqueFileName}"; // Returning the relative path
        }

        private void DeleteImage(string imageUrl)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
