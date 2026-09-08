using System.Collections.Generic;
using StudentDataAccessLayer.Models;
namespace StudentDataAccessLayer.Interfaces;

public interface IStudentRepository
{
  Task<List<StudentModel>> GetAllAsync();
  Task<List<StudentModel>> GetPassedAsync();
  Task<double?> GetAverageGradeAsync();
  Task<StudentModel?> GetStudentByIdAsync(int studentId);
  Task<int> AddStudentAsync(StudentModel student);
  Task<bool> DeleteStudentAsync(int studentId);
  Task<bool> UpdateStudentAsync(StudentModel student);
  Task<bool> UpdateStudentImageAsync(
    int studentId,
    byte[] imageData,
    string imageContentType);
  Task<StudentImageModel?> GetStudentImageByIdAsync(int studentId);
  Task<bool> EmailExistsAsync(string email);
  Task<studentAuthModel?> GetStudentAuthByEmailAsync(string email);
  Task<studentAuthModel?> RegisterStudentAsync(
    StudentModel student,
    string passwordHash,
    string role
  );
}