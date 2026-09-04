
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Threading.Tasks;

// using Newtonsoft.Json;

namespace StudentApiClient
{
  class Program
  {
  // ==========================
  // Configuration
  // ==========================

  // Base URL of the secured Student API
  private const string BaseUrl = "https://localhost:4470/";
    static readonly HttpClient httpClient = new HttpClient();
    // Test credentials that already exist in the API
    private const string Email = "ybounite@gmail.com";
    private const string Password = "ybounite@gmail.com";
    static async Task Main(string[] args)
    {
      // httpClient.BaseAddress = new Uri("http://localhost:5000/api/Students/"); // Set this to the correct URI for your API

      // await GetAllStudents();

      // await GetPassedStudents();

      // await GetAverageGrade();

      // await GetStudentByID(4);

      // await AddStudent(new Student{
      //   Id = 1,
      //   Name = "yassir",
      //   Age = -24,
      //   Grade= 1
      //   });

      //   await DeleteStudent(1);

      //   await UpdateStudent(4, new Student{
      //     Id = 4,
      //     Name = "yassin",
      //     Age = 24,
      //     Grade = 80
      //   });
      Console.WriteLine("=== Student API Console Client (JWT) ===");
      Console.WriteLine();

      // Create an HttpClient configured for local HTTPS development
      using var http = CreateHttpClientForLocalDev(BaseUrl);

      var token = await LoginAndGetTokenAsync(http, Email, Password);
      // if no token was returened, login failed and we stop execution.
      if (string.IsNullOrWhiteSpace(token))
      {
        Console.WriteLine("Login failed.");
        return ;
      }

      Console.WriteLine("Login secceded.");
      Console.WriteLine($"Token (first 30 chars): {token[..30]}...");
      Console.WriteLine();

      // Setp 2: call a secured endpoint without sending a token.
      // this is expected to fail with 401 Unauthorized.

      Console.WriteLine("Calling GET /api/Students WITHOUT token (expected 401)...");
      await CallGetAllStudentsAsync(http, "");
      Console.WriteLine();

      // Step 3:  Call the samw secured endpoint with a valid JWT token.
      // This is expected to succeed.
      Console.WriteLine("Calling GET /api/Students WITHOUT token (expected 200)...");
      await CallGetAllStudentsAsync(http, token);
      Console.WriteLine();
      
    }

    static private void PrintAllStudents(List<Student> students)
    {
      Console.WriteLine($"{students.Count} Students returned: ");
      foreach (var student in students)
      { 
        Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Age: {student.Age}, Age: {student.Grade}, Email: {student.email}");
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

    static async Task DeleteStudent(int ID)
    {
      try
      {
        Console.WriteLine("\n_____________________________");
        Console.WriteLine($"\nFetching Delete Student with ID {ID}...\n");
        var response = await httpClient.DeleteAsync(ID.ToString());
        if (response.IsSuccessStatusCode)
        {
          Console.WriteLine($"Student with ID {ID} has been deleted.");
        }else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
          Console.WriteLine($"Bad Requset: Not accepted ID : {ID}");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
          Console.WriteLine($"Not Found: Student with ID: {ID}"); 
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }
    }
  
    static async Task UpdateStudent(int ID, Student updateStudent)
    {
      try
      {
        Console.WriteLine("\n_____________________________");
        Console.WriteLine("\nFetching Update student By ID...\n");
        var response = await httpClient.PutAsJsonAsync("Update/"+ ID.ToString(), updateStudent);
        if (response.IsSuccessStatusCode)
        {
          var student = await response.Content.ReadFromJsonAsync<Student>();
          if (student != null)
            Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Age: {student.Age}, Age: {student.Grade}");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
          Console.WriteLine($"Bad Requset: Not accepted ID : {ID}");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
          Console.WriteLine($"Not Found: Student with ID: {ID}"); 
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }
    }
    
    // ==========================
    // Helper Methods
    // ==========================
  static HttpClient CreateHttpClientForLocalDev(string baseUrl)
    {
      var handler = new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback = 
          (message, certificate, chain, sslErrors) =>
          sslErrors == SslPolicyErrors.None ||
          sslErrors == SslPolicyErrors.RemoteCertificateChainErrors
      };

      return new HttpClient(handler)
      {
        BaseAddress = new Uri(baseUrl)
      };
    }

  static async Task<string> LoginAndGetTokenAsync(HttpClient http, string email, string password)
    {
      var request = new LoginRequest
      {
        Email = email,
        Password = password
      };
      var response = await http.PostAsJsonAsync("/api/Auth/login", request);

      if (response.StatusCode == HttpStatusCode.Unauthorized)
      {
        Console.WriteLine("Invalid credentials.");
        return "";
      }

      if (!response.IsSuccessStatusCode)
      {
        Console.WriteLine($"Login failed: {response.StatusCode}");
        return "";
      }
      var TokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

      return TokenResponse?.Token ?? "";
    }
  
  static async Task CallGetAllStudentsAsync(HttpClient http, string token)
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, "api/Students/All");

    if (!string.IsNullOrWhiteSpace(token))
    {
      request.Headers.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
    }

    var response = await http.SendAsync(request);

    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
      Console.WriteLine("401 Unauthorized");
      return;
    }

    if (!response.IsSuccessStatusCode)
    {
      Console.WriteLine($"Request failed: {response.StatusCode}");
      return ;
    }

    var student = await response.Content.ReadFromJsonAsync<List<Student>>();
    if (student == null)
    {
      Console.WriteLine("No students found.");
      return;
    }
    PrintAllStudents(student);
  }
}

  
  // ==========================
  // DTOs
  // ==========================
  class LoginRequest
  {
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
  }

  class TokenResponse
  {
    public string Token {get; set;} = string.Empty;
  }
  public class Student
  {
      public int Id { get; set; }
      public string Name { get; set; } = String.Empty;
      public int Age { get; set; }
      public decimal?  Grade { get; set; }

      public string? email {get; set;} = string.Empty;
  }
}
