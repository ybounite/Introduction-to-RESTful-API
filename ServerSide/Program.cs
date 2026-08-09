var builder = WebApplication.CreateBuilder(args);
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