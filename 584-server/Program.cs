using _584_server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<SchoolDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => 
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }); 
});

builder.Services.AddIdentity<SchoolModelUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<SchoolDbContext>();

builder.Services.AddAuthentication(c =>
{
    c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(c =>
{
    c.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secretkey"]!))
    };
});

builder.Services.AddScoped<JwtHandler>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
    OpenApiSecurityScheme securityScheme = new()
    {
        Name = "Comp584 Authentication",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer Token",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/test-db", async (SchoolDbContext db, IConfiguration config) =>
{
    try
    {
        // Force an actual connection attempt
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();
        
        // If we get here, connection worked
        var districtCount = await db.Districts.CountAsync();
        return Results.Ok(new { 
            status = "Database connected successfully",
            districtCount = districtCount 
        });
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