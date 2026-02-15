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
using Scalar.AspNetCore;
using envmanager.src.services.usecases.invitation;

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

// --- Registro do Exception Handler ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Services & Utils ---
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<SecurityService>();

// --- Repositories ---
builder.Services.AddScoped<IInviteRepository, InviteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>(); // get users
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>(); // create users
builder.Services.AddScoped<IValidateRefreshToken, ValidateRefreshToken>(); // validade refresh token
builder.Services.AddScoped<IAuthLoginUseCase, AuthLoginUseCase>(); // athentication 
builder.Services.AddScoped<IGetProjectsUseCase, GetProjectsUseCase>(); // get projects
builder.Services.AddScoped<ICreateProjectUseCase, CreateProjectUseCase>(); // create projects
builder.Services.AddScoped<IUpdateProjectVariables, UpdateProjectVariables>(); // update project variables 
builder.Services.AddScoped<ICreateInviteUseCase, CreateInviteUseCase>(); // create invites to projects
builder.Services.AddScoped<IResponseInvitation, AcceptProjectInvite>(); // accept invites to projects
builder.Services.AddSingleton<ITokenFactory, TokenFactory>(); // token factory

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Map("/", () => "Server Active!");

app.Run();