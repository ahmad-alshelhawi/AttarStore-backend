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
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;

        public ManufacturerController(IManufacturerRepository manufacturerRepository, IMapper mapper)
        {
            _manufacturerRepository = manufacturerRepository ?? throw new ArgumentNullException(nameof(manufacturerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("GetAllManufacturers")]
        public async Task<IActionResult> GetAllManufacturers()
        {
            var manufacturers = await _manufacturerRepository.GetAllManufacturers();
            return Ok(_mapper.Map<ManufacturerMapper[]>(manufacturers));
        }

        [HttpGet("GetManufacturerById/{id}")]
        public async Task<IActionResult> GetManufacturerById(int id)
        {
            var manufacturer = await _manufacturerRepository.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                return NotFound();

            return Ok(_mapper.Map<ManufacturerMapper>(manufacturer));
        }

        [HttpPost("AddManufacturer")]
        public async Task<IActionResult> AddManufacturer(ManufacturerCreateMapper manufacturerCreateMapper)
        {
            if (manufacturerCreateMapper == null)
                return BadRequest("Invalid manufacturer data.");

            var manufacturer = _mapper.Map<Manufacturer>(manufacturerCreateMapper);
            await _manufacturerRepository.AddManufacturer(manufacturer);

            return CreatedAtAction(nameof(GetManufacturerById), new { id = manufacturer.Id }, _mapper.Map<ManufacturerMapper>(manufacturer));
        }

        [HttpPut("UpdateManufacturer/{id}")]
        public async Task<IActionResult> UpdateManufacturer(int id, ManufacturerUpdateMapper manufacturerUpdateMapper)
        {
            if (manufacturerUpdateMapper == null /*|| id != manufacturerUpdateMapper.Id*/)
                return BadRequest("Invalid manufacturer data.");

            var existingManufacturer = await _manufacturerRepository.GetManufacturerByIdAsync(id);
            if (existingManufacturer == null)
                return NotFound();

            _mapper.Map(manufacturerUpdateMapper, existingManufacturer);
            await _manufacturerRepository.UpdateManufacturer(existingManufacturer);

            return NoContent();
        }

        [HttpDelete("DeleteManufacturer/{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var manufacturer = await _manufacturerRepository.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                return NotFound("Manufacturer not found.");

            await _manufacturerRepository.DeleteManufacturer(id);
            return NoContent();
        }
    }
}
