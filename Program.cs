using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;
using envmanager.src.services.interfaces.user;
using envmanager.src.services.usecases.auth;
using envmanager.src.services.usecases.user;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using envmanager.src.data.service.repositories;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.infra.db;
using envmanager.src.services.interfaces.project;
using envmanager.src.services.usecases.project;

var builder = WebApplication.CreateBuilder(args);

// --- Configurações de Segurança ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is not configured in appsettings.json");
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero 
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- Registro do Exception Handler (Novo!) ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Services & Utils ---
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<JWTService>();
builder.Services.AddSingleton<SecurityService>();

// --- Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
builder.Services.AddScoped<IValidateRefreshToken, ValidateRefreshToken>();
builder.Services.AddScoped<IAuthLoginUseCase, AuthLoginUseCase>();
builder.Services.AddScoped<IGetProjectsUseCase, GetProjectsUseCase>();
builder.Services.AddScoped<ICreateProjectUseCase, CreateProjectUseCase>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 2. Identifica o usuário
app.UseAuthorization();  // 3. Verifica permissões

app.MapControllers();

app.Run();