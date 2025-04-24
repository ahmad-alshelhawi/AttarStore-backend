using AttarStore.Entities;
using AttarStore.Entities.submodels;
using AttarStore.Models;
using AttarStore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace AttarStore.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionRepo;
        private readonly IClientRepository _clientRepository;


        public UserController(IUserRepository userRepository, IMapper mapper, IPermissionRepository permissionRepository, IAdminRepository adminRepository, IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _mapper = mapper;
            _permissionRepo = permissionRepository;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<UserMapperView[]>> Get()
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(_mapper.Map<UserMapperView[]>(users));
        }


        //[Authorize(Roles = "Master,Admin")]
        [HttpPost("AddNewUser")]
        public async Task<IActionResult> CreateUser(UserMapperCreate userCreate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check name uniqueness
            var adminWithSameName = await _adminRepository.GetByAdmin(userCreate.Name);
            var userWithSameName = await _userRepository.GetByUser(userCreate.Name);
            var clientWithSameName = await _clientRepository.GetByClient(userCreate.Name);
            if (adminWithSameName != null || userWithSameName != null || clientWithSameName != null)
                return Conflict(new { status = "Name already exists in the system" });

            // Check email uniqueness
            var adminWithSameEmail = await _adminRepository.GetByAdminEmail(userCreate.Email);
            var userWithSameEmail = await _userRepository.GetByUserOrEmail(userCreate.Email);
            var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(userCreate.Email);
            if (adminWithSameEmail != null || userWithSameEmail != null || clientWithSameEmail != null)
                return Conflict(new { status = "Email already exists in the system" });

            var user = _mapper.Map<User>(userCreate);
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _userRepository.AddUser(user);

            return StatusCode(201, _mapper.Map<UserMapperView>(user));

        }




        [HttpGet("GetUserBy/")]
        public async Task<IActionResult> GetUserById([FromQuery] int id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserMapperCreate>(user);
            return Ok(userDto);
        }


        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserMapperUpdate userUpdate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
                return NotFound(new { status = "User not found" });

            // Check name uniqueness
            if (!string.IsNullOrWhiteSpace(userUpdate.Name) && userUpdate.Name != existingUser.Name)
            {
                var userWithSameName = await _userRepository.GetByUser(userUpdate.Name);
                var adminWithSameName = await _adminRepository.GetByAdmin(userUpdate.Name);
                var clientWithSameName = await _clientRepository.GetByClient(userUpdate.Name);

                if ((userWithSameName != null && userWithSameName.Id != id) || adminWithSameName != null || clientWithSameName != null)
                    return Conflict(new { status = "Name already exists in the system" });
            }

            // Check email uniqueness
            if (!string.IsNullOrWhiteSpace(userUpdate.Email) && userUpdate.Email != existingUser.Email)
            {
                var userWithSameEmail = await _userRepository.GetByUserOrEmail(userUpdate.Email);
                var adminWithSameEmail = await _adminRepository.GetByAdminEmail(userUpdate.Email);
                var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(userUpdate.Email);

                if ((userWithSameEmail != null && userWithSameEmail.Id != id) || adminWithSameEmail != null || clientWithSameEmail != null)
                    return Conflict(new { status = "Email already exists in the system" });
            }

            _mapper.Map(userUpdate, existingUser);
            await _userRepository.UpdateUserAsync(existingUser);

            return Ok(_mapper.Map<UserMapperView>(existingUser));
        }



        [HttpDelete("DeleteUser/")]
        public async Task<IActionResult> DeleteUserAsync([FromQuery] int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { status = "User not found" });
                }



                await _userRepository.DeleteUserAsync(user);
                return NoContent(); // 204 No Content response indicates success without returning data
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }


        //// Fetch profile of the currently logged-in admin
        //[HttpGet("GetProfile")]
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> GetProfile()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        //    if (userIdClaim == null)
        //        return Unauthorized(new { message = "Invalid token: user ID missing." });

        //    var userId = int.Parse(userIdClaim.Value);
        //    var user = await _userRepository.GetUserById(userId);

        //    if (user == null)
        //        return NotFound(new { message = "User profile not found." });

        //    return Ok(new
        //    {
        //        user.Name,
        //        user.Email,
        //        user.Phone,
        //        user.Address,
        //        // Add any other fields you want to expose
        //    });
        //}

        // Update profile for the currently logged-in admin
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateMapper userUpdate)
        {
            // Get the current admin's ID from the token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            // Get the existing admin profile
            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser == null)
                return NotFound(new { status = "User profile not found" });

            bool isUpdated = false;

            // Check if the Name is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(userUpdate.Name) && userUpdate.Name != existingUser.Name)
            {
                var adminWithSameName = await _adminRepository.GetByAdmin(userUpdate.Name);
                var userWithSameName = await _userRepository.GetByUser(userUpdate.Name);
                var clientWithSameName = await _clientRepository.GetByClient(userUpdate.Name);


                if ((userWithSameName != null && userWithSameName.Id != userId) || adminWithSameName != null || clientWithSameName != null)
                {
                    return Conflict(new { status = "Name already exists in the system." });
                }

                existingUser.Name = userUpdate.Name;
                isUpdated = true;
            }

            // Check if the Email is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(userUpdate.Email) && userUpdate.Email != existingUser.Email)
            {
                var adminWithSameEmail = await _adminRepository.GetByAdminEmail(userUpdate.Email);
                var userWithSameEmail = await _userRepository.GetByUserOrEmail(userUpdate.Email);
                var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(userUpdate.Email);


                if ((userWithSameEmail != null && userWithSameEmail.Id != userId) || adminWithSameEmail != null || clientWithSameEmail != null)
                {
                    return Conflict(new { status = "Email already exists in the system." });
                }

                existingUser.Email = userUpdate.Email;
                isUpdated = true;
            }

            // Update Phone if it's changed
            if (!string.IsNullOrWhiteSpace(userUpdate.Phone) && userUpdate.Phone != existingUser.Phone)
            {
                existingUser.Phone = userUpdate.Phone;
                isUpdated = true;
            }

            // Update Address if it's changed
            if (!string.IsNullOrWhiteSpace(userUpdate.Address) && userUpdate.Address != existingUser.Address)
            {
                existingUser.Address = userUpdate.Address;
                isUpdated = true;
            }

            // If no updates were made, return a message indicating no changes
            if (!isUpdated)
                return Ok(new { status = "No changes have been made." });

            // Save the updated profile
            await _userRepository.UpdateUserAsync(existingUser);

            return Ok(new { status = "Profile updated successfully." });
        }



        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordUser changePassword)
        {
            // Validate input
            if (string.IsNullOrEmpty(changePassword.CurrentPassword) || string.IsNullOrEmpty(changePassword.NewPassword))
                return BadRequest(new { status = "Current password and new password are required" });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            var user = await _userRepository.GetUserById(userId);
            if (user == null) return NotFound(new { status = "User not found" });

            // Check if the current password is correct
            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, user.Password))
                return BadRequest(new { status = "Current password is incorrect" });

            // Hash the new password
            user.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);

            // Save the new password
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { status = "Password changed successfully" });
        }
    }
}


/* [HttpDelete("DeleteUser/{id:int}")]
 public async Task<IActionResult> Delete(int id)
 {
     try
     {
         var user = await _userRepository.GetUserById(id);
         if (user == null)
         {
             return NotFound(new { status = "User not found" });
         }

         // Set the IsDeleted flag to true
         user.IsDeleted = true;

         await _userRepository.UpdateUserAsync(user);
         return NoContent(); // 204 No Content response indicates success without returning data
     }
     catch (Exception ex)
     {
         var errorMessage = ex.InnerException?.Message ?? ex.Message;
         return StatusCode(500, errorMessage);
     */


/*

[HttpPost("add-image")]
public async Task<IActionResult> AddImage(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        return BadRequest("Invalid file.");
    }

    var fileName = Path.GetFileName(file.FileName);
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

    // Create the directory if it doesn't exist
    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Ok(new { filePath });
}*/

