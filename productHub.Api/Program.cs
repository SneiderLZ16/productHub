using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

// Infrastructure
using productHub.Infrastructure.Persistence;
using productHub.Infrastructure.Repositories;
using productHub.Infrastructure.Security;

// Application / Domain
using productHub.Application.Interfaces;
using productHub.Application.Services;
using productHub.Application.Security;
using productHub.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
// Kestrel: usar el puerto que Render asigna (PORT)
// ------------------------------------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    options.ListenAnyIP(int.Parse(port));
});

// ------------------------------------------------------
// Controllers + Swagger (siempre habilitado)
// ------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProductHub API",
        Version = "v1",
        Description = "API ProductHub con MySQL (Aiven) y JWT"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insertar token en formato: Bearer {token}",
        Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// ------------------------------------------------------
// Database (MySQL Aiven con SSL) - prioriza ENV VAR
// ------------------------------------------------------
// Render/Aiven: configura esto como env var:
//   ConnectionStrings__DefaultConnection=server=...;port=...;database=...;user=...;password=...;sslmode=Required;
var connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not configured. Set ConnectionStrings__DefaultConnection or DB_CONNECTION_STRING.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// ------------------------------------------------------
// Dependency Injection
// ------------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Security (Infrastructure/Security)
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// ------------------------------------------------------
// JWT Authentication (prioriza ENV VARS)
// ------------------------------------------------------
var jwtKey =
    Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT secret not configured. Set JWT_SECRET env var.");

var jwtIssuer =
    Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer           = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidIssuer              = jwtIssuer,
            ValidateAudience         = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidAudience            = jwtAudience,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ------------------------------------------------------
// Pipeline
// ------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();


if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
