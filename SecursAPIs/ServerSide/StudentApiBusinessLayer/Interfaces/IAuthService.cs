using StudentApiBusinessLayer.DTOs.Auth;

namespace StudentApiBusinessLayer.Interfaces;

public interface IAuthService
{
  Task<TokenResponse?> LoginAsync(LoginRequest loginDto);
  Task<RegisterResponseDTO?> RegisterAsync(RegisterDTO registerDTO);
  Task<TokenResponse?> RefreshTokensAsync(RefreshRequest rawRefreshToken);
  Task<bool> LogoutAsync(LogoutRequest logoutDto);
}