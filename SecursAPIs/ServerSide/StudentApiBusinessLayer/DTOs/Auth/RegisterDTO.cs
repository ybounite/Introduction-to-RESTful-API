using System.ComponentModel.DataAnnotations;

namespace StudentApiBusinessLayer.DTOs.Auth;

public class RegisterDTO
{
  [Required]
  public string Name {get; set;} = string.Empty;
  [Range(1, 100)]
  public int Age {get; set;}
  // [Optional]
  [Range(1, 100)]
  public decimal? Grade {get; set;}
  [Required]
  [EmailAddress]
  public string Email {get; set;} = string.Empty;
  [Required]
  [MinLength(8)]
  public string Password {get; set;} = string.Empty;
}