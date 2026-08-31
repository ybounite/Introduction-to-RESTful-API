using StudentDataAccessLayer;
namespace StudentApiBusinessLayer.JWT;

public class AuthService
{
  private readonly JwtService _jwtService;

  public AuthService(JwtService jwtService)
  {
    _jwtService = jwtService;
  }

  public async Task<string?> Login(
    string email,
    string password)
  {
    // 1. Find student
    var student =
        await StudentApiBusinessLayer.Student.GetStudentAuthByEmail(email);

    // 2. Student doesn't exist
    if (student == null)
    {
        return null;
    }

    // 3. Verify password
    var isValidPassword =
    Student.VerifyPassword(
        password,
        student.PasswordHash
      );

    if (!isValidPassword)
    {
        return null;
    }

      // 4. Generate JWT
    var token =
        _jwtService.GenerateToken(student);

    return token;
  }

  public static async Task<StudentAuth?> Register(RegisterDTO registerDto)
  {
    // Check if email already exists 
    bool emailExists = await StudentApiBusinessLayer.Student.EmailExists(registerDto.Email);
    if (emailExists)
    {
      Console.WriteLine($"email elready exists : {registerDto.Email}");
      return null;
    }

    // Hash Password
    string PasswordHash = BCrypt.Net.BCrypt.HashPassword(
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
    var studentAuth = await StudentData.RegisterStudent(
      student,
      PasswordHash,
      "Student"
    );
    return studentAuth;
  }

}