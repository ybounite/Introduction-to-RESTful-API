using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace StudentDataAccessLayer;

public class StudentDTO
{
  public int Id { get; set; }
  [Required]
  public string Name { get; set; } = string.Empty;

  [Range(1, 100)]
  public int Age { get; set; }

  [Range(0, 100)]
  public decimal Grade { get; set; }

  public StudentDTO(int id, string name, int age, decimal grade)
  {
    this.Id = id;
    this.Name = name;
    this.Age = age;
    this.Grade = grade;
  }
}

public class StudentData
{
  private static string _connectionString = string.Empty;

  public static void Initialize(string connectionString)
  {
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new InvalidCastException(
        "DefaultConnection is empty."
      );
    }
    _connectionString = connectionString;
  }
  
  public static async Task<List<StudentDTO>> GetAllStudents()
  {
    var students = new List<StudentDTO>();
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
      students.Add( new StudentDTO(
        Convert.ToInt32(reader["Id"]),
        reader["Name"] .ToString() ?? string.Empty,
        Convert.ToInt32(reader["Age"]),
        Convert.ToDecimal(reader["Grade"])
      ));
    }
    return students;
  }
  
  public static async Task<List<StudentDTO>> GetPassedStudents()
  {
    var students = new List<StudentDTO>();

    await using var connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "GetPassedStudents",
      connection
    );

    command.CommandType = CommandType.StoredProcedure;
    await connection.OpenAsync();
    await using var reader = await command.ExecuteReaderAsync();
    while( await reader.ReadAsync())
    {
      students.Add(new StudentDTO(
        Convert.ToInt32(reader["Id"]),
        reader["Name"].ToString() ?? string.Empty,
        Convert.ToInt32(reader["Age"]),
        Convert.ToDecimal(reader["Grade"])
      ));
    }
    return students;
  }

  public static async Task<double?> GetAverageGrade()
  {
    await using var connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "GetAverageGrade",
      connection
    );
    command.CommandType = CommandType.StoredProcedure;
    await connection.OpenAsync();

    object? result = await command.ExecuteScalarAsync();
    if (result == null || result == DBNull.Value)
    {
      return null;
    }
      return Convert.ToDouble(result);
  }

  public static async Task<StudentDTO?> GetStudentByID(int StudentID)
  {
    await using var connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "GetStudentByID",
      connection
    );

    command.CommandType = CommandType.StoredProcedure;
    command.Parameters.Add("@Id", SqlDbType.Int).Value = StudentID;
    // command.Parameters.AddWithValue("@Id", StudentID);

    await connection.OpenAsync();

    await using var reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
      return new StudentDTO(
          Convert.ToInt32(reader["Id"]),
          reader["Name"].ToString() ?? string.Empty,
          Convert.ToInt32(reader["Age"]),
          Convert.ToDecimal(reader["Grade"])
        );
    }
    return null;
  }
  public static async Task<int> AddStudent(StudentDTO student)
  {
    await using var connection = new SqlConnection(_connectionString);
    
    await using var command = new SqlCommand(
      "AddStudent",
      connection
      );
      command.CommandType = CommandType.StoredProcedure;

      command.Parameters.AddWithValue(
        "@Name", student.Name);

      command.Parameters.AddWithValue(
        "@Age", student.Age);

      command.Parameters.AddWithValue(
        "@Grade", student.Grade);

      var returnParameter = new SqlParameter
      {
        ParameterName = "@ReturnValue",
        Direction = ParameterDirection.ReturnValue,
        SqlDbType = SqlDbType.Int
      };
      command.Parameters.Add(returnParameter);
      await connection.OpenAsync();
      await command.ExecuteNonQueryAsync();
  
      return (int)returnParameter.Value;
  }

  public static async Task<bool> UpdateStudent(StudentDTO student)
  {
    await using var connection = new SqlConnection(_connectionString);
    
    await using var command = new SqlCommand(
      "UpdateStudent",
      connection
      );
      command.CommandType = CommandType.StoredProcedure;

    command.Parameters.AddWithValue(
      "@Id", student.Id);

    command.Parameters.AddWithValue(
      "@Name", student.Name);

    command.Parameters.AddWithValue(
      "@Age", student.Age);

    command.Parameters.AddWithValue(
      "@Grade", student.Grade);
    var returnParameter = new SqlParameter
      {
        ParameterName = "@ReturnValue",
        Direction = ParameterDirection.ReturnValue,
        SqlDbType = SqlDbType.Int
      };

    command.Parameters.Add(returnParameter);
    await connection.OpenAsync();
    await command.ExecuteNonQueryAsync();

    int result = (int)returnParameter.Value;

      return (result == 1);
  }

  public static async Task<bool> DeleteStudent(int studentId)
  {
    await using var connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "DeleteStudent",
      connection
    );

    command.CommandType = CommandType.StoredProcedure;
    command.Parameters.AddWithValue("@Id", studentId);
    var returnParameter = new SqlParameter
      {
        ParameterName = "@ReturnValue",
        Direction = ParameterDirection.ReturnValue,
        SqlDbType = SqlDbType.Int
      };
    command.Parameters.Add(returnParameter);
    await connection.OpenAsync();
    await command.ExecuteNonQueryAsync();

    int result = (int)returnParameter.Value;
    return (Convert.ToBoolean(result));
  }
}
