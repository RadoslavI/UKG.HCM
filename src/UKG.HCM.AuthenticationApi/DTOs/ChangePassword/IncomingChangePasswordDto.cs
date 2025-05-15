namespace UKG.HCM.AuthenticationApi.DTOs.ChangePassword;

public record IncomingChangePasswordDto(string Email, string CurrentPassword, string NewPassword);