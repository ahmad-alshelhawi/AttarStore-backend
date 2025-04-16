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
        private readonly IClientRepository _clientRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdminController(IAdminRepository adminRepository, IMapper mapper, IUserRepository userRepository, IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
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

            // Check name uniqueness
            var adminWithSameName = await _adminRepository.GetByAdmin(adminCreate.Name);
            var userWithSameName = await _userRepository.GetByUser(adminCreate.Name);
            var clientWithSameName = await _clientRepository.GetByClient(adminCreate.Name);
            if (adminWithSameName != null || userWithSameName != null || clientWithSameName != null)
                return Conflict(new { status = "Name already exists in the system" });

            // Check email uniqueness
            var adminWithSameEmail = await _adminRepository.GetByAdminEmail(adminCreate.Email);
            var userWithSameEmail = await _userRepository.GetByUserOrEmail(adminCreate.Email);
            var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(adminCreate.Email);
            if (adminWithSameEmail != null || userWithSameEmail != null || clientWithSameEmail != null)
                return Conflict(new { status = "Email already exists in the system" });

            var admin = _mapper.Map<Admin>(adminCreate);
            admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
            await _adminRepository.AddAdmin(admin);

            return StatusCode(201, _mapper.Map<AdminMapperView>(admin));

        }


        [HttpPut("UpdateAdmin/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, AdminMapperUpdate adminUpdate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingAdmin = await _adminRepository.GetAdminById(id);
            if (existingAdmin == null)
                return NotFound(new { status = "Admin not found" });

            // Check name uniqueness
            if (!string.IsNullOrWhiteSpace(adminUpdate.Name) && adminUpdate.Name != existingAdmin.Name)
            {
                var adminWithSameName = await _adminRepository.GetByAdmin(adminUpdate.Name);
                var userWithSameName = await _userRepository.GetByUser(adminUpdate.Name);
                var clientWithSameName = await _clientRepository.GetByClient(adminUpdate.Name);
                if ((adminWithSameName != null && adminWithSameName.Id != id) || userWithSameName != null || clientWithSameName != null)
                    return Conflict(new { status = "Name already exists in the system" });
            }

            // Check email uniqueness
            if (!string.IsNullOrWhiteSpace(adminUpdate.Email) && adminUpdate.Email != existingAdmin.Email)
            {
                var adminWithSameEmail = await _adminRepository.GetByAdminEmail(adminUpdate.Email);
                var userWithSameEmail = await _userRepository.GetByUserOrEmail(adminUpdate.Email);
                var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(adminUpdate.Email);
                if ((adminWithSameEmail != null && adminWithSameEmail.Id != id) || userWithSameEmail != null || clientWithSameEmail != null)
                    return Conflict(new { status = "Email already exists in the system" });
            }

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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token: user ID missing." });

            var adminId = int.Parse(userIdClaim.Value);
            var admin = await _adminRepository.GetAdminById(adminId);

            if (admin == null)
                return NotFound(new { message = "Admin profile not found." });

            return Ok(new
            {
                admin.Name,
                admin.Email,
                admin.Phone,
                admin.Address,
                // Add any other fields you want to expose
            });
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
                var clientWithSameName = await _clientRepository.GetByClient(adminUpdate.Name);


                if ((adminWithSameName != null && adminWithSameName.Id != adminId) || userWithSameName != null || clientWithSameName != null)
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
                var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(adminUpdate.Email);


                if ((adminWithSameEmail != null && adminWithSameEmail.Id != adminId) || userWithSameEmail != null || clientWithSameEmail != null)
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
            if (admin == null)
                return NotFound(new { status = "Admin not found" });

            // Check if the current password is correct
            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, admin.Password))
                return BadRequest(new { status = "Current password is incorrect" });

            // Check if the new password is the same as the old password
            if (BCrypt.Net.BCrypt.Verify(changePassword.NewPassword, admin.Password))
                return BadRequest(new { status = "New password cannot be the same as the current password" });

            // Update the password
            admin.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
            await _adminRepository.UpdateAdminAsync(admin);

            return Ok(new { status = "Password changed successfully" });
        }

    }
}
