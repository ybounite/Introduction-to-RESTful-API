using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentDataAccessLayer;

namespace StudentApiBusinessLayer.JWT;

public class JwtService
{
  private readonly JwtSettings _jwtSettings;

  public JwtService(IOptions<JwtSettings> jwtSettings)
  {
    _jwtSettings = jwtSettings.Value;
  }
  public string GenerateToken(StudentAuth student)
  {
    var claims = new[]
    {
      new Claim(
        ClaimTypes.NameIdentifier,
        student.Id.ToString()
      ),

      new Claim(
        ClaimTypes.Email,
        student.Email
      ),

      new Claim(
        ClaimTypes.Role,
        student.Role
      )
    };

    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_jwtSettings.Secret)
    );

    var credentials = new SigningCredentials(
      key,
      SecurityAlgorithms.HmacSha256
    );

    var token = new JwtSecurityToken(
      issuer: _jwtSettings.Issuer,
      audience: _jwtSettings.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(30),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler()
      .WriteToken(token);
  }

}
