using Microsoft.AspNetCore.Mvc;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.DTOs.LoginUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;

namespace UKG.HCM.AuthenticationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] IncomingLoginUserDto login)
    {
        var user = await userService.ValidateUserAsync(login.Email, login.Password);
        if (user is null)
            return Unauthorized("Invalid email or password");

        var token = tokenService.GenerateToken(user);
        return Ok(new OutgoingLoginUserDto(token));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] IncomingCreateUserDto dto)
    {
        var created = await userService.CreateUserAsync(dto);
        if (!created)
            return Conflict($"User with email {dto.Email} already exists.");

        return Ok();
    }
}