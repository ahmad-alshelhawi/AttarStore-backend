//// Controllers/AuthController.cs
//using AttarStore.Models;
//using AttarStore.Services;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    private readonly AppDbContext _dbcontext;
//    private readonly TokenService _tokenService;

//    public AuthController(AppDbContext context, TokenService tokenService)
//    {
//        _dbcontext = context;
//        _tokenService = tokenService;
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginModel request)
//    {
//        var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.Name == request.Name);
//        if (admin != null && BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
//            return Ok(new { token = _tokenService.CreateToken(admin.Name, "Admin") });

//        var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
//        if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
//            return Ok(new { token = _tokenService.CreateToken(user.Name, "User") });

//        var client = await _dbcontext.Clients.FirstOrDefaultAsync(c => c.Name == request.Name);
//        if (client != null && BCrypt.Net.BCrypt.Verify(request.Password, client.Password))
//            return Ok(new { token = _tokenService.CreateToken(client.Name, "Client") });

//        return Unauthorized("Invalid credentials");
//    }
//}
