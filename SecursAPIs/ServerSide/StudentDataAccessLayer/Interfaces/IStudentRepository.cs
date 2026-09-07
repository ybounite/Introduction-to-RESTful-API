using System.Collections.Generic;

namespace StudentDataAccessLayer.Interfaces;

public interface IStudentRepository
{
  Task<List<StudentDTO>> GetAllAsync();
  Task<List<StudentDTO>> GetPassedAsync();
  Task<double?> GetAverageGradeAsync();
  Task<StudentDTO?> GetStudentByIdAsync(int studentId);
  Task<int> AddStudentAsync(StudentDTO student);
  Task<bool> DeleteStudentAsync(int studentId);
  Task<bool> UpdateStudentAsync(StudentDTO student);
  Task<bool> UpdateStudentImageAsync(
    int studentId,
    byte[] imageData,
    string imageContentType);
  Task<StudentImageDTO?> GetStudentImageByIdAsync(int studentId);
  Task<bool> EmailExistsAsync(string email);
  Task<StudentAuth?> GetStudentAuthByEmailAsync(string email);
  Task<StudentAuth?> RegisterStudentAsync(
    StudentDTO student,
    string passwordHash,
    string role
  );
}