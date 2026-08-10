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

    [HttpGet("Passed", Name ="GetPassedStudents")]
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

    [HttpGet("{StudentID}", Name ="GetStudentByID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Student> GetStudentByID(int StudentID)
    {
        if (StudentID < 1)
        {
            return BadRequest($"Not accepted ID {StudentID}");
        }
        var Student = StudentDataSimulation.StudentsList.FirstOrDefault(student => student.Id == StudentID);

        if (Student == null)
        {
            return NotFound($"Student By Id {StudentID} Not Found.");
        }
        return Ok(Student);
    }

    [HttpPost("AddStudent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Student> AddStudent(Student newStudent)
    {
        if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) || newStudent.Age < 0 || newStudent.Grade < 0)
        {
            return BadRequest("Invalid student data.");
        }
        newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(student => student.Id) + 1 : 1;
        StudentDataSimulation.StudentsList.Add(newStudent);
        return CreatedAtRoute("GetStudentByID", new {StudentID = newStudent.Id}, newStudent);
    }
    [HttpDelete("{ID}", Name ="DeleteStudent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult DeleteStudent(int ID)
    {
        if (ID < 1)
        {
            return BadRequest($"Not Accepted ID : {ID}");
        }
        var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == ID);

        if (student == null)
        {
            return NotFound($"Student with ID {ID} Not found.");
        }
        StudentDataSimulation.StudentsList.Remove(student);
        return Ok($"Student with ID {ID} has been deleted.");
    }

    [HttpPut("Update/{ID}", Name ="UpdateStudent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Student> UpdateStudent(int ID, Student UpdateStudent)
    {
        if (ID < 1 || UpdateStudent == null || string.IsNullOrEmpty(UpdateStudent.Name)
                || UpdateStudent.Age < 0 || UpdateStudent.Grade < 0)
        {
            return BadRequest($"Invalid student data.");
        }
        var student = StudentDataSimulation.StudentsList.Find(s => s.Id == ID);
        if (student == null)
        {
            return NotFound($"Student with ID {ID} Not found.");
        }
        student.Name = UpdateStudent.Name;
        student.Grade = UpdateStudent.Grade;
        student.Age = UpdateStudent.Age;
        return Ok(student);
    }
}