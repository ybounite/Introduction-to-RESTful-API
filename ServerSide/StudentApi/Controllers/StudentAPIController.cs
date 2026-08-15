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
}