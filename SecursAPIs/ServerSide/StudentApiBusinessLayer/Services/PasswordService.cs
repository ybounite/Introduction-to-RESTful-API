namespace StudentApiBusinessLayer.Services;

public class PasswordService
{
	public bool VerifyPassword(string password, string passwordHash)
	{
		return BCrypt.Net.BCrypt.Verify(password, passwordHash);
	}

	public string HashPassword(string password)
	{
		return BCrypt.Net.BCrypt.HashPassword(password);
	}
}
