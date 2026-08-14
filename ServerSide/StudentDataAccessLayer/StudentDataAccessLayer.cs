using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentDataAccessLayer;

public class StudentDtO
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;

  public int Age { get; set; }
  public decimal Grade { get; set; }

  public StudentDtO(int id, string name, int age, decimal grade)
  {
    this.Id = id;
    this.Name = name;
    this.Age = age;
    this.Grade = grade;
  }

  public class StudentData
  {
    static string _connectionString = "Server=localhost,1433;Database=StudentDB;User Id=sa;Password=StudentDb@2026!;TrustServerCertificate=True;";

  //public StudentData(IConfiguration configuration)
  // {
  //   _connectionString = 
  //     configuration.GetConnectionString("DefaultConnection")
  //     ?? throw new InvalidOperationException(
  //       "DefaultConnection was not found."
  //     );
  // }
    public static async Task<List<StudentDtO>> GetAllStudents()
    {
      var students = new List<StudentDtO>();
      //  Create SQL connection
      // This creates a connection object that knows how to connect to your SQL Server database.

      /*await using means:
      "When I'm finished with this connection, clean it up automatically."*/
      await using var connection = new SqlConnection(_connectionString);

      //  Create SQL command
      await using var command = new SqlCommand(
        // is the name of your stored procedure.
        "GetStudents",
        // I want to execute something called GetStudents using this SQL connection.
        connection
      );
    //  Tell SQL Server this is a stored procedure
    // GetStudents is the name of a stored procedure, not normal SQL text.
    command.CommandType = CommandType.StoredProcedure;

    // Open the database connection
    /*
    Before:
--------------------
    ASP.NET         |
      ↓   
    SQL Connection object
      ↓
    NOT CONNECTED   |
---------------------
    After:
---------------------
    ASP.NET         |
      ↓
    SQL Connection  |
      ↓
    SQL Server      | 
      ↓
    CONNECTED       |
---------------------
    */
    await connection.OpenAsync();
    // Execute the stored procedure
    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      students.Add( new StudentDtO(
        Convert.ToInt32(reader["Id"]),
        reader["Name"] .ToString() ?? string.Empty,
        Convert.ToInt32(reader["Age"]),
        Convert.ToDecimal(reader["Grade"])
    ));
    }
    return students;
    }
  }
}
