using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using StudentApiBusinessLayer;
using StudentDataAccessLayer;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/students")]

public class StudentsController : ControllerBase
{
	[HttpGet("All", Name ="GetAllStudents")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
	{
		List<StudentDTO> studentsList = await StudentApiBusinessLayer.Student.GetAllStudents();
		if (studentsList == null)
		{
			return NotFound("No Students Found!");
		}
		return Ok(studentsList);
	}

	[HttpGet("Passed", Name ="GetPassedStudents")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<IEnumerable<StudentDTO>>> GetPassedStudents()
	{
		List<StudentDTO> students = await StudentApiBusinessLayer.Student.GetPassedStudents();
		if (students == null)
		{
			return BadRequest("No Students Found!");
		}
		return Ok(students);
	}

	[HttpGet("AverageGrade", Name ="GetAverageGrade")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<double>> GetAverageGrade()
	{
		var averageGrade = await StudentApiBusinessLayer.Student.GetAverageGrade();
		if (averageGrade == null)
		{
			return NotFound("No grades found.");
		}
		return Ok(averageGrade);
	}

	[HttpGet("{studentId}", Name ="GetStudentByID")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<StudentDTO>> GetStudentByID(int studentId)
	{
		if (studentId <= 0)
		{
			return BadRequest("Student ID must be greater than 0.");
		}
		// var student = await StudentApiBusinessLayer.Student.GetStudentByID(studentId);
		StudentApiBusinessLayer.Student? student = await StudentApiBusinessLayer.Student.Find(studentId);
			// Student does not exist
		if (student == null)
		{
			return NotFound($"Student with ID {studentId} was not found.");
		}
		return Ok(student);
	}

	[HttpPost("AddStudent")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<StudentDTO>> AddStudent([FromBody]StudentDTO newStudent)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) ||
			newStudent.Age <= 0 || newStudent.Grade < 0)
		{
			return BadRequest("Invalid student data.");
		}
		StudentApiBusinessLayer.Student student = new StudentApiBusinessLayer.Student(new StudentDTO(
			newStudent.Id, newStudent.Name, newStudent.Age, newStudent.Grade));
		await student.Save();

		return CreatedAtRoute("GetStudentByID", new {StudentID = student.SDTO.Id}, student.SDTO);
	}

	[HttpDelete("{studentID}", Name ="DeleteStudent")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> DeleteStudent(int studentID)
	{
		try
		{
			if (studentID < 0)
			{
				return BadRequest("Student ID must be greater than 0.");
			}
			if (await StudentApiBusinessLayer.Student.DeleteStudent(studentID))
			{
				return Ok($"Student with ID {studentID} has been deleted.");
			}else
			{
				return NotFound($"Student with ID {studentID} not found. no rows deleted!");
			}
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while deleted the student.\n Message error {ex.Message}");
		}
	}

	[HttpPut("Update/{studentId}", Name ="UpdateStudent")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StudentDTO>> UpdateStudent(int studentId, [FromBody]StudentDTO updateStudent)
	{
		try{
			if (studentId <= 0)
			{
				return BadRequest("Student ID must be greater than 0.");
			}
			if (updateStudent == null || string.IsNullOrEmpty(updateStudent.Name) ||
				updateStudent.Age <= 0 || updateStudent.Grade < 0)
			{
				return BadRequest("Invalid student data.");
			}
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
      StudentApiBusinessLayer.Student? student = await StudentApiBusinessLayer.Student.Find(studentId);
      if (student == null)
      {
        return NotFound($"Student with ID {studentId} was not found.");
      }
      student.Name = updateStudent.Name;
      student.Age = updateStudent.Age;
      student.Grade = updateStudent.Grade;
      await student.Save();

      return Ok(student.SDTO);
    }
    catch
    {
			return StatusCode(500, "An error occurred while updating the student.");
    }
  }
	
	[HttpPost("{Id}/image")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UploadImage(int Id, IFormFile Image)
	{
		try
		{
			if (Id <= 0)
			{
				return BadRequest("Invalid student ID.");
			}
			if (Image == null || Image.Length == 0)
			{
				return BadRequest("Image is required.");
			}

			//  5 MB maximum
			if (Image.Length > 5 * 1024 * 1024)
			{
				return BadRequest("Image size cannot exceed 5 BM.");
			}
			
			var allowedExtensions = new[]{
				".jpg",
				".jpeg",
				".png"
			};

			var extension = Path.GetExtension(Image.FileName).ToLowerInvariant();

			if (!allowedExtensions.Contains(extension))
			{
				return BadRequest("Only JPG, JPEG and PNG images are allowed.");
			}
			
			await using var stream = new MemoryStream();
			await Image.CopyToAsync(stream);

			byte[] imageData = stream.ToArray();

			var result = await StudentApiBusinessLayer.Student.UpdateStudentImage(
				Id,
				imageData,
				Image.ContentType
			);

			if (!result)
			{
				return NotFound($"Student with ID {Id} not found.");
			}
			return Ok(new
			{
				message = "Image uploaded successfully.",
				studentId = Id
		});
		}
		catch
		{
			return StatusCode(500, "An error occurred while updating the student.");
		}
	}
	
	[HttpGet("{Id}/Image")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StudentImageDTO>> GetStudentImage(int Id)
	{
		try{
			if (Id < 0)
			{
				return BadRequest("Invalid student ID.");
			}
			var result = await StudentApiBusinessLayer.Student.GetStudentImageByID(Id);

			if(result == null)
			{
				return NotFound($"Image for student with ID {Id} not found.");
			}
			return File(
				result.imageData,
				result.contentType
			);
		}
		catch
		{
			return StatusCode(500, "An error occurred while updating the student.");
		}
	}
}
