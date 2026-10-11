
using BackendService.Data;
using BackendService.Models;
using BackendService.Repos;
using BackendService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mysqlHost = builder.Configuration["MYSQL_HOST"];
var mysqlPort = builder.Configuration["MYSQL_PORT"];
var mysqlUser = builder.Configuration["MYSQL_ROOT_USER"];
var mysqlPassword = builder.Configuration["MYSQL_ROOT_PASSWORD"];
var mysqlDatabase = builder.Configuration["MYSQL_DATABASE"];


var sqlConnection = $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User={mysqlUser};Password={mysqlPassword};";


builder.Services.AddDbContext<AppDbContext>(op =>
    op.UseMySql(sqlConnection, ServerVersion.AutoDetect(sqlConnection)));

// Repos DI
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IDonorRepo, DonorRepo>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();