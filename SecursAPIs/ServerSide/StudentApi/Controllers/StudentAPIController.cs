using System.IO.Pipelines;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudentApiBusinessLayer.Interfaces;
using StudentApiBusinessLayer.DTOs;
namespace StudentApi.Controllers;

[Authorize] // This means: Every endpoint inside this controller, Require a valid JWT
[ApiController]
[Route("api/students")]

public class StudentsController : ControllerBase
{
	private readonly IStudentService _studentService;

	public StudentsController(IStudentService studentService)
	{
		_studentService = studentService;
	}

[HttpGet("All", Name = "GetAllStudents")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
	{
		try
		{
			var students = await _studentService.GetAllStudentsAsync();
			if (students == null)
				return NotFound("No students found.");
			return Ok(students);
		}
		catch (Exception)
    {
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				"An error occurred while getting students.");
    }
	}

	[HttpGet("Passed", Name ="GetPassedStudents")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<StudentDTO>>> GetPassedStudents()
	{
		try
		{
			var studentsPassed = await _studentService.GetPassedStudentsAsync();
			if (studentsPassed == null || studentsPassed.Count == 0)
				return NotFound("No passed students found.");

			return Ok(studentsPassed);
		}
		catch (Exception)
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				"An error occurred while getting passed students.");
		}
	}

	[HttpGet("AverageGrade", Name ="GetAverageGrade")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<double>> GetAverageGrade()
	{
		try
		{
			var averageGrade = await _studentService.GetAverageGradeAsync();
			if (averageGrade == null)
			{
				return NotFound("No grades found.");
			}
			return Ok(averageGrade);
		}
		catch (Exception)
    {
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				"An error occurred while getting average grade students.");
		}
	}

	[HttpGet("{studentId}", Name ="GetStudentByID")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StudentDTO>> GetStudentByID(int studentId)
	{
		try
		{
			if (studentId <= 0)
			{
				return BadRequest("Student ID must be greater than 0.");
			}
			var student = await _studentService.GetStudentByIdAsync(studentId);
			if (student == null)
			{
				return NotFound($"Student with ID {studentId} was not found.");
			}
			return Ok(student);
		}
		catch (Exception Message)
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				$"An error occurred while getting the student.\n {Message}");		
		}
	}

	[HttpPost("AddStudent")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StudentDTO>> AddStudent([FromBody]StudentDTO newStudent)
	{
		try
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) ||
				newStudent.Age <= 0 || newStudent.Grade < 0)
			{
				return BadRequest("Invalid student data.");
			}

			int studentId = await _studentService.AddStudentAsync(newStudent);

			if (studentId == -1)
			{
					return StatusCode(
						StatusCodes.Status500InternalServerError,
						"The Student could not be added."
					);
			}

			newStudent.Id = studentId;

			return CreatedAtRoute(
				"GetStudentByID",
				new { studentId },
				newStudent
			);
		}
		catch (Exception Message)
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				$"An error occurred while creating the student.\n {Message}");		
		}
	}

	[HttpDelete("{studentId}", Name ="DeleteStudent")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> DeleteStudent(int studentId)
	{
		try
		{
			if (studentId <= 0)
			{
				return BadRequest("Student ID must be greater than 0.");
			}

			var isDeleteStudent = await _studentService.DeleteStudentAsync(studentId);

			if(isDeleteStudent)
			{
				return Ok(
					$"Student with ID {studentId} has been deleted.");
			}else
			{
				return NotFound(
					$"Student with ID {studentId} not found. no rows deleted!"
				);
			}
		}
		catch (Exception Message)
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				$"An error occurred while deleting the student.\n {Message}");		
		}
	}

	[HttpPut("Update/{studentId}", Name ="UpdateStudent")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StudentDTO>> UpdateStudent(int studentId, [FromBody]StudentDTO updateStudent)
	{
		try
		{
			if (studentId <= 0)
			{
				return BadRequest("Student ID must be greater than 0.");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			
			if (updateStudent == null ||
            string.IsNullOrWhiteSpace(updateStudent.Name) ||
            updateStudent.Age <= 0 ||
            updateStudent.Grade < 0)
			{
				return BadRequest("Invalid student data.");
			}
			var existingStudent = 
				await _studentService.GetStudentByIdAsync(studentId);

			if (existingStudent == null)
			{
				return NotFound(
						$"Student with ID {studentId} was not found."
				);
			}
			existingStudent.Name = updateStudent.Name;
			existingStudent.Age = updateStudent.Age;
			existingStudent.Grade = updateStudent.Grade;

			bool updated = 
				await _studentService.UpdateStudentAsync(existingStudent);
			
			if (!updated)
			{
				return NotFound(
					$"Student with ID {studentId} was not updated."
				);
			}
			return Ok(existingStudent);
		}
		catch (Exception Message)
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				$"An error occurred while updating the student.\n {Message}");		
		}
	}

	[HttpPost("{studentId}/image")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UploadImage(int studentId, IFormFile image)
	{
		try
		{
			if (studentId <= 0)
			{
				return BadRequest("Invalid student ID.");
			}
			if (image == null || image.Length == 0)
			{
				return BadRequest("Image is required.");
			}

			// 5 MB maximum
			if (image.Length > 5 * 1024 * 1024)
			{
				return BadRequest("Image size cannot exceed 5 MB.");
			}

			var allowedExtensions = new[]
			{
				".jpg",
				".jpeg",
				".png"
			};

			var extension = Path
				.GetExtension(image.FileName)
				.ToLowerInvariant();

			if (!allowedExtensions.Contains(extension))
			{
				return BadRequest(
					"Only JPG, JPEG and PNG images are allowed.");
			}
			await using var stream = new MemoryStream();
			await image.CopyToAsync(stream);

			byte[] imageData = stream.ToArray();

			bool result = await _studentService.UpdateStudentImageAsync(
				studentId,
				imageData,
				image.ContentType
			);
			if (!result)
			{
				return NotFound(
						$"Student with ID {studentId} not found.");
			}
			return Ok(new
			{
				message = "Image uploaded successfully.",
				studentId
			});
		}
		catch (Exception)
    {
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				"An error occurred while updating the student image."
			);
		}
	}

	[HttpGet("{studentId}/Image")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> GetStudentImage(int studentId)
	{
		try{
			if (studentId <= 0)
			{
				return BadRequest("Invalid student ID.");
			}
			var result = await _studentService.GetStudentImageByIdAsync(studentId);

			if(result == null)
			{
				return NotFound($"Image for student with ID {studentId} not found.");
			}
			return File(
				result.ImageData,
				result.ContentType
			);
		}
		catch
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				"An error occurred while getting the student image."
			);
		}
	}
}