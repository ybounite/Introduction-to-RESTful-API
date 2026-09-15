namespace StudentDataAccessLayer.Models;

public class RefreshToken
{
	public int Id {get; set;}
	public int UserId {get; set;}
	public string TokenHash {get; set;} = string.Empty;
	public DateTime RefreshTokenExpiresAt {get; set;}
	public DateTime? RefreshTokenRevokedAt {get; set;}
}