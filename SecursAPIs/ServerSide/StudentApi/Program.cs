using DotNetEnv;
using StudentDataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

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
/*
The next step is to register the service in Program.cs,
then create methods in StudentService that call your stored procedures.
*/
// builder.Services.AddScoped<StudentService>();

var connectionsString = builder.Configuration.GetConnectionString("DefaultConnection")
 ?? throw new InvalidOperationException("DefaultConnection was not found.");

StudentData.Initialize(connectionsString);

// 1. Register Services BEFORE building the app
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Minimal APIs
builder.Services.AddSwaggerGen();           // Required to generate the OpenAPI document

var app = builder.Build();

// 2. Configure Middleware AFTER building the app
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Serves the generated JSON
    app.UseSwaggerUI();    // Serves the interactive UI
}
// Verify HTTPS Redirection Middleware
app.UseHttpsRedirection();
//  this middleware redirects HTTP-> HTTPS
app.MapControllers();

app.Run();