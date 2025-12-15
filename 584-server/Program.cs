using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your Auth0 JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(c =>
    {
        c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
{
    options.Authority = "https://dev-yl7blimqv18gp6tv.us.auth0.com/";
    options.Audience = "https://comp584server-h6g8b9bqdwfye8ab.canadacentral-01.azurewebsites.net";
});

builder.Services.AddAuthorization(options =>
{
    // Define policies based on permissions or roles
    options.AddPolicy("read:data", policy => 
        policy.RequireClaim("permissions", "read:data"));
    
    options.AddPolicy("write:data", policy => 
        policy.RequireClaim("permissions", "write:data"));
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
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1");
        
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseStaticFiles();

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
