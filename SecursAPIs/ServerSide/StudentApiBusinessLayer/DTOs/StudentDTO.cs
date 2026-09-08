using System.ComponentModel.DataAnnotations;

namespace StudentApiBusinessLayer.DTOs;

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
	public string Email { get; set; } = string.Empty;
}