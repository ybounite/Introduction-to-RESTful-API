using StudentDataAccessLayer;
using StudentDataAccessLayer.Interfaces;

namespace StudentApiBusinessLayer.Services;

public class StudentService : Interfaces.IStudentService
{
  private readonly IStudentRepository _studentRepository;
  public StudentService(IStudentRepository studentRepository)
  {
    _studentRepository = studentRepository;
  }
  public async Task<List<StudentDTO>> GetAllStudentsAsync()
  {
    return await this._studentRepository.GetAllAsync();
  }
  public async Task<List<StudentDTO>> GetPassedStudentsAsync()
  {
    return await this._studentRepository.GetPassedAsync();
  }
  public async Task<double?> GetAverageGradeAsync()
  {
    return await _studentRepository.GetAverageGradeAsync();
  }
  public async Task<StudentDTO?> GetStudentByIdAsync(int studentId)
  {
    return await _studentRepository.GetStudentByIdAsync(studentId);
  }
  public async Task<int> AddStudentAsync(StudentDTO student)
  {
    return await _studentRepository.AddStudentAsync(student);
  }
  public async Task<bool> DeleteStudentAsync(int studentId)
  {
    return await _studentRepository.DeleteStudentAsync(studentId);
  }

  public async Task<bool> UpdateStudentAsync(StudentDTO student)
  {
    return await _studentRepository.UpdateStudentAsync(student);
  }

  public Task<bool> UpdateStudentImageAsync(
    int studentId,
    byte[] imageData,
    string imageContentType)
  {
    return _studentRepository.UpdateStudentImageAsync(
      studentId,
      imageData,
      imageContentType);
  }
  public async Task<StudentImageDTO?> GetStudentImageByIdAsync(int studentId)
  {
    return await _studentRepository.GetStudentImageByIdAsync(studentId);
  }
}