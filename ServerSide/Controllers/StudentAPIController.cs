using Microsoft.AspNetCore.Mvc;
using TestApp.Model;
using TestApp.StudentDataSimulation;

[ApiController]
[Route("api/Students")]
public class ProductsController : ControllerBase
{
    [HttpGet("All", Name ="GetAllStudents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<Student>> GetAllStudents()
    {
        if (StudentDataSimulation.StudentsList.Count == 0)
        {
            return NotFound("No Student Found!");
        }
        return Ok(StudentDataSimulation.StudentsList);
    }

    [HttpGet("Passed", Name ="getPassedStudents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<Student>> GetPassedStudents()
    {
        if (StudentDataSimulation.StudentsList.Count == 0)
        {
            return NotFound("No Students Found.");
        }
        return Ok(StudentDataSimulation.StudentsList.Where(student => student.Grade >= 50).ToList());
    }

    [HttpGet("AverageGrade", Name ="GetAverageGrade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<double>> GetAverageGrade()
    {
        if (StudentDataSimulation.StudentsList.Count == 0)
        {
            return NotFound("No Students Found.");
        }
        var averageGrade = StudentDataSimulation.StudentsList.Average(Student => Student.Grade);
        return Ok(averageGrade);
    }

    [HttpGet("{Id}", Name ="GetStudentByID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Student> GetStudentByID(int Id)
    {
        if (Id < 1)
        {
            return BadRequest($"Not accepted ID {Id}");
        }
        var Student = StudentDataSimulation.StudentsList.FirstOrDefault(student => student.Id == Id);

        if (Student == null)
        {
            return NotFound($"Student By Id {Id} Not Found.");
        }
        return Ok(Student);
    }

}