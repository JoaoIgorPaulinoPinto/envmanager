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
using Microsoft.AspNetCore.SignalR; // Adicionado

var builder = WebApplication.CreateBuilder(args);

// --- Configurações de Segurança ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "envmanager_api";

// --- CONFIGURAÇÃO SIGNALR & AUTH ---
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

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

    // Permite que o SignalR receba o token via QueryString (padrão do protocolo)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
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
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
builder.Services.AddScoped<IValidateRefreshToken, ValidateRefreshToken>();
builder.Services.AddScoped<IAuthLoginUseCase, AuthLoginUseCase>();
builder.Services.AddScoped<IGetProjectsUseCase, GetProjectsUseCase>();
builder.Services.AddScoped<ICreateProjectUseCase, CreateProjectUseCase>();
builder.Services.AddScoped<IUpdateProjectVariablesUseCase, UpdateProjectVariablesUseCase>();
builder.Services.AddScoped<ICreateInviteUseCase, CreateInviteUseCase>();
builder.Services.AddScoped<IUpdateProjectDescriptionUseCase, UpdateProjectDescriptionUseCase>();
builder.Services.AddScoped<IUpdateProjectNameUseCase, UpdateProjectNameUseCase>();
builder.Services.AddScoped<IResponseInvitationUseCase, ResponseInvitationUseCase>();
builder.Services.AddScoped<ITurnIntoAdminUseCase, TurnIntoAdminUseCase>();
builder.Services.AddSingleton<ITokenFactory, TokenFactory>();

// --- Configuração de CORS (Obrigatório para o SignalR) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000") // URLs do seu front
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Importante para Auth
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Use o CORS antes da Auth
app.UseCors("SignalRPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- Mapeamento do Hub ---
app.MapHub<NotificationHub>("/notificationHub");

app.Map("/", () => "Server Active!");
app.Run();