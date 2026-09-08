using System.Globalization;

namespace StudentDataAccessLayer.Models;

public class StudentModel
{
	public int Id {get; set;}
	public String Name {get; set;} = string.Empty;
	public int Age {get; set;}
	public decimal? Grade {get; set;}
	public string Email {get; set;} = string.Empty;
	public StudentModel() { }
	public StudentModel(int id, string name, int age, decimal? grade, string email = "")
	{
		Id = id;
		Name = name;
		Age = age;
		Grade = grade;
		Email = email;
	}
}