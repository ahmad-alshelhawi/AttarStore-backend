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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<CategoryMapper[]>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            if (categories == null || !categories.Any())
                return NotFound("No categories found.");

            return Ok(_mapper.Map<CategoryMapper[]>(categories));
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByCategoryIdAsync(id);
            if (category == null) return NotFound(new { status = "Category not found" });

            return Ok(_mapper.Map<CategoryMapper>(category));
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryCreateMapper categoryCreate, IFormFile? imageFile)
        {
            if (categoryCreate == null)
                return BadRequest("Invalid category data.");

            var category = _mapper.Map<Category>(categoryCreate);

            // Save image if provided
            if (imageFile != null)
            {
                category.ImageUrl = await SaveImageAsync(imageFile);
            }

            await _categoryRepository.AddCategory(category);
            var categoryDto = _mapper.Map<CategoryMapper>(category);

            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateMapper categoryUpdate, IFormFile? imageFile)
        {
            if (categoryUpdate == null)
                return BadRequest("Invalid category data.");

            var existingCategory = await _categoryRepository.GetByCategoryIdAsync(id);
            if (existingCategory == null)
                return NotFound("Category not found.");

            _mapper.Map(categoryUpdate, existingCategory);

            // Save new image if provided
            if (imageFile != null)
            {
                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(existingCategory.ImageUrl))
                {
                    DeleteImage(existingCategory.ImageUrl);
                }

                existingCategory.ImageUrl = await SaveImageAsync(imageFile);
            }

            await _categoryRepository.UpdateCategory(existingCategory);
            return NoContent();
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetByCategoryIdAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            // Delete the image if it exists
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                DeleteImage(category.ImageUrl);
            }

            await _categoryRepository.DeleteCategory(id);
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
