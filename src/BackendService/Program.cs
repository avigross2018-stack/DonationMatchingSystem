
using System.Security.Claims;
using System.Text;
using BackendService.Data;
using BackendService.Models;
using BackendService.Repos;
using BackendService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using BackendService.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

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
builder.Services.AddScoped<IDonationRepo, DonationRepo>();
builder.Services.AddScoped<IPopulationTypeRepo, PopulationTypeRepo>();

// Services DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<DonationService>();


var jwtKey = builder.Configuration["JWT:KEY"]
    ?? throw new InvalidOperationException("JWT key is missing");

var jwtIssuer = builder.Configuration["JWT:ISSUER"]
    ?? throw new InvalidOperationException("JWT issuer is missing");

var jwtAudience = builder.Configuration["JWT:AUDIENCE"]
    ?? throw new InvalidOperationException("JWT audience is missing");


// System.Console.WriteLine($"jwtKey {jwtKey}");
// System.Console.WriteLine($"jwtIssuer {jwtIssuer}");
// System.Console.WriteLine($"jwtAudience {jwtAudience}");
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
).AddJwtBearer(op =>
{
    op.MapInboundClaims = false;

    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        ValidateAudience = true,
        ValidAudience = jwtAudience,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        ),

        RoleClaimType = ClaimTypes.Role,
        NameClaimType = "sub",

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();