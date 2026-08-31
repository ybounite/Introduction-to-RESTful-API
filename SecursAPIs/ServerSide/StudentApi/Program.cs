using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentApi.Controllers;
using StudentApiBusinessLayer.JWT;
using StudentApiBusinessLayer;
using StudentDataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Load .env
// ========================================
// Find .env in the solution root
var envPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "..",
    ".env"
);
// Load .env
Env.Load(envPath);
// Add environment variables to ASP.NET configuration
builder.Configuration.AddEnvironmentVariables();

// ========================================
// JWT Configuration
// ========================================
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

if (string.IsNullOrWhiteSpace(jwtSecret))
{
  throw new InvalidOperationException(
  "JWT_SECRET was not found."
  );
}
if (string.IsNullOrWhiteSpace(jwtIssuer))
{
  throw new InvalidOperationException(
    "JWT_ISSUER was not found."
  );
}
if (string.IsNullOrWhiteSpace(jwtAudience))
{
  throw new InvalidOperationException(
    "JWT_AUDIENCE was not found."
  );
}

builder.Services.Configure<JwtSettings>(options =>
{
  options.Secret = jwtSecret;
  options.Issuer = jwtIssuer;
  options.Audience = jwtAudience;
});

// ========================================
// JWT Authentication
// ========================================
// validates the token
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSecret)
      ),
      ValidateIssuer = true,
      ValidIssuer = jwtSecret,
      ValidateAudience = true,
      ValidAudience = jwtAudience,

      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero
    };
  });

// ========================================
// Authorization
// ========================================

builder.Services.AddAuthorization();

/*
The next step is to register the service in Program.cs,
then create methods in StudentService that call your stored procedures.
*/
// builder.Services.AddScoped<StudentService>();
// ========================================
// Database
// ========================================
var connectionsString = builder.Configuration.GetConnectionString("DefaultConnection")
 ?? throw new InvalidOperationException("DefaultConnection was not found.");

StudentData.Initialize(connectionsString);

// ========================================
// Services
// ========================================
// 1. Register Services BEFORE building the app
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Minimal APIs
builder.Services.AddSwaggerGen();           // Required to generate the OpenAPI document

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
// ========================================
// CORS
// ========================================
builder.Services.AddCors( options =>
{
  options.AddPolicy("StudentApiCorsPolicy", policy =>
  {
    policy
    .WithOrigins(
        "https://localhost:4470",
        "http://localhost:3000"
        )
    .AllowAnyHeader()
    .AllowAnyMethod();
  });
});

// ========================================
// Build
// ========================================
var app = builder.Build();

// ========================================
// Swagger
// ========================================
// 2. Configure Middleware AFTER building the app
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();      // Serves the generated JSON
  app.UseSwaggerUI();    // Serves the interactive UI
}
// ========================================
// Middleware
// ========================================
// Verify HTTPS Redirection Middleware
app.UseHttpsRedirection();

// Apply CORS Middleware (Pipline)
app.UseCors("StudentApiCorsPolicy");

// IMPORTANT
app.UseAuthentication();
app.UseAuthorization();

//  this middleware redirects HTTP-> HTTPS
app.MapControllers();

app.Run();