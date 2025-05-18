using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UKG.HCM.AuthenticationApi.DTOs.ChangePassword;
using UKG.HCM.AuthenticationApi.DTOs.CreateUser;
using UKG.HCM.AuthenticationApi.DTOs.DeleteUser;
using UKG.HCM.AuthenticationApi.DTOs.LoginUser;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.AuthenticationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
{
    [HttpGet("login")]
    [AllowAnonymous]
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
    public async Task<IActionResult> Register([FromBody] IncomingCreateOrUpdateUserDto dto)
    {
        var result = await userService.CreateUserAsync(dto);
        if (!result.Success)
            return Conflict(result.ErrorMessage);

        return Ok("User registered successfully");
    }
    
    [HttpPut("update")]
    [Authorize(Policy = PolicyNames.RequireManagerOrAbove)]
    public async Task<IActionResult> Update([FromBody] IncomingCreateOrUpdateUserDto dto)
    {
        var result = await userService.UpdateUserAsync(dto);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok("User updated successfully");
    }
    
    [HttpPatch("change-password")]
    [Authorize(Policy = PolicyNames.RequireAuthenticatedUser)]
    public async Task<IActionResult> ChangePassword([FromBody] IncomingChangePasswordDto dto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return Unauthorized("User not properly authenticated");
        
        var result = await userService.ChangePasswordAsync(dto.Email, dto.CurrentPassword, dto.NewPassword);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);
            
        return Ok("Password changed successfully");
    }

    [HttpDelete("Delete")]
    [Authorize(Policy = PolicyNames.RequireHRAdmin)]
    public async Task<IActionResult> Delete([FromBody] IncomingDeleteUserDTO dto)
    {
        var result = await userService.DeleteUserAsync(dto.Email);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);
            
        return Ok("User deleted successfully");
    }
}