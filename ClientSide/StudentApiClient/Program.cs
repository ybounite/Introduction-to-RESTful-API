
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

	// How long we wait between calls to force access token expiry.
	// (Set your API access token expiry to ~10 seconds to see refresh easily.)
	private const int WaitSecondsBeforeSecondCall = 15;
	static async Task Main(string[] args)
	{
		/* httpClient.BaseAddress = new Uri("http://localhost:5000/api/Students/"); // Set this to the correct URI for your API

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
		//   });*/
		
		/*Console.WriteLine("=== Student API Console Client (JWT) ===");
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
		Console.WriteLine();*/
		Console.WriteLine("=== Student API Console Client (Access + Refresh Tokens) ===");
		Console.WriteLine();
		using var http = CreateHttpClientForLocalDev(BaseUrl);

		// 1) Login to get AccessToken + RefreshToken
		var tokenPair = await LoginAsync(http, Email,Password);
		if (tokenPair == null ||
			string.IsNullOrWhiteSpace(tokenPair.AccessToken) ||
			string.IsNullOrWhiteSpace(tokenPair.RefreshToken))
		{
			Console.WriteLine("Login failed.");
			return ;
		}
		// Store tokens in a single mutable object (no ref/out; async-friendly)
		var tokenState = new TokenState(tokenPair.AccessToken, tokenPair.RefreshToken);

		Console.WriteLine("Login succeeded.");
		Console.WriteLine("======================================");
		Console.WriteLine("Initial Tokens:");
		Console.WriteLine("======================================");

		Console.WriteLine($"Access Token:\n{tokenState.AccessToken}");
		Console.WriteLine();
		Console.WriteLine($"Refresh Token:\n{tokenState.RefreshToken}");
		Console.WriteLine("======================================");
		Console.WriteLine();

		/// 2) First secured call (expected 200)
		Console.WriteLine("First call: GET /api/Students/All (expected 200)...");
		await CallGetAllStudentsWithAutoRefreshAsync(http, tokenState);
		// 3) Wait to let access token expire (same run, no restart)
		Console.WriteLine();
		//var response = await logout(http, tokenState.RefreshToken);
		//Console.WriteLine($"response for logout : {response}");
		Console.WriteLine($"Waiting {WaitSecondsBeforeSecondCall} seconds to let the access token expire...");
		await Task.Delay(TimeSpan.FromSeconds(WaitSecondsBeforeSecondCall));
		Console.WriteLine("Wait done.");
		Console.WriteLine();

		// 4) Second secured call (expected 401 then refresh then 200)
		Console.WriteLine("Second call: GET /api/Students/All (expected 401 -> refresh -> 200)...");
		await CallGetAllStudentsWithAutoRefreshAsync(http, tokenState);

		Console.WriteLine();
		Console.WriteLine("Done.");
	}
	public static async Task<string?> logout(HttpClient http, string refreshToken)
	{
		// 1. Send an object, not just the string
    var requestBody = new { RefreshToken = refreshToken };
		// See the "Important Fixes" section below regarding the payload
		var response = await http.PostAsJsonAsync("/api/Auth/Logout", requestBody);

		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
				Console.WriteLine("Invalid credentials.");
				return null;
		}
		if (!response.IsSuccessStatusCode)
		{
				Console.WriteLine($"Logout failed: {response.StatusCode}");
				return null;
		}

		// Read the raw JSON string directly from the response
		string rawResponse = await response.Content.ReadAsStringAsync();
		
		// Print it to the console
		Console.WriteLine($"Server Response: {rawResponse}");

		return rawResponse;
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
	// Creates an HttpClient for local HTTPS development (self-signed dev cert support).
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
	// --------------------------
	// Login: returns AccessToken + RefreshToken
	// --------------------------
	private static async Task<TokenResponse?> LoginAsync(HttpClient http, string email, string password)
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
			return null;
		}
		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Login failed: {response.StatusCode}");
			return null;
		}
		return await response.Content.ReadFromJsonAsync<TokenResponse>();
	}
	// --------------------------
	// Refresh: returns NEW AccessToken + NEW RefreshToken (rotation)
	// --------------------------
	private static async Task<TokenResponse?> RefreshTokensAsync(
		HttpClient http,
		string refreshToken)
	{
		var request = new RefreshRequest
		{
			RefreshToken = refreshToken
		};
		var response = await http.PostAsJsonAsync("/api/auth/refresh", request);

		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			Console.WriteLine("Refresh failed: Unauthorized (refresh token invalid/expired/revoked).");
			return null;
		}
		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Refresh failed: {response.StatusCode}");
			return null;
		}
		return await response.Content.ReadFromJsonAsync<TokenResponse>();
	}

	// HttpRequestMessage cannot be re-sent once sent, so we clone it for retry.
	// For GET calls, this is enough. For POST/PUT with body, you'd need to recreate content too.
	private static  HttpRequestMessage CloneRequest(HttpRequestMessage original)
	{
		var clone = new HttpRequestMessage(original.Method, original.RequestUri);

		foreach(var Header in original.Headers)
			clone.Headers.TryAddWithoutValidation(Header.Key, Header.Value);
		
		// Content is not used for GET here, but we copy if present.
		// Note: some HttpContent types are single-use; for robust POST/PUT retries,
		// recreate content from the original payload instead of copying.
		if (original.Content != null)
			clone.Content = original.Content;
		return clone;
	}

	// --------------------------
	// Sends a request with AccessToken.
	// If 401 happens, refreshes tokens once and retries the request one time.
	// --------------------------
	private static async Task<HttpResponseMessage> SendWithAutoRefreshAsync(
		HttpClient http,
		HttpRequestMessage request,
		TokenState tokenState)
	{
		// First attempt with current access token
		ApplyBearerToken(request, tokenState.AccessToken); // add token with heard
		var response = await http.SendAsync(request);
		// If not 401, return immediately
		if (response.StatusCode != HttpStatusCode.Unauthorized)
		{
			return response;
		}

		// 401 => access token expired/invalid => try refresh once
		Console.WriteLine("Access token rejected (401). Refreshing tokens...");

		response.Dispose();
		var newTokens = await RefreshTokensAsync(http, tokenState.RefreshToken);
		if (newTokens == null ||
			string.IsNullOrWhiteSpace(newTokens.AccessToken) ||
			string.IsNullOrWhiteSpace(newTokens.RefreshToken))
		{
			// Refresh failed => force re-login scenario
			return new HttpResponseMessage(HttpStatusCode.Unauthorized);
		}
		// Update stored tokens (rotation)
		tokenState.AccessToken = newTokens.AccessToken;
		tokenState.RefreshToken = newTokens.RefreshToken;

		Console.WriteLine("Refresh succeeded. Retrying the original request...");
		Console.WriteLine("======================================");
		Console.WriteLine("NEW TOKENS RECEIVED AFTER REFRESH:");
		Console.WriteLine("======================================");

		Console.WriteLine($"New Access Token:\n{tokenState.AccessToken}");
		Console.WriteLine();
		Console.WriteLine($"New Refresh Token:\n{tokenState.RefreshToken}");

		Console.WriteLine("======================================");
		Console.WriteLine();
		// Retry the original request once with new access token
		using var retryRequest = CloneRequest(request);
		ApplyBearerToken(retryRequest, tokenState.AccessToken);

		return await http.SendAsync(retryRequest);
	}
	
		static async Task CallGetAllStudentsWithAutoRefreshAsync(
		HttpClient http,
		TokenState tokenState)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, "api/Students/All");
		var response = await SendWithAutoRefreshAsync(http, request, tokenState);
		if (response.StatusCode == HttpStatusCode.Forbidden)
		{
			Console.WriteLine("403 Forbidden. You are authenticated, but not allowed to do this action.");
			return;
		}
		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine($"Request failed: {response.StatusCode}");
			return;
		}
		var students = await response.Content.ReadFromJsonAsync<List<Student>>();
		if (students == null)
		{
			Console.WriteLine("No data returned.");
			return;
		}
		Console.WriteLine($"{students.Count} students returned:");
		PrintAllStudents(students);

		Console.WriteLine();
		Console.WriteLine("======================================");
		Console.WriteLine("Current Token State After Request:");
		Console.WriteLine("======================================");

		Console.WriteLine($"Access Token:\n{tokenState.AccessToken}");
		Console.WriteLine();
		Console.WriteLine($"Refresh Token:\n{tokenState.RefreshToken}");

		Console.WriteLine("======================================");
		Console.WriteLine();
	}
	// --------------------------
	// Token utilities
	// --------------------------
	private static void ApplyBearerToken(
		HttpRequestMessage request,
		string accessToken)
	{
		request.Headers.Authorization = 
			new AuthenticationHeaderValue("Bearer", accessToken);
	}
  /*static async Task<string> LoginAndGetTokenAsync(HttpClient http, string email, string password)
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
    }*/
  
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
    public string AccessToken {get; set;} = string.Empty;
    public string RefreshToken {get; set;} = string.Empty;
  }
	// ==========================
	// Token State (mutable, async-safe)
	// ==========================
	class TokenState
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public TokenState(string accessToken, string refreshToken)
		{
			AccessToken = accessToken;
			RefreshToken = refreshToken;
		}
	}

	class RefreshRequest
	{
		//public string Email { get; set; } = "";
		public string RefreshToken { get; set; } = "";
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
