
using Microsoft.AspNetCore.Mvc;
using StudentApiBusinessLayer.JWT;
using StudentApiBusinessLayer;
using StudentDataAccessLayer;

namespace StudentApi.Controllers;


[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;

  public AuthController(AuthService authService)
  {
    _authService = authService;
  }
  [HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> Login([FromBody]LoginDTO request)
	{
    var token = await _authService.Login(request.Email, request.Password);
    if (token == null)
    {
      return Unauthorized("Invalid credentials!");
    }
    // Step 7: Return the serialized JWT token to the client.
    // The client will send this token with future requests.
		return Ok(new
    {
      token
    });
	}
  	[HttpPost("Register")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StudentAuth?>> Register([FromBody]RegisterDTO registerDto)
	{
		try{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (registerDto == null || string.IsNullOrWhiteSpace(registerDto.Name)
				|| string.IsNullOrWhiteSpace(registerDto.Email) 
				|| string.IsNullOrWhiteSpace(registerDto.Password))
			{
				return BadRequest("Name, Email, and Password are required.");
			}
			if (registerDto.Age <= 0)
			{
				return BadRequest("Age must be greater than 0.");
			}
			if (registerDto.Grade < 0 || registerDto.Grade > 100)
			{
				return BadRequest("Grade must be between 0 and 100.");
			}
			
			if (registerDto.Password.Length < 8)
			{
				return BadRequest(
					"Password must contain at least 8 characters."
				);
			}
			
			var result = await StudentApiBusinessLayer.JWT.AuthService.Register(registerDto);

			if (result == null)
			{
				return Conflict("Email already exists.");
			}

			// return Ok("Student registered successfully.");
			return Ok(result);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while register the student.\n Message error {ex.Message}");
		}
	}
}