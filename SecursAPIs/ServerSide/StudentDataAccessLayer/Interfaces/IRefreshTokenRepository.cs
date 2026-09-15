using StudentDataAccessLayer.Models;

namespace StudentDataAccessLayer.Interfaces;

public interface IRefreshTokenRepository
{
	Task 								CreateAsync(RefreshToken refreshToken);
	Task<RefreshToken?>	GetByTokenHashAsync(string TokenHash);
	Task<bool> 					RevokeAsync(int refreshTokenId, DateTime revokedAt);
}