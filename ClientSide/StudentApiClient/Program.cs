
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
// using Newtonsoft.Json;

namespace StudentApiClient
{
  class Program
  {
  static readonly HttpClient httpClient = new HttpClient();

  static async Task Main(string[] args)
  {
    httpClient.BaseAddress = new Uri("http://localhost:5004/api/Students/"); // Set this to the correct URI for your API

    await GetAllStudents();

    await GetPassedStudents();

    await GetAverageGrade();

    await GetStudentByID(4);

    await AddStudent(new Student{
      Id = 1,
      Name = "yassir",
      Age = -24,
      Grade= 1
      });
  }

  static private void PrintAllStudents(List<Student> students)
  {
    foreach (var student in students)
    { 
      Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Age: {student.Age}, Age: {student.Grade}");
    }
  }
  static async Task GetAllStudents()
  {
    try
    {
      Console.WriteLine("\n_____________________________");
      Console.WriteLine("\nFetching all students...\n");
      //GetAllStudents Method: The endpoint string in
      //the GetFromJsonAsync method has been changed to "All",
      //which matches the [HttpGet("All", Name = "GetAllStudents")] attribute on the server.
      var students = await httpClient.GetFromJsonAsync<List<Student>>("All");
      
      if (students != null)
      {
        PrintAllStudents(students);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }

  static async Task GetPassedStudents()
  {
    try
    {
      Console.WriteLine("\n_____________________________");
      Console.WriteLine("\nFetching Passed students...\n");
      //GetAllStudents Method: The endpoint string in
      //the GetFromJsonAsync method has been changed to "Passed",
      //which matches the [HttpGet("Passed", Name = "GetPassedStudents")] attribute on the server.
      var students = await httpClient.GetFromJsonAsync<List<Student>>("Passed");

      if (students != null)
      {
        PrintAllStudents(students);
      }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }

  static async Task GetAverageGrade()
  {
    try
    {
      Console.WriteLine("\n_____________________________");
      Console.WriteLine("\nFetching average grade...\n");
      var response = await httpClient.GetAsync("AverageGrade");
      if (response.IsSuccessStatusCode)
      {
        var averageGrade = await response.Content.ReadFromJsonAsync<double>();
        Console.WriteLine($"Average Grade: {averageGrade}");
      }else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
       Console.WriteLine("No student found."); 
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }
  
  static async Task GetStudentByID(int id)
  {
    try
    {
      Console.WriteLine("\n_____________________________");
      Console.WriteLine("\nFetching Get student By ID...\n");
      var response = await httpClient.GetAsync(id.ToString());
      if (response.IsSuccessStatusCode)
      {
        var student = await response.Content.ReadFromJsonAsync<Student>();
        if (student != null)
          Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Age: {student.Age}, Age: {student.Grade}");
      }else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        Console.WriteLine($"Not Found: Student with ID: {id}"); 
      }else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
      {
        Console.WriteLine($"Bad Requset: Not accepted ID : {id}");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }

  static async Task AddStudent(Student NewStudent)
  {
    try
    {
      Console.WriteLine("\n_____________________________");
      Console.WriteLine("\nFetching Add New Student...\n");
      var response = await httpClient.PostAsJsonAsync("AddStudent", NewStudent);
      if (response.IsSuccessStatusCode)
      {
         var student = await response.Content.ReadFromJsonAsync<Student>();
        if (student != null)
          Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Age: {student.Age}, Age: {student.Grade}");
      }
      else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
      {
        Console.WriteLine($"Bad Requset: Not accepted New Student : ");
        Console.WriteLine($"ID: {NewStudent.Id}, Name: {NewStudent.Name}, Age: {NewStudent.Age}, Age: {NewStudent.Grade}");
      }
  }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }
}

  public class Student
  {
      public int Id { get; set; }
      public string Name { get; set; } = String.Empty;
      public int Age { get; set; }
      public int  Grade { get; set; }
  }
}
