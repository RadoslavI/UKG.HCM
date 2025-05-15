using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UKG.HCM.AuthenticationApi.DTOs.ChangePassword;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.DTOs.LoginUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.Shared.Constants;

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
    [Authorize(Policy = PolicyNames.RequireHRAdmin)]
    public async Task<IActionResult> Register([FromBody] IncomingCreateUserDto dto)
    {
        var created = await userService.CreateUserAsync(dto);
        if (!created)
            return Conflict($"User with email {dto.Email} already exists.");

        return Ok();
    }
    
    [HttpPost("change-password")]
    [Authorize(Policy = PolicyNames.RequireAuthenticatedUser)]
    public async Task<IActionResult> ChangePassword([FromBody] IncomingChangePasswordDto dto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return Unauthorized("User not properly authenticated");
        
        var success = await userService.ChangePasswordAsync(dto.Email, dto.CurrentPassword, dto.NewPassword);
        if (!success)
            return BadRequest("Current password is incorrect");
            
        return Ok("Password changed successfully");
    }

    [HttpDelete("Delete")]
    [Authorize(Policy = PolicyNames.RequireHRAdmin)]
    public async Task<IActionResult> Delete([FromBody] IncomingChangePasswordDto dto)
    {
        var success = await userService.DeleteUserAsync(dto.Email);
        if (!success)
            return BadRequest("User deletion failed");
            
        return Ok("User deleted successfully");
    }
}