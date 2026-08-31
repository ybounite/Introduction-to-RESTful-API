
using Microsoft.AspNetCore.Mvc;
using StudentApiBusinessLayer.JWT;
using StudentApiBusinessLayer;

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
}