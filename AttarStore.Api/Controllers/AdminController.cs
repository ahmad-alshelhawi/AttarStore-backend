using AttarStore.Entities;
using AttarStore.Models;
using AttarStore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AttarStore.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdminController(IAdminRepository adminRepository, IMapper mapper, IUserRepository userRepository)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet("GetAllAdmins")]
        public async Task<ActionResult<AdminMapperView[]>> GetAllAdmins()
        {
            var admins = await _adminRepository.GetAllAdmins();
            if (admins == null || !admins.Any())
                return NotFound("No admins found.");

            return Ok(_mapper.Map<AdminMapperView[]>(admins));
        }

        [HttpPost("AddNewAdmin")]
        public async Task<IActionResult> CreateAdmin(AdminMapperCreate adminCreate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _adminRepository.EmailExists(adminCreate.Email))
                return Conflict(new { status = "Email already exists" });

            var admin = _mapper.Map<Admin>(adminCreate);
            admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
            await _adminRepository.AddAdmin(admin);

            return CreatedAtAction(nameof(GetAdminById), new { id = admin.Id }, _mapper.Map<AdminMapperView>(admin));
        }

        [HttpGet("GetAdminById/{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var admin = await _adminRepository.GetAdminById(id);
            if (admin == null) return NotFound(new { status = "Admin not found" });

            return Ok(_mapper.Map<AdminMapperView>(admin));
        }

        [HttpPut("UpdateAdmin/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, AdminMapperUpdate adminUpdate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingAdmin = await _adminRepository.GetAdminById(id);
            if (existingAdmin == null) return NotFound(new { status = "Admin not found" });

            if (await _adminRepository.EmailExists(adminUpdate.Email) && existingAdmin.Email != adminUpdate.Email)
                return Conflict(new { status = "Email already exists" });

            _mapper.Map(adminUpdate, existingAdmin);
            await _adminRepository.UpdateAdminAsync(existingAdmin);

            return Ok(_mapper.Map<AdminMapperView>(existingAdmin));
        }

        [HttpDelete("DeleteAdmin/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminRepository.GetAdminById(id);
            if (admin == null) return NotFound(new { status = "Admin not found" });

            await _adminRepository.DeleteAdminAsync(admin.Id);
            return NoContent();
        }

        // Fetch profile of the currently logged-in admin
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            var admin = await _adminRepository.GetAdminById(adminId);
            if (admin == null) return NotFound(new { status = "Admin profile not found" });

            return Ok(_mapper.Map<AdminMapperView>(admin));
        }

        // Update profile for the currently logged-in admin
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(AdminProfileUpdateMapper adminUpdate)
        {
            // Get the current admin's ID from the token
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            // Get the existing admin profile
            var existingAdmin = await _adminRepository.GetAdminById(adminId);
            if (existingAdmin == null)
                return NotFound(new { status = "Admin profile not found" });

            bool isUpdated = false;

            // Check if the Name is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(adminUpdate.Name) && adminUpdate.Name != existingAdmin.Name)
            {
                var adminWithSameName = await _adminRepository.GetByAdmin(adminUpdate.Name);
                var userWithSameName = await _userRepository.GetByUser(adminUpdate.Name);

                if ((adminWithSameName != null && adminWithSameName.Id != adminId) || userWithSameName != null)
                {
                    return Conflict(new { status = "Name already exists in the system." });
                }

                existingAdmin.Name = adminUpdate.Name;
                isUpdated = true;
            }

            // Check if the Email is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(adminUpdate.Email) && adminUpdate.Email != existingAdmin.Email)
            {
                var adminWithSameEmail = await _adminRepository.GetByAdminEmail(adminUpdate.Email);
                var userWithSameEmail = await _userRepository.GetByUserOrEmail(adminUpdate.Email);

                if ((adminWithSameEmail != null && adminWithSameEmail.Id != adminId) || userWithSameEmail != null)
                {
                    return Conflict(new { status = "Email already exists in the system." });
                }

                existingAdmin.Email = adminUpdate.Email;
                isUpdated = true;
            }

            // Update Phone if it's changed
            if (!string.IsNullOrWhiteSpace(adminUpdate.Phone) && adminUpdate.Phone != existingAdmin.Phone)
            {
                existingAdmin.Phone = adminUpdate.Phone;
                isUpdated = true;
            }

            // Update Address if it's changed
            if (!string.IsNullOrWhiteSpace(adminUpdate.Address) && adminUpdate.Address != existingAdmin.Address)
            {
                existingAdmin.Address = adminUpdate.Address;
                isUpdated = true;
            }

            // If no updates were made, return a message indicating no changes
            if (!isUpdated)
                return Ok(new { status = "No changes have been made." });

            // Save the updated profile
            await _adminRepository.UpdateAdminAsync(existingAdmin);

            return Ok(new { status = "Profile updated successfully." });
        }



        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordAdmin changePassword)
        {
            // Validate input
            if (string.IsNullOrEmpty(changePassword.CurrentPassword) || string.IsNullOrEmpty(changePassword.NewPassword))
                return BadRequest(new { status = "Current password and new password are required" });

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            var admin = await _adminRepository.GetAdminById(adminId);
            if (admin == null) return NotFound(new { status = "Admin not found" });

            // Check if the current password is correct
            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, admin.Password))
                return BadRequest(new { status = "Current password is incorrect" });

            // Hash the new password
            admin.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);

            // Save the new password
            await _adminRepository.UpdateAdminAsync(admin);

            return Ok(new { status = "Password changed successfully" });
        }
    }
}
