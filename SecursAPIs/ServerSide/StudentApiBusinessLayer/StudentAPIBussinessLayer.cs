using System.Runtime.Intrinsics.Arm;
using StudentDataAccessLayer;

namespace StudentApiBusinessLayer;

public class Student
{
  public enum enMode { AddNew = 0, Update = 1};

  public enMode Mode = enMode.AddNew;
  public StudentDTO SDTO
  {
    get { return (new StudentDTO(this.ID, this.Name, this.Age, this.Grade, this.Email));}
  }

  public int ID { get; set; }
  public string Name { get; set; } = string.Empty;
  public int Age { get; set; }
  public decimal? Grade { get; set; }

  // Authentication-related fields
  public string Email {get; set;} = string.Empty;
  public string PasswordHash {get; set;} = string.Empty;
  public string Role {get; set;} = string.Empty;

  public Student(StudentDTO SDTO, enMode cMode = enMode.AddNew)
  {
    this.ID = SDTO.Id;
    this.Name = SDTO.Name;
    this.Age = SDTO.Age;
    this.Grade = SDTO.Grade;
    this.Email = SDTO.Email;
    Mode = cMode;
  }
  private async Task<bool> _AddNewStudent()
  {
    this.ID = await StudentData.AddStudent(SDTO);
    return (this.ID != -1);
  }

  private async Task<bool> _UpdateStudent()
  {
    return await StudentData.UpdateStudent(SDTO);
  }
  public static async Task<List<StudentDTO>> GetAllStudents()
  {
    return await StudentData.GetAllStudents();
  }

  public static async Task<List<StudentDTO>> GetPassedStudents()
  {
    return await StudentData.GetPassedStudents();
  }
  public static async Task<double?>  GetAverageGrade()
  {
    return await StudentData.GetAverageGrade();
  }
  public static async Task<Student?> Find(int StudentID)
  {
    StudentDTO ?SDTO = await StudentData.GetStudentByID(StudentID);
    if (SDTO != null)
    {
      return new Student(SDTO, enMode.Update);
    }
    return null;
  }

  public async Task<bool> Save()
  {
    switch (Mode)
    {
      case enMode.AddNew:
        if (await _AddNewStudent())
        {
          Mode = enMode.Update;
          return true;
        }
        else
          return false;
      case enMode.Update:
        return await _UpdateStudent();
    }
    return false;
  }

  public static async Task<bool> DeleteStudent(int studentId)
  {
    return (await StudentData.DeleteStudent(studentId));
  }

  public static async Task<bool> UpdateStudentImage(int studentId,
        byte[] imageData,
        string imageContentType)
  {
    return await StudentData.UpdateStudentImage(
      studentId,
      imageData,
      imageContentType
    );
  }
  public static async Task<StudentImageDTO?> GetStudentImageByID(int studentId)
  {
    return await StudentData.GetStudentImageByID(studentId);
  }
  
  public static async Task<bool> EmailExists(string email)
  {
    return await StudentData.EmailExists(email);
  }
  public static async Task<StudentAuth?> Register(RegisterDTO registerDto)
  {
    // Check if email already exists 
    bool emailExists = await EmailExists(registerDto.Email);
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

  public static bool VerifyPassword(string password, string hash)
  {
    bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, hash);
    return isValidPassword;
  }
  public static async Task<StudentAuth?> GetStudentAuthByEmail(string email)
  {
    return await StudentData.GetStudentAuthByEmail(email);
  }
}
