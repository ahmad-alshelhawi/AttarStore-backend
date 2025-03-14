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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<CategoryMapper[]>> GetAllCategories()
        {
            var category = await _categoryRepository.GetAllCategories();
            if (category == null || !category.Any())
                return NotFound("No admins found.");

            return Ok(_mapper.Map<CategoryMapper[]>(category));
        }
        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByCategoryIdAsync(id);
            if (category == null) return NotFound(new { status = "Category not found" });

            return Ok(_mapper.Map<CategoryMapper>(category));
        }

        [HttpGet("AddCategory")]

        public async Task<IActionResult> AddCategory(CategoryCreateMapper categoryCreate)
        {
            if (categoryCreate == null)
                return BadRequest("Invalid category data.");

            var category = _mapper.Map<Category>(categoryCreate); // Mapping DTO -> Entity
            await _categoryRepository.AddCategory(category);

            var categoryDto = _mapper.Map<CategoryCreateMapper>(category); // Convert back to DTO for response
            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateMapper categoryUpdate)
        {
            if (categoryUpdate == null /*|| id != categoryUpdate.Id*/)
                return BadRequest("Invalid category data.");

            var existingCategory = await _categoryRepository.GetByCategoryIdAsync(id);
            if (existingCategory == null)
                return NotFound("Category not found.");

            _mapper.Map(categoryUpdate, existingCategory); // Map updated fields onto the existing entity
            await _categoryRepository.UpdateCategory(existingCategory);

            return NoContent();
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetByCategoryIdAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            await _categoryRepository.DeleteCategory(id);
            return NoContent();
        }

    }
}
