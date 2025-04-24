using AttarStore.Models;
using AttarStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttarStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IClientRepository _clientRepo;



        public ProfileController(
            IUserRepository userRepo,
            IAdminRepository adminRepo,
            IClientRepository clientRepo)

        {
            _userRepo = userRepo;
            _adminRepo = adminRepo;
            _clientRepo = clientRepo;

        }

        [Authorize]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!int.TryParse(idClaim, out var userId) || string.IsNullOrEmpty(role))
                return Unauthorized();

            object profile;
            switch (role)
            {
                case "Master":
                    var admin = await _adminRepo.GetAdminById(userId);
                    profile = new
                    {
                        id = admin.Id,
                        name = admin.Name,
                        phone = admin.Phone,
                        email = admin.Email,
                        address = admin.Address
                    };
                    break;

                case "Client":
                    var client = await _clientRepo.GetClientById(userId);
                    profile = new
                    {
                        id = client.Id,
                        name = client.Name,
                        phone = client.Phone,
                        email = client.Email,
                        address = client.Address

                    };
                    break;

                default:
                    var user = await _userRepo.GetUserById(userId);
                    profile = new
                    {
                        id = user.Id,
                        name = user.Name,
                        phone = user.Phone,
                        email = user.Email,
                        address = user.Address

                    };
                    break;
            }

            return Ok(profile);
        }


        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileMapper model)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!int.TryParse(idClaim, out var userId) || string.IsNullOrEmpty(role))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            bool isUpdated = false;

            switch (role)
            {
                case "Master":
                    var admin = await _adminRepo.GetAdminById(userId);
                    if (admin == null) return NotFound(new { status = "Admin profile not found" });

                    if (!string.IsNullOrWhiteSpace(model.Name) && model.Name != admin.Name)
                    {
                        var adminWithSameName = await _adminRepo.GetByAdmin(model.Name);
                        var userWithSameName = await _userRepo.GetByUser(model.Name);
                        var clientWithSameName = await _clientRepo.GetByClient(model.Name);

                        if ((adminWithSameName != null && adminWithSameName.Id != userId)
                            || (userWithSameName != null && userWithSameName.Id != userId)
                            || (clientWithSameName != null && clientWithSameName.Id != userId))
                        {
                            return Conflict(new { status = "Name already exists in the system." });
                        }

                        admin.Name = model.Name;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != admin.Email)
                    {
                        var adminWithSameEmail = await _adminRepo.GetByAdminEmail(model.Email);
                        var userWithSameEmail = await _userRepo.GetByUserOrEmail(model.Email);
                        var clientWithSameEmail = await _clientRepo.GetByClientOrEmail(model.Email);

                        if ((adminWithSameEmail != null && adminWithSameEmail.Id != userId)
                            || (userWithSameEmail != null && userWithSameEmail.Id != userId)
                            || (clientWithSameEmail != null && clientWithSameEmail.Id != userId))
                        {
                            return Conflict(new { status = "Email already exists in the system." });
                        }

                        admin.Email = model.Email;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Phone) && model.Phone != admin.Phone)
                    {
                        admin.Phone = model.Phone;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Address) && model.Address != admin.Address)
                    {
                        admin.Address = model.Address;
                        isUpdated = true;
                    }

                    if (!isUpdated)
                        return Ok(new { status = "No changes have been made." });

                    await _adminRepo.UpdateAdminAsync(admin);
                    break;

                case "Client":
                    var client = await _clientRepo.GetClientById(userId);
                    if (client == null) return NotFound(new { status = "Client profile not found" });

                    if (!string.IsNullOrWhiteSpace(model.Name) && model.Name != client.Name)
                    {
                        var adminWithSameName = await _adminRepo.GetByAdmin(model.Name);
                        var userWithSameName = await _userRepo.GetByUser(model.Name);
                        var clientWithSameName = await _clientRepo.GetByClient(model.Name);

                        if ((adminWithSameName != null && adminWithSameName.Id != userId)
                            || (userWithSameName != null && userWithSameName.Id != userId)
                            || (clientWithSameName != null && clientWithSameName.Id != userId))
                        {
                            return Conflict(new { status = "Name already exists in the system." });
                        }

                        client.Name = model.Name;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != client.Email)
                    {
                        var adminWithSameEmail = await _adminRepo.GetByAdminEmail(model.Email);
                        var userWithSameEmail = await _userRepo.GetByUserOrEmail(model.Email);
                        var clientWithSameEmail = await _clientRepo.GetByClientOrEmail(model.Email);

                        if ((adminWithSameEmail != null && adminWithSameEmail.Id != userId)
                            || (userWithSameEmail != null && userWithSameEmail.Id != userId)
                            || (clientWithSameEmail != null && clientWithSameEmail.Id != userId))
                        {
                            return Conflict(new { status = "Email already exists in the system." });
                        }

                        client.Email = model.Email;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Phone) && model.Phone != client.Phone)
                    {
                        client.Phone = model.Phone;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Address) && model.Address != client.Address)
                    {
                        client.Address = model.Address;
                        isUpdated = true;
                    }

                    if (!isUpdated)
                        return Ok(new { status = "No changes have been made." });

                    await _clientRepo.UpdateClientAsync(client);
                    break;

                default:
                    var user = await _userRepo.GetUserById(userId);
                    if (user == null) return NotFound(new { status = "User profile not found" });

                    if (!string.IsNullOrWhiteSpace(model.Name) && model.Name != user.Name)
                    {
                        var adminWithSameName = await _adminRepo.GetByAdmin(model.Name);
                        var userWithSameName = await _userRepo.GetByUser(model.Name);
                        var clientWithSameName = await _clientRepo.GetByClient(model.Name);

                        if ((adminWithSameName != null && adminWithSameName.Id != userId)
                            || (userWithSameName != null && userWithSameName.Id != userId)
                            || (clientWithSameName != null && clientWithSameName.Id != userId))
                        {
                            return Conflict(new { status = "Name already exists in the system." });
                        }

                        user.Name = model.Name;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
                    {
                        var adminWithSameEmail = await _adminRepo.GetByAdminEmail(model.Email);
                        var userWithSameEmail = await _userRepo.GetByUserOrEmail(model.Email);
                        var clientWithSameEmail = await _clientRepo.GetByClientOrEmail(model.Email);

                        if ((adminWithSameEmail != null && adminWithSameEmail.Id != userId)
                            || (userWithSameEmail != null && userWithSameEmail.Id != userId)
                            || (clientWithSameEmail != null && clientWithSameEmail.Id != userId))
                        {
                            return Conflict(new { status = "Email already exists in the system." });
                        }

                        user.Email = model.Email;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Phone) && model.Phone != user.Phone)
                    {
                        user.Phone = model.Phone;
                        isUpdated = true;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Address) && model.Address != user.Address)
                    {
                        user.Address = model.Address;
                        isUpdated = true;
                    }

                    if (!isUpdated)
                        return Ok(new { status = "No changes have been made." });

                    await _userRepo.UpdateUserAsync(user);
                    break;
            }

            return Ok(new { status = "Profile updated successfully." });
        }





        [Authorize]
        [HttpDelete("DeleteProfile")]
        public async Task<IActionResult> DeleteProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!int.TryParse(idClaim, out var userId) || string.IsNullOrEmpty(role))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            switch (role)
            {
                case "Master":
                    var admin = await _adminRepo.GetAdminById(userId);
                    if (admin == null)
                        return NotFound(new { status = "Admin not found" });

                    await _adminRepo.DeleteAdminAsync(admin); // You can soft delete if needed
                    break;

                case "Client":
                    var client = await _clientRepo.GetClientById(userId);
                    if (client == null)
                        return NotFound(new { status = "Client not found" });

                    await _clientRepo.DeleteClientAsync(client);
                    break;

                default:
                    var user = await _userRepo.GetUserById(userId);
                    if (user == null)
                        return NotFound(new { status = "User not found" });

                    await _userRepo.DeleteUserAsync(user);
                    break;
            }

            // Clear the cookies (accessToken & refreshToken)
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { status = "Profile deleted successfully." });
        }


    }
}
