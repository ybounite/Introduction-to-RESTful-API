using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{StudentID}", Name ="GetStudentByID")]
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

    // [HttpPost("AddStudent")]
    // [ProducesResponseType(StatusCodes.Status201Created)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public ActionResult<StudentDTO> AddStudent(StudentDTO newStudent)
    // {
    //     if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) ||
    //         newStudent.Age <= 0 || newStudent.Grade < 0)
    //     {
    //         return BadRequest();
    //     }
    //     return OK();
    // }

    // [HttpDelete("{ID}", Name ="DeleteStudent")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public ActionResult DeleteStudent(int ID)
    // {
        
    // }

    // [HttpPut("Update/{ID}", Name ="UpdateStudent")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public ActionResult<Student> UpdateStudent(int ID, Student UpdateStudent)
    // {
        
    // }
}
