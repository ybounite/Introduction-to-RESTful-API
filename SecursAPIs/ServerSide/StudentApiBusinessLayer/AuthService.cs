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
    // Step 1: Find the student by email from the in-memory data store.
    // Email acts as the unique login identifier.
    var student =
        await StudentApiBusinessLayer.Student.GetStudentAuthByEmail(email);

    // If no student is found with the given email,
    // return 401 Unauthorized without revealing which field was wrong
    if (student == null)
    {
        return null;
    }

    // Step 2: Verify the provided password against the stored hash.
    // BCrypt handles hashing and salt internally
    var isValidPassword =
    Student.VerifyPassword(
        password,
        student.PasswordHash
      );
    // If the password does not match the stored hash,
    if (!isValidPassword)
    {
        return null;  //return 401 Unauthorized.
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
    var studentAuth = await StudentDataAccessLayer.StudentData.RegisterStudent(
      student,
      PasswordHash,
      "Student"
    );
    return studentAuth;
  }

}