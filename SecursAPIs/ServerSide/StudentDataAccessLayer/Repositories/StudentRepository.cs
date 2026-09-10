using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentDataAccessLayer.Interfaces;
using StudentDataAccessLayer.Models;

namespace StudentDataAccessLayer.Repositories;

public class StudentRepository : Interfaces.IStudentRepository
{
  private readonly string _connectionString = string.Empty;
  public StudentRepository(IConfiguration configuration)
  {
    _connectionString = 
      configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
          "DefaultConnection was not found."
        );
  }
  public async Task<List<StudentModel>> GetAllAsync()
  {
    var students = new List<StudentModel>();
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
      students.Add( new StudentModel(
        Convert.ToInt32(reader["Id"]),
        reader["Name"] .ToString() ?? string.Empty,
        Convert.ToInt32(reader["Age"]),
        reader["Grade"] == DBNull.Value ? null : Convert.ToDecimal(reader["Grade"])
      ));
    }
    return students;
  }
  public async Task<List<StudentModel>> GetPassedAsync()
  {
    var students = new List<StudentModel>();

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
      students.Add(new StudentModel(
        Convert.ToInt32(reader["Id"]),
        reader["Name"].ToString() ?? string.Empty,
        Convert.ToInt32(reader["Age"]),
        Convert.ToDecimal(reader["Grade"])
      ));
    }
    return students;
  }
  public async  Task<double?> GetAverageGradeAsync()
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
  public async Task<StudentModel?> GetStudentByIdAsync(int studentId)
  {
    await using var connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "GetStudentByID",
      connection
    );

    command.CommandType = CommandType.StoredProcedure;
    command.Parameters.Add("@Id", SqlDbType.Int).Value = studentId;
    // command.Parameters.AddWithValue("@Id", StudentID);

    await connection.OpenAsync();

    await using var reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
      return new StudentModel(
          Convert.ToInt32(reader["Id"]),
          reader["Name"].ToString() ?? string.Empty,
          Convert.ToInt32(reader["Age"]),
          reader.IsDBNull(reader.GetOrdinal("Grade"))
          ? null
          : reader.GetDecimal(reader.GetOrdinal("Grade"))
        );
    }
    return null;
  }
  public async Task<int> AddStudentAsync(StudentModel student)
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
  public async Task<bool> DeleteStudentAsync(int studentId)
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
    var result = await command.ExecuteScalarAsync();
    int rowsAffected = Convert.ToInt32(result);
    // Console.WriteLine($"Rows affected: {rowsAffected}");
    return (rowsAffected == 1);
  }
  public async Task<bool> UpdateStudentAsync(StudentModel student)
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

    await connection.OpenAsync();
    int rewsAffected = await command.ExecuteNonQueryAsync();

    return (rewsAffected == 1);
  }
  public async Task<bool> UpdateStudentImageAsync(
    int studentId,
    byte[] imageData,
    string imageContentType)
  {
    await using var connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "UpdateStudentImage",
      connection
    );

    command.CommandType = CommandType.StoredProcedure;

    command.Parameters.Add("@ID", SqlDbType.Int)
    .Value = studentId;
    command.Parameters.Add("@ImageData", SqlDbType.VarBinary, -1)
    .Value = imageData;
    command.Parameters.Add("@ImageContentType", SqlDbType.NVarChar, 100)
    .Value = imageContentType;

    await connection.OpenAsync();

    var result = await command.ExecuteScalarAsync();

    int rowsAffected = Convert.ToInt32(result);

    return (rowsAffected == 1);
  }
  public async Task<StudentImageModel?> GetStudentImageByIdAsync(int studentId)
  {
    using var connection = new SqlConnection(_connectionString);

    using var command = new SqlCommand(
      @"SELECT ImageData , ImageContentType
      FROM Students WHERE ID = @Id",
      connection
    );

    command.Parameters.AddWithValue("@Id", studentId);
    await connection.OpenAsync();

    using var reader = await command.ExecuteReaderAsync();
    
    if (!await reader.ReadAsync())
    {
      return null;
    }

    if (reader.IsDBNull(reader.GetOrdinal("ImageData")))
    {
      return null;
    }
    return new StudentImageModel
    {
      ImageData = (byte[])reader["ImageData"],
      ContentType = reader["ImageContentType"]?.ToString()
        ??"application/octet-stream"
    };
  }
  public async Task<bool> EmailExistsAsync(string email)
  {
    await using var connection = 
      new SqlConnection(_connectionString);
    
    await using var command = new SqlCommand(
      @"
      SELECT COUNT(1)
      FROM Students
      WHERE Email = @Email",
      connection);
    command.Parameters.AddWithValue("@Email", email);

    await connection.OpenAsync();

    var result = await command.ExecuteScalarAsync();

    return Convert.ToInt32(result) > 0;
  }
  public async Task<studentAuthModel?> GetStudentAuthByEmailAsync(string email)
  {
    await using var  connection = new SqlConnection(_connectionString);

    await using var command = new SqlCommand(
      "GetStudentAuthByEmail",
      connection
    );

    command.CommandType = CommandType.StoredProcedure;

    command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
    .Value = email;

    await connection.OpenAsync();
    await using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
    {
      return null;
    }
    return new studentAuthModel
    {
      Id = Convert.ToInt32(reader["Id"]),
      Name = reader["Name"].ToString() ?? "",
      Email = reader["Email"].ToString() ?? "",
      PasswordHash = reader["PasswordHash"].ToString() ?? "",
      Role = reader["Role"].ToString() ?? ""
    };
  }
  public async Task<studentAuthModel?> RegisterStudentAsync(
    StudentModel student,
    string passwordHash,
    string role)
  {
    await using var connection = 
      new SqlConnection(_connectionString);
    
    await using var command = new SqlCommand(
      "RegisterStudent",
      connection
    );
    command.CommandType = CommandType.StoredProcedure;

    command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
      .Value = student.Name;
    command.Parameters.Add("@Age", SqlDbType.Int)
      .Value = student.Age;
    command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
      .Value = student.Email;
    command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255)
      .Value = passwordHash;
    command.Parameters.Add("@Role", SqlDbType.NVarChar, 50)
      .Value = role;
    var returnParameter = new SqlParameter
    {
      ParameterName = "@ReturnValue",
      Direction = ParameterDirection.ReturnValue,
      SqlDbType = SqlDbType.Int
    };
    command.Parameters.Add(returnParameter);

    await connection.OpenAsync();
  
    await using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
    {
      return null;
    }
    return new studentAuthModel
    {
      Id = Convert.ToInt32(reader["Id"]),
      Name = reader["Name"].ToString() ?? "",
      Email = reader["Email"].ToString() ?? "",
      PasswordHash = reader["PasswordHash"].ToString() ?? "", // remove this password hashing
      Role = reader["Role"].ToString() ??""
    };
  }

}