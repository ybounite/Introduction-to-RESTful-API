using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentApiBusinessLayer.JWT;
using Microsoft.OpenApi;
using StudentDataAccessLayer.Interfaces;
using StudentDataAccessLayer.Repositories;
using StudentApiBusinessLayer.Interfaces;
using StudentApiBusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using StudentApi.Authorization.Requirements;
using StudentApi.Authorization.Handlers;

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
// Register authentication services in the dependency injection container.
// JwtBearerDefaults.AuthenticationScheme tells ASP.NET Core that
// JWT Bearer authentication will be the default authentication method.
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    // TokenValidationParameters define how incoming JWTs will be validated.
    options.TokenValidationParameters = new TokenValidationParameters
    {

      // Ensures the token was issued by a trusted issuer.
      ValidateIssuer = true,
      // The expected issuer value (must match the issuer used when creating the JWT).
      ValidIssuer = jwtIssuer,

      // Ensures the token is intended for this API (audience check).
      ValidateAudience = true,
      // The expected audience value (must match the audience used when creating the JWT).
      ValidAudience = jwtAudience,
  
      // Ensures the token has not expired.
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero,

      // Ensures the token signature is valid and was signed by the API.
      ValidateIssuerSigningKey = true,
      // The secret key used to validate the JWT signature.
      // This must be the same key used when generating the token.
      IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSecret)
      ),
    };
  });

// ========================================
// Authorization Configuration
// ========================================
// Register authorization services.
// This enables attributes like [Authorize] and role-based authorization.
builder.Services.AddAuthorization(options => {
    options.AddPolicy("StudentOwnerOrAdmin", policy =>
    {
      //! The user must be authenticated.
      policy.RequireAuthenticatedUser();
      //!The StudentOwnerOrAdminRequirement must succeed.
      policy.Requirements.Add(new StudentOwnerOrAdminRequirement());
    });
});
//! When this requirement is evaluated, use this handler.
//* Register controller support (enables [Apicontroller] controllers).
builder.Services.AddSingleton<IAuthorizationHandler, StudentOwnershipHandler>();
/*
The next step is to register the service in Program.cs,
then create methods in StudentService that call your stored procedures.
*/
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
// ========================================
// Database
// ========================================
var connectionsString = builder.Configuration.GetConnectionString("DefaultConnection")
 ?? throw new InvalidOperationException("DefaultConnection was not found.");

// StudentData.Initialize(connectionsString);

// ========================================
// Services
// ========================================
// 1. Register Services BEFORE building the app
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Minimal APIs
// 2. Register Swager generator and customize its behavoir.
builder.Services.AddSwaggerGen(options =>
{
  // ===============================
  // 1) Define the JWT Bearer security scheme
  // ===============================
  // This tells Swagger that our API uses JWT Bearer authentication
  // through the HTTP Authorization header.
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    // The name of the HTTP header where the token will be sent.
    Name = "Authorization",

    // Indicates this is an HTTP authentication scheme.
    Type = SecuritySchemeType.Http,

    // Specifies the authentication scheme name.
    // Must be exactly "Bearer" for JWT Bearer tok
    Scheme = "Bearer",

    // Optional metadata to describe the token format.
    BearerFormat = "JWT",

    // Specifies that the token is sent in the request header.
    In = ParameterLocation.Header,

    // Text shown in Swagger UI to guide the user.
    Description = "Enter: Bearer {your JWT token}"
  });
  // ===============================
  // 2) Require the Bearer scheme for secured endpoints
  // ===============================
  //
  // This tells Swagger that endpoints protected by [Authorize]
  // require the Bearer token defined above.

  options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
  {
    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
  });
});// Required to generate the OpenAPI document

//? Register the RefreshTokenRepository in Program.cs
builder.Services.AddScoped<
  IRefreshTokenRepository,
  RefreshTokenRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();
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
// Swagger HTTP Request Pipeline
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
// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Apply CORS Middleware (Pipline)
app.UseCors("StudentApiCorsPolicy");

// IMPORTANT
// Authentication middleware must run BEFORE authorization middleware.
// Authentication identifies the user.
// Authorization decides what the user is allowed to do.
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes (e.g., /api/Students, /api/Auth).
app.MapControllers();

// await AdminSeeder.SeedAdmin();
// Start the application.
app.Run();