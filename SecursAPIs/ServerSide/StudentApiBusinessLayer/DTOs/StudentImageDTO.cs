namespace StudentApiBusinessLayer.DTOs;

public class StudentImageDTO
{
	public byte[] ImageData { get; set; } = Array.Empty<byte>();
	public string ContentType { get; set; } = string.Empty;
}