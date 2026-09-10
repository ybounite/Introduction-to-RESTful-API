using StudentApiBusinessLayer.DTOs;

namespace StudentApiBusinessLayer.Interfaces;

public interface IStudentService
{
  Task<List<StudentDTO>> GetAllStudentsAsync();
  Task<List<StudentDTO>> GetPassedStudentsAsync();
  Task<double?> GetAverageGradeAsync();
  Task<StudentDTO?> GetStudentByIdAsync(int studentId);
  Task<StudentDTO?> GetStudentByIdAsync(int requestedStudentId,
    int currentUserId,
    string currentUserRole);
  Task<int> AddStudentAsync(StudentDTO student);
  Task<bool> DeleteStudentAsync(int studentId);
  Task<bool> UpdateStudentAsync(StudentDTO student);
  Task<bool> UpdateStudentImageAsync(
    int studentId,
    byte[] imageData,
    string imageContentType);
  Task<StudentImageDTO?> GetStudentImageByIdAsync(int studentId);

}