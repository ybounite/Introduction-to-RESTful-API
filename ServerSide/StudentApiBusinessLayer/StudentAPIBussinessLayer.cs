using StudentDataAccessLayer;

namespace StudentApiBusinessLayer;

public class Student
{
  public static Task<List<StudentDTO>> GetAllStudents()
  {
    return StudentData.GetAllStudents();
  }
}
