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
    // Step 3: Create claims that represent the authenticated user's identity.
    // These claims will be embedded inside the JWT.
    var claims = new[]
    {
      new Claim(
      // Unique identifier for the student
        ClaimTypes.NameIdentifier,
        student.Id.ToString()
      ),
      // Student email address
      new Claim(
        ClaimTypes.Email,
        student.Email
      ),
      // Role (Student or Admin) used later for authorization
      new Claim(
        ClaimTypes.Role,
        student.Role
      )
    };
    // Step 4: Create the symmetric security key used to sign the JWT.
    // This key must match the key used in JWT validation middleware.
    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_jwtSettings.Secret)
    );
    // Step 5: Define the signing credentials.
    // This specifies the algorithm used to sign the token.
    var credentials = new SigningCredentials(
      key,
      SecurityAlgorithms.HmacSha256
    );
    
    // Step 6: Create the JWT token.
    // The token includes issuer, audience, claims, expiration, and signature.
    var token = new JwtSecurityToken(
      issuer: _jwtSettings.Issuer,
      audience: _jwtSettings.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(30),
      signingCredentials: credentials
    );
    // Step 7: Return the serialized JWT token to the client.
    // The client will send this token with future requests.
    return new JwtSecurityTokenHandler()
      .WriteToken(token);
  }

}
