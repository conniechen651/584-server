using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SchoolModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<SchoolDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1");

    });
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/test-db", async (SchoolDbContext db, IConfiguration config) =>
{
    try
    {
        // Get connection string (masked for security)
        var connString = config.GetConnectionString("DefaultConnection");
        var maskedConnString = connString?.Substring(0, Math.Min(50, connString.Length)) + "...";
        
        var canConnect = await db.Database.CanConnectAsync();
        if (canConnect)
        {
            var districtCount = await db.Districts.CountAsync();
            return Results.Ok(new { 
                status = "Database connected",
                districtCount = districtCount 
            });
        }
        return Results.Problem("Cannot connect to database");
    }
    catch (Exception ex)
    {
        var innerMsg = ex.InnerException?.Message ?? "No inner exception";
        var innerInnerMsg = ex.InnerException?.InnerException?.Message ?? "No deeper exception";
        
        return Results.Json(new { 
            error = ex.Message,
            innerError = innerMsg,
            deeperError = innerInnerMsg,
            exceptionType = ex.GetType().Name,
            stackTrace = ex.StackTrace?.Split('\n').Take(5)
        }, statusCode: 500);
    }
});

app.Run();
