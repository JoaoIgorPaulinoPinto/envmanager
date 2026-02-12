using envmanager.src.data.interfaces;
using envmanager.src.data.repositories;
using envmanager.src.data.utils;
using envmanager.src.infra.db;
using envmanager.src.infra.interfaces;
using envmanager.src.infra.repositories;
using envmanager.src.services.interfaces.auth;
using envmanager.src.services.interfaces.user;
using envmanager.src.services.usecases.auth;
using envmanager.src.services.usecases.user;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key não configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "envmanager_api";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- Services & Utils ---
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<JWTService>();
builder.Services.AddSingleton<SecurityService>(); 

// --- Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
builder.Services.AddScoped<IAuthLoginUseCase, AuthLoginUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- ORDEM IMPORTANTE DOS MIDDLEWARES ---
app.UseAuthentication(); // 1. Verifica quem você é (lê o token)
app.UseAuthorization();  // 2. Verifica o que você pode fazer

app.MapControllers();

app.Run();