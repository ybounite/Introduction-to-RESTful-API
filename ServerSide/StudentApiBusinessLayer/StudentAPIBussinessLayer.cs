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

  public Student(StudentDTO SDTO, enMode cMode = enMode.AddNew)
  {
    this.ID = SDTO.Id;
    this.Name = SDTO.Name;
    this.Age = SDTO.Age;
    this.Grade = SDTO.Grade;
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
}
