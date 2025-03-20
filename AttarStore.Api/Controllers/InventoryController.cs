using AttarStore.Entities;
using AttarStore.Models;
using AttarStore.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public InventoryController(IInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventories = await _inventoryRepository.GetAllInventories();
            return Ok(_mapper.Map<InventoryMapper[]>(inventories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var inventory = await _inventoryRepository.GetInventoryByIdAsync(id);
            if (inventory == null) return NotFound();
            return Ok(_mapper.Map<InventoryMapper>(inventory));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateInventoryMapper createInventoryMapper)
        {
            var inventory = _mapper.Map<Inventory>(createInventoryMapper);
            await _inventoryRepository.AddInventory(inventory);
            return CreatedAtAction(nameof(GetById), new { id = inventory.Id }, createInventoryMapper);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateInventoryMapper updateInventoryMapper)
        {
            var inventory = await _inventoryRepository.GetInventoryByIdAsync(id);
            if (inventory == null) return NotFound();
            _mapper.Map(updateInventoryMapper, inventory);
            await _inventoryRepository.UpdateInventory(inventory);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _inventoryRepository.DeleteInventory(id);
            return NoContent();
        }
    }
}
