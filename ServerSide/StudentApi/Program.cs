using StudentDataAccessLayer;
// using StudentApi.Services;
var builder = WebApplication.CreateBuilder(args);


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

app.MapControllers();

app.Run();