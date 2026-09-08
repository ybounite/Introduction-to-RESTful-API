namespace StudentDataAccessLayer.Models;

public class StudentImageModel
{
	public byte[] ImageData { get; set; } = Array.Empty<byte>();
	public string ContentType { get; set; } = string.Empty;
}