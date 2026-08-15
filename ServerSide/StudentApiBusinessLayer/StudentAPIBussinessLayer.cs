using System.Runtime.Intrinsics.Arm;
using StudentDataAccessLayer;

namespace StudentApiBusinessLayer;

public class Student
{
  public enum enMode { AddNew = 0, Update = 1};

  public enMode Mode = enMode.AddNew;
  public StudentDTO SDTO
  {
    get { return (new StudentDTO(this.ID, this.Name, this.Age, this.Grade));}
  }

  public int ID { get; set; }
  public string Name { get; set; } = string.Empty;
  public int Age { get; set; }
  public decimal Grade { get; set; }

  public Student(StudentDTO STDO, enMode cMode = enMode.AddNew)
  {
    this.ID = STDO.Id;
    this.Name = SDTO.Name;
    this.Age = STDO.Age;
    this.Grade = SDTO.Grade;
    Mode = cMode;
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
      return new Student(SDTO, enMode.AddNew);
    }
    return null;
  }

  // public static Student _save(StudentDTO newStudent)
  // {
    
  // }
}
