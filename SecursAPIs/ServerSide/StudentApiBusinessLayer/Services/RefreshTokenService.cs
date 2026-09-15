using System.Security.Cryptography;

namespace StudentApiBusinessLayer.Services.RefreshToken;

public class RefreshTokenService
{
	public string GenerateRefreshToken()
	{
		//creates 64 random bytes
		var randomBytes = new byte[64];
		using var randomNumberGenerator = RandomNumberGenerator.Create();
		//fills those bytes using a cryptographically secure random number generator.
		randomNumberGenerator.GetBytes(randomBytes);
		//converts the bytes into a string that we can send to the client.
		return Convert.ToBase64String(randomBytes);
	}
	public string HashRefreshToken()
	{
		return "";
	}
}