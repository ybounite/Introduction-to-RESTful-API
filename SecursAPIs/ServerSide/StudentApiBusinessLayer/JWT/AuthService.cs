using StudentApiBusinessLayer.Interfaces;
using StudentApiBusinessLayer.Services;
using StudentDataAccessLayer.Interfaces;
using StudentApiBusinessLayer.DTOs.Auth;
using StudentDataAccessLayer.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudentApiBusinessLayer.JWT;

// AuthService = "Who are you, and are you allowed to log in?"
// JwtService  = "Okay, I'll create your JWT token."

public class AuthService : IAuthService
{
  private readonly JwtService _jwtService;
  private readonly IStudentRepository _studentRepository;
  private readonly PasswordService _passwordService;
  private readonly IRefreshTokenRepository _refreshTokenRepository;
  public AuthService(
    JwtService jwtService,
    IStudentRepository studentRepository,
    PasswordService passwordService,
    IRefreshTokenRepository refreshTokenRepository)
  {
    _jwtService = jwtService;
    _studentRepository = studentRepository;
    _passwordService = passwordService;
    _refreshTokenRepository = refreshTokenRepository;
  }

  public async Task<TokenResponse?> LoginAsync(LoginRequest loginDto)
  {
    // Step 1: Find the student by email through the repository.
    // Email acts as the unique login identifier.
    var student =
        await _studentRepository.GetStudentAuthByEmailAsync(loginDto.Email);

    // If no student is found with the given email,
    // return 401 Unauthorized without revealing which field was wrong
    if (student == null)
    {
        return null;
    }

    // Step 2: Verify the provided password against the stored hash.
    // BCrypt handles hashing and salt internally
    var isValidPassword =
    _passwordService.VerifyPassword(
        loginDto.Password,
        student.PasswordHash
      );
    // If the password does not match the stored hash,
    if (!isValidPassword)
    {
        return null;  //return 401 Unauthorized.
    }

    // 3. Generate access token
    var accessToken =
        _jwtService.GenerateToken(student); //accessToken
    // 4. Generate raw refresh token
    var rawRefreshToken =
        GenerateRefreshToken();
    // 5. Hash refresh token
    var refreshTokenHash =
        HashRefreshToken(rawRefreshToken);
    // 6. Create database entity
    var refreshTokenEntity = new RefreshToken
    {
      Id = 0,
      UserId = student.Id,
      TokenHash = refreshTokenHash,
      RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
      RefreshTokenRevokedAt = null
    };
    // 7. Save to database
    await _refreshTokenRepository.CreateAsync(
      refreshTokenEntity
    );
    // 8. Return both tokens
    return new TokenResponse
    {
      AccessToken = accessToken,
      RefreshToken = rawRefreshToken
    };
  }

  public async Task<RegisterResponseDTO?> RegisterAsync(RegisterDTO registerDto)
  {
    // Check if email already exists 
    bool emailExists = 
      await _studentRepository.EmailExistsAsync(registerDto.Email);
    if (emailExists)
    {
      Console.WriteLine($"email elready exists : {registerDto.Email}");
      return null;
    }

    // Hash Password
    string passwordHash = _passwordService.HashPassword(
      registerDto.Password
    );

    // Create student DTO
    StudentModel student = new StudentModel(
      0,
      registerDto.Name,
      registerDto.Age,
      registerDto.Grade,
      registerDto.Email
    );

    // Save student + Password hash
    var studentAuth = await _studentRepository.RegisterStudentAsync(
      student,
      passwordHash,
      "Student"
    );

    if (studentAuth == null)
      return null;

    return new RegisterResponseDTO{
      Id = studentAuth.Id,
      Name = studentAuth.Name,
      Email = studentAuth.Email,
      Role = studentAuth.Role
    };
  }
  
	private string GenerateRefreshToken()
	{
		//creates 64 random bytes
		var randomBytes = new byte[64];
		using var randomNumberGenerator = RandomNumberGenerator.Create();
		//fills those bytes using a cryptographically secure random number generator.
		randomNumberGenerator.GetBytes(randomBytes);
		//converts the bytes into a string that we can send to the client.
		return Convert.ToBase64String(randomBytes);
  }
  private string HashRefreshToken(string refreshToken)
  {
    using var sha256 = SHA256.Create();
    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
    return Convert.ToHexString(bytes);
  }
  private bool VerifyRefresToken()
  {
    return true;
  }
  public async Task<TokenResponse?> RefreshTokensAsync(RefreshRequest rawRefreshToken)
  {
    //* 1. Hash the incoming refresh token to find it in the database
    string tokenHash = HashRefreshToken(rawRefreshToken.RefreshToken);
    //* 2. Get the token from the database
    var storedToke = 
      await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
    //* 3. Business Rule: Does it exist?
    if (storedToke == null)
    {
      return null; //* Token is invalid
    }
    //* 4. Business Rule: Is it expired?
    if (storedToke.RefreshTokenExpiresAt < DateTime.UtcNow)
    {
      return null; //* Token is expired
    }
    //* 5. Business Rule: Is it revoked?
    if (storedToke.RefreshTokenRevokedAt != null)
    {
      return null; //* Token was revoked (e.g., user logged out)
    }
    //* 6. Revoke the old token so it cannot be used again (Token Rotation)
    //await _refreshTokenRepository.RevokeAsync(storedToke.Id, DateTime.UtcNow);

    //? 7. Generate new Access Token (Using your existing JwtService)
    //! NOTE: You will need to pass the correct parameters your JwtService expects.
    // For example, if it needs a User object, you might need to fetch the User by storedToken.UserId first.
    var student = await _studentRepository.GetStudentAuthByIdAsync(storedToke.UserId);
    if (student == null)
    {
      return null;
    }
    var newAccessToken = _jwtService.GenerateToken(new studentAuthModel
    {
      Id = student.Id,
      Name = student.Name,
      Email = student.Email,
      PasswordHash = student.PasswordHash,
      Role = student.Role
    });

    string newRefreshToken = GenerateRefreshToken();
    string newRefreshTokenHash = HashRefreshToken(newRefreshToken);

    await _refreshTokenRepository.CreateAsync(new RefreshToken
    {
      Id = 0,
      UserId = storedToke.UserId,
      TokenHash = newRefreshTokenHash,
      RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
      RefreshTokenRevokedAt = null
    });
    return new TokenResponse
    {
      AccessToken = newAccessToken,
      RefreshToken = newRefreshToken
    };
  }
}