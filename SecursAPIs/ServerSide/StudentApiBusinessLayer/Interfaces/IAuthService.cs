using StudentApiBusinessLayer.DTOs;

namespace StudentApiBusinessLayer.Interfaces;

public interface IAuthService
{
  Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDto);
  Task<RegisterResponseDTO?> RegisterAsync(RegisterDTO registerDTO);
}