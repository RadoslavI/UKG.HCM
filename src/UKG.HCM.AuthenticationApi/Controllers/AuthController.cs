using Microsoft.AspNetCore.Mvc;
using UKG.HCM.AuthenticationApi.DTOs.LoginUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;

namespace UKG.HCM.AuthenticationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] IncomingLoginUserDto login)
    {
        var user = userService.ValidateUser(login.Username, login.Password);
        if (user is null)
            return Unauthorized("Invalid username or password");

        var token = tokenService.GenerateToken(user);
        return Ok(new { token });
    }
}