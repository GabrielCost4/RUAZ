using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RUAZ.API.Authorization;
using RUAZ.API.ExceptionHandling;
using RUAZ.Application;
using RUAZ.Application.Interfaces;
using RUAZ.Application.Services;
using RUAZ.Domain.Enums;
using RUAZ.Infrastructure.Data;
using RUAZ.Infrastructure.Repositories;
using RUAZ.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? string.Empty)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthPolicies.User, policy => policy.RequireRole(
        nameof(TypeActivity.User), nameof(TypeActivity.Store), nameof(TypeActivity.Admin)))
    .AddPolicy(AuthPolicies.Store, policy => policy.RequireRole(
        nameof(TypeActivity.Store), nameof(TypeActivity.Admin)))
    .AddPolicy(AuthPolicies.Admin, policy => policy.RequireRole(
        nameof(TypeActivity.Admin)));

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Dependency Injections //
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJWTService, JWTService>();

// Pipeline - (Ações da aplicação ao receber requisições)
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
