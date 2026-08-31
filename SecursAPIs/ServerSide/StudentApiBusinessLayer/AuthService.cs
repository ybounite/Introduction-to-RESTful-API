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
}