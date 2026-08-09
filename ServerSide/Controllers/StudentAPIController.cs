using Microsoft.AspNetCore.Mvc;
using TestApp.Model;
using TestApp.StudentDataSimulation;

[ApiController]
[Route("api/Students")]
public class ProductsController : ControllerBase
{
    [HttpGet("All", Name ="GetAllStudents")]
    public ActionResult<IEnumerable<Student>> GetAllStudents()
    {
        return Ok(StudentDataSimulation.StudentsList);
    }

    [HttpGet("Passed", Name ="getPassedStudents")]
    public ActionResult<IEnumerable<Student>> GetPassedStudents()
    {
        return Ok(StudentDataSimulation.StudentsList.Where(student => student.Grade >= 50).ToList());
    }

    [HttpGet("AverageGrade", Name ="GetAverageGrade")]
    public ActionResult<IEnumerable<double>> GetAverageGrade()
    {
        if (StudentDataSimulation.StudentsList.Count == 0)
        {
            return NotFound("No Students found.");
        }
        var averageGrade = StudentDataSimulation.StudentsList.Average(Student => Student.Grade);
        return Ok(averageGrade);
    }
}