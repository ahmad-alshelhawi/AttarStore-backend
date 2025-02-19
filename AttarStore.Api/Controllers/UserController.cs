using AttarStore.Entities;
using AttarStore.Entities.submodels;
using AttarStore.Models;
using AttarStore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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

        public UserController(IUserRepository userRepository, IMapper mapper, IPermissionRepository permissionRepository, IAdminRepository adminRepository)
        {
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

        [HttpGet("GetAllAdmins")]
        public async Task<ActionResult<AdminMapperView[]>> GetAdmins()
        {
            var admins = await _adminRepository.GetAllAdmins();

            if (admins == null || !admins.Any())
            {
                return NotFound("No admins found.");
            }

            return Ok(_mapper.Map<AdminMapperView[]>(admins));
        }

        [HttpPost("AddNewUser")]
        public async Task<IActionResult> CreateUser(UserMapperCreate userCreate)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the email already exists
                if (await _userRepository.EmailExists(userCreate.Email))
                {
                    return Conflict(new { status = "Email already exists" });
                }

                var user = _mapper.Map<User>(userCreate);
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _userRepository.AddUser(user);

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, _mapper.Map<UserMapperCreate>(user));
            }

            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }

        [HttpPost("AddNewAdmin")]
        public async Task<IActionResult> AddAdmin(AdminMapperCreate adminCreate)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the email already exists
                if (await _adminRepository.EmailExists(adminCreate.Email))
                {
                    return Conflict(new { status = "Email already exists" });
                }

                var admin = _mapper.Map<Admin>(adminCreate);
                admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
                await _adminRepository.AddAdmin(admin);
                return CreatedAtAction(nameof(GetUserById), new { id = admin.Id }, _mapper.Map<AdminMapperCreate>(admin));
            }

            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
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

        [HttpPut("UpdateUser/")]
        public async Task<IActionResult> Update([FromQuery] int id, UserMapperUpdate userIn)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userRepository.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound(new { status = "User not found" });
                }

                // Check if the new email is already taken by another user
                if (await _userRepository.EmailExists(userIn.Name) && existingUser.Name != userIn.Name)
                {
                    return Conflict(new { status = "Email already exists" });
                }

                // Update the existing user with the new values
                _mapper.Map(userIn, existingUser);

                await _userRepository.UpdateUserAsync(existingUser);
                return Ok(_mapper.Map<UserMapperUpdate>(existingUser));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
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



                await _userRepository.DeleteUserAsync(user.Id);
                return NoContent(); // 204 No Content response indicates success without returning data
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
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
    }

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

}
