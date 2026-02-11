using envmanager.src.infra.db;
using envmanager.src.infra.interfaces;
using envmanager.src.infra.repositories;
using envmanager.src.services.interfaces;
using envmanager.src.services.usecases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// --- Database ---
builder.Services.AddSingleton<AppDbContext>();
// --- Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
// --- Repositories  ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
// --- Use Cases ---
builder.Services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
