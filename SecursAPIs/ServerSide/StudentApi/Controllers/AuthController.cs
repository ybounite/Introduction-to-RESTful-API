
using Microsoft.AspNetCore.Mvc;
using StudentApiBusinessLayer.JWT;
using StudentApiBusinessLayer.DTOs;
using StudentApiBusinessLayer.DTOs.Auth;
using StudentDataAccessLayer;
using StudentApiBusinessLayer.Interfaces;

namespace StudentApi.Controllers;


[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }
  [HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult> Login([FromBody]LoginRequest loginDto)
	{
		try{
			var token = 
				await _authService.LoginAsync(loginDto);
			if (token == null)
			{
				return Unauthorized("Invalid credentials!");
			}
			// Step 7: Return the serialized JWT token to the client.
			// The client will send this token with future requests.
			return Ok(token);
		}catch (Exception Message)
		{
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				$"An error occurred while login.\n {Message}");		
		}
	}

	[HttpPost("Register")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<RegisterResponseDTO?>> Register([FromBody]RegisterDTO registerDto)
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
			
			var result = await _authService.RegisterAsync(registerDto);
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

	[HttpPost("refresh")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<TokenResponse?>> Refresh([FromBody] RefreshRequest request)
	{
		try
		{
			// 1. Validate the incoming JSON model (checks for [Required] fields)
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			// 2. Pass the request to the Business Layer
			var tokenResponse = await _authService.RefreshTokensAsync(request);
			// 3. If the service returned null, the token was invalid, expired, or revoked
			if (tokenResponse == null)
			{
				return Unauthorized(new { message = "Invalid or expired refresh token." });
			}
			// 4. Return the new tokens with a 200 OK status
			return Ok(tokenResponse);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while register the student.\n Message error {ex.Message}");
		}
	}
}