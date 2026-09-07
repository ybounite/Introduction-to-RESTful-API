using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace StudentDataAccessLayer;

public class StudentDTO
{
  public int Id { get; set; }
  [Required]
  public string Name { get; set; } = string.Empty;

  [Range(1, 100)]
  public int Age { get; set; }

  [Range(0, 100)]
  public decimal? Grade { get; set; }

  [EmailAddress]
  public string Email {get; set;} = string.Empty;

  public StudentDTO(int id, string name, int age, decimal? grade, string Email = "")
  {
    this.Id = id;
    this.Name = name;
    this.Age = age;
    this.Grade = grade;
    this.Email = Email;
  }
}


public class StudentImageDTO
{
  public byte[] imageData { get; set; } = Array.Empty<byte>();

  public string contentType { get; set; } = string.Empty;

  // private string GetMimType(string filePath)
  // {
  //   var extension = Path.GetExtension(filePath).ToLowerInvariant();
  //   return extension switch
  //   {
  //     ".jpg" or ".jpn" => "image/jpeg",
  //     ".png" => "image/png",
  //     ".gif" => "image/gif",
  //     _ => "application/octet-stream",
  //   };
  // }
}

// Authentication-related fields
public class StudentAuth
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Email {get; set; } = string.Empty;

  public string PasswordHash { get; set; } = string.Empty;

  public string Role { get; set; } = string.Empty;
}