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
    public class ClientController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientController(IClientRepository clientRepository, IMapper mapper, IUserRepository userRepository, IAdminRepository adminRepository)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        [HttpGet("GetAllClients")]
        public async Task<ActionResult<ClientMapperView[]>> Get()
        {
            var clients = await _clientRepository.GetAllClients();
            return Ok(_mapper.Map<ClientMapperView[]>(clients));
        }

        [HttpPost("AddNewClient")]
        public async Task<IActionResult> CreateClient(ClientMapperCreate clientCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check name uniqueness across all tables
                var nameExistsInAdmin = await _adminRepository.GetByAdmin(clientCreate.Name);
                var nameExistsInUser = await _userRepository.GetByUser(clientCreate.Name);
                var nameExistsInClient = await _clientRepository.GetByClient(clientCreate.Name);
                if (nameExistsInAdmin != null || nameExistsInUser != null || nameExistsInClient != null)
                    return Conflict(new { status = "Name already exists in the system" });

                // Check email uniqueness across all tables
                var emailExistsInAdmin = await _adminRepository.GetByAdminEmail(clientCreate.Email);
                var emailExistsInUser = await _userRepository.GetByUserOrEmail(clientCreate.Email);
                var emailExistsInClient = await _clientRepository.GetByClientOrEmail(clientCreate.Email);
                if (emailExistsInAdmin != null || emailExistsInUser != null || emailExistsInClient != null)
                    return Conflict(new { status = "Email already exists in the system" });

                var client = _mapper.Map<Client>(clientCreate);
                client.Password = BCrypt.Net.BCrypt.HashPassword(client.Password);

                await _clientRepository.AddClient(client);
                return StatusCode(201, _mapper.Map<ClientMapperView>(client));

            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }


        [HttpGet("GetClientBy")]
        public async Task<IActionResult> GetClientById([FromQuery] int id)
        {
            var client = await _clientRepository.GetClientById(id);
            if (client == null)
                return NotFound();

            var clientDto = _mapper.Map<ClientMapperCreate>(client);
            return Ok(clientDto);
        }

        [HttpPut("UpdateClient")]
        public async Task<IActionResult> Update([FromQuery] int id, ClientMapperUpdate clientIn)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingClient = await _clientRepository.GetClientById(id);
                if (existingClient == null)
                    return NotFound(new { status = "Client not found" });

                // Check name uniqueness
                if (!string.IsNullOrWhiteSpace(clientIn.Name) && clientIn.Name != existingClient.Name)
                {
                    var adminWithSameName = await _adminRepository.GetByAdmin(clientIn.Name);
                    var userWithSameName = await _userRepository.GetByUser(clientIn.Name);
                    var clientWithSameName = await _clientRepository.GetByClient(clientIn.Name);
                    if ((clientWithSameName != null && clientWithSameName.Id != id) || adminWithSameName != null || userWithSameName != null)
                        return Conflict(new { status = "Name already exists in the system" });
                }

                // Check email uniqueness
                if (!string.IsNullOrWhiteSpace(clientIn.Email) && clientIn.Email != existingClient.Email)
                {
                    var adminWithSameEmail = await _adminRepository.GetByAdminEmail(clientIn.Email);
                    var userWithSameEmail = await _userRepository.GetByUserOrEmail(clientIn.Email);
                    var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(clientIn.Email);
                    if ((clientWithSameEmail != null && clientWithSameEmail.Id != id) || adminWithSameEmail != null || userWithSameEmail != null)
                        return Conflict(new { status = "Email already exists in the system" });
                }

                _mapper.Map(clientIn, existingClient);
                await _clientRepository.UpdateClientAsync(existingClient);
                return Ok(_mapper.Map<ClientMapperUpdate>(existingClient));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }


        [HttpDelete("DeleteClient")]
        public async Task<IActionResult> DeleteClientAsync([FromQuery] int id)
        {
            try
            {
                var client = await _clientRepository.GetClientById(id);
                if (client == null)
                    return NotFound(new { status = "Client not found" });

                await _clientRepository.DeleteClientAsync(client.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, errorMessage);
            }
        }

        // Fetch profile of the currently logged-in admin
        [HttpGet("GetProfile")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token: user ID missing." });

            var clientId = int.Parse(userIdClaim.Value);
            var client = await _clientRepository.GetClientById(clientId);

            if (client == null)
                return NotFound(new { message = "User profile not found." });

            return Ok(new
            {
                client.Name,
                client.Email,
                client.Phone,
                client.Address,
                // Add any other fields you want to expose
            });
        }

        // Update profile for the currently logged-in admin
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ClientProfileUpdateMapper clientUpdate)
        {
            // Get the current admin's ID from the token
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            // Get the existing admin profile
            var existingClient = await _clientRepository.GetClientById(clientId);
            if (existingClient == null)
                return NotFound(new { status = "User profile not found" });

            bool isUpdated = false;

            // Check if the Name is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(clientUpdate.Name) && clientUpdate.Name != existingClient.Name)
            {
                var clientWithSameName = await _clientRepository.GetByClient(clientUpdate.Name);
                var userWithSameName = await _userRepository.GetByUser(clientUpdate.Name);
                var adminWithSame = await _adminRepository.GetByAdmin(clientUpdate.Name);

                if ((clientWithSameName != null && clientWithSameName.Id != clientId) || userWithSameName != null || adminWithSame != null)
                {
                    return Conflict(new { status = "Name already exists in the system." });
                }

                existingClient.Name = clientUpdate.Name;
                isUpdated = true;
            }

            // Check if the Email is being updated and if it already exists
            if (!string.IsNullOrWhiteSpace(clientUpdate.Email) && clientUpdate.Email != existingClient.Email)
            {
                var adminWithSameEmail = await _adminRepository.GetByAdminEmail(clientUpdate.Email);
                var userWithSameEmail = await _userRepository.GetByUserOrEmail(clientUpdate.Email);
                var clientWithSameEmail = await _clientRepository.GetByClientOrEmail(clientUpdate.Email);

                if ((clientWithSameEmail != null && clientWithSameEmail.Id != clientId) || userWithSameEmail != null || adminWithSameEmail != null)
                {
                    return Conflict(new { status = "Email already exists in the system." });
                }

                existingClient.Email = clientUpdate.Email;
                isUpdated = true;
            }

            // Update Phone if it's changed
            if (!string.IsNullOrWhiteSpace(clientUpdate.Phone) && clientUpdate.Phone != existingClient.Phone)
            {
                existingClient.Phone = clientUpdate.Phone;
                isUpdated = true;
            }

            // Update Address if it's changed
            if (!string.IsNullOrWhiteSpace(clientUpdate.Address) && clientUpdate.Address != existingClient.Address)
            {
                existingClient.Address = clientUpdate.Address;
                isUpdated = true;
            }

            // If no updates were made, return a message indicating no changes
            if (!isUpdated)
                return Ok(new { status = "No changes have been made." });

            // Save the updated profile
            await _clientRepository.UpdateClientAsync(existingClient);

            return Ok(new { status = "Profile updated successfully." });
        }



        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordClient changePassword)
        {
            // Validate input
            if (string.IsNullOrEmpty(changePassword.CurrentPassword) || string.IsNullOrEmpty(changePassword.NewPassword))
                return BadRequest(new { status = "Current password and new password are required" });

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token or missing user identifier" });

            var admin = await _clientRepository.GetClientById(adminId);
            if (admin == null) return NotFound(new { status = "Admin not found" });

            // Check if the current password is correct
            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, admin.Password))
                return BadRequest(new { status = "Current password is incorrect" });

            // Hash the new password
            admin.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);

            // Save the new password
            await _clientRepository.UpdateClientAsync(admin);

            return Ok(new { status = "Password changed successfully" });
        }
    }
}
