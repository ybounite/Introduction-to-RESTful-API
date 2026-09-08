using StudentApiBusinessLayer.DTOs;
using StudentApiBusinessLayer.Interfaces;
using StudentDataAccessLayer.Interfaces;
using StudentDataAccessLayer.Models;

namespace StudentApiBusinessLayer.Services;

public class StudentService : IStudentService
{
  private readonly IStudentRepository _studentRepository;

  public StudentService(IStudentRepository studentRepository)
  {
      _studentRepository = studentRepository;
  }

  public async Task<List<StudentDTO>> GetAllStudentsAsync()
  {
      var students = await _studentRepository.GetAllAsync();

      return students.Select(ToDTO).ToList();
  }

  public async Task<List<StudentDTO>> GetPassedStudentsAsync()
  {
      var students = await _studentRepository.GetPassedAsync();

      return students.Select(ToDTO).ToList();
  }

  public async Task<double?> GetAverageGradeAsync()
  {
      return await _studentRepository.GetAverageGradeAsync();
  }

  public async Task<StudentDTO?> GetStudentByIdAsync(int studentId)
  {
      var student =
          await _studentRepository.GetStudentByIdAsync(studentId);

      if (student == null)
      {
          return null;
      }

      return ToDTO(student);
  }

  public async Task<int> AddStudentAsync(StudentDTO student)
  {
      var model = ToModel(student);

      return await _studentRepository.AddStudentAsync(model);
  }

  public async Task<bool> DeleteStudentAsync(int studentId)
  {
      return await _studentRepository.DeleteStudentAsync(studentId);
  }

  public async Task<bool> UpdateStudentAsync(StudentDTO student)
  {
      var model = ToModel(student);

      return await _studentRepository.UpdateStudentAsync(model);
  }

  public async Task<bool> UpdateStudentImageAsync(
      int studentId,
      byte[] imageData,
      string imageContentType)
  {
      return await _studentRepository.UpdateStudentImageAsync(
          studentId,
          imageData,
          imageContentType);
  }

  public async Task<StudentImageDTO?> GetStudentImageByIdAsync(
      int studentId)
  {
      var image =
          await _studentRepository.GetStudentImageByIdAsync(studentId);

      if (image == null)
      {
          return null;
      }

      return new StudentImageDTO
      {
          ImageData = image.ImageData,
          ContentType = image.ContentType
      };
  }

  // Business DTO → Data Access Model
  private static StudentModel ToModel(StudentDTO student)
  {
    return new StudentModel
    {
        Id = student.Id,
        Name = student.Name,
        Age = student.Age,
        Grade = student.Grade,
        Email = student.Email
    };
}

  // Data Access Model → Business DTO
  private static StudentDTO ToDTO(StudentModel student)
  {
    return new StudentDTO
    {
      Id = student.Id,
      Name = student.Name,
      Age = student.Age,
      Grade = student.Grade,
      Email = student.Email
    };
  }
}
