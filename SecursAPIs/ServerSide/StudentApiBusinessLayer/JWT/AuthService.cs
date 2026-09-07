using StudentApiBusinessLayer.Interfaces;
using StudentApiBusinessLayer.Services;
using StudentDataAccessLayer;
using StudentDataAccessLayer.Interfaces;
using StudentApiBusinessLayer.DTOs;

namespace StudentApiBusinessLayer.JWT;

// AuthService = "Who are you, and are you allowed to log in?"
// JwtService  = "Okay, I'll create your JWT token."

public class AuthService : IAuthService
{
  private readonly JwtService _jwtService;
  private readonly IStudentRepository _studentRepository;
  private readonly PasswordService _passwordService;
  public AuthService(
    JwtService jwtService,
    IStudentRepository studentRepository,
    PasswordService passwordService)
  {
    _jwtService = jwtService;
    _studentRepository = studentRepository;
    _passwordService = passwordService;
  }

  public async Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDto)
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

    // 3. Generate JWT
    var token =
        _jwtService.GenerateToken(student);

    return new LoginResponseDTO
    {
      Token = token
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
    StudentDTO student = new StudentDTO(
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

}