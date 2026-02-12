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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// --- Database ---
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<JWTService>();
// --- Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
// --- Repositories  ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
builder.Services.AddScoped<IAuthLoginUseCase, AuthLoginUseCase>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
