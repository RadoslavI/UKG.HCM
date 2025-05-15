using System.Security.Cryptography;
using System.Text;

namespace UKG.HCM.AuthenticationApi.Utilities;

/// <summary>
/// Utility class for hashing passwords securely using SHA256
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Hashes a password using SHA256 algorithm
    /// </summary>
    /// <param name="password">Plain text password to hash</param>
    /// <returns>Base64 encoded hash of the password</returns>
    public static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
    
    /// <summary>
    /// Generates a random password with mixed characters
    /// </summary>
    /// <param name="length">Length of the password to generate (default: 12)</param>
    /// <returns>Randomly generated password string</returns>
    public static string GenerateRandomPassword(int length = 12)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        var password = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        
        return password;
    }
}
