using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InstaportApi.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using InstaportApi;

var builder = WebApplication.CreateBuilder(args);

FirebaseInitializer.InitializeFirebase();
// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep casing as in the model (no camelCase conversion)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "InstaportApi",
        Version = "v1"
    });
    c.CustomSchemaIds(type => type.FullName); // Prevent casing confusion
});
builder.Services.AddScoped<FcmService>();

builder.Services.AddScoped<IPasswordHasher<admin>, PasswordHasher<admin>>();
builder.Services.AddScoped<IPasswordHasher<users>, PasswordHasher<users>>();
builder.Services.AddScoped<IPasswordHasher<riders>, PasswordHasher<riders>>();

builder.Services.AddDbContext<InstaportApi.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

      builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAll", builder =>
{
builder.AllowAnyOrigin()
.AllowAnyHeader()
.AllowAnyMethod();
});
});  
    
var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();



// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
