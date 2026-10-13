
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

var sqlConnection =
    $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User={mysqlUser};Password={mysqlPassword};";

builder.Services.AddDbContext<AppDbContext>(op =>
    op.UseMySql(sqlConnection, ServerVersion.AutoDetect(sqlConnection)));

// Repositories
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IDonorRepo, DonorRepo>();
builder.Services.AddScoped<IDonationRepo, DonationRepo>();
builder.Services.AddScoped<IPopulationTypeRepo, PopulationTypeRepo>();
builder.Services.AddScoped<IRecipientRepo, RecipientRepo>();
builder.Services.AddScoped<IAdminRepo, AdminRepo>();
builder.Services.AddScoped<IRedemptionRepo, RedemptionRepo>();
builder.Services.AddScoped<
    IAdminDashboardStatisticsRepo,
    AdminDashboardStatisticsRepo>();

// Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AdminRedemptionService>();
builder.Services.AddScoped<AdminPopulationTypeService>();
builder.Services.AddScoped<AdminDonationService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddScoped<DonationMatchingService>();
builder.Services.AddSingleton<MatchingEventQueue>();
builder.Services.AddHostedService<DonationMatchingWorker>();
builder.Services.AddScoped<EligibilityService>();
builder.Services.AddScoped<RecipientService>();
builder.Services.AddScoped<DonorService>();
builder.Services.AddScoped<PopulationTypeService>();
builder.Services.AddScoped<AdminStatisticsService>();


var jwtKey = builder.Configuration["JWT:KEY"]
    ?? throw new InvalidOperationException("JWT key is missing");

var jwtIssuer = builder.Configuration["JWT:ISSUER"]
    ?? throw new InvalidOperationException("JWT issuer is missing");

var jwtAudience = builder.Configuration["JWT:AUDIENCE"]
    ?? throw new InvalidOperationException("JWT audience is missing");

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


    op.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userIdValue = context.Principal?
                .FindFirst("sub")?.Value;

            if (!int.TryParse(userIdValue, out int userId))
            {
                context.Fail("Invalid user identifier.");
                return;
            }

            var userRepo = context.HttpContext.RequestServices
                .GetRequiredService<IUserRepo>();

            var user = await userRepo.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                context.Fail("User account is inactive or unavailable.");
                return;
            }
        }
    };

});

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed the initial admin account on startup (idempotent).
using (var scope = app.Services.CreateScope())
{
    var config =
        scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var adminName = config["ADMIN_SEED:NAME"];
    var adminPhone = config["ADMIN_SEED:PHONE"];
    var adminPassword = config["ADMIN_SEED:PASSWORD"];

    if (string.IsNullOrWhiteSpace(adminName) ||
        string.IsNullOrWhiteSpace(adminPhone) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        app.Logger.LogWarning(
            "Admin seed skipped: ADMIN_SEED settings are missing.");
    }
    else
    {
        var authService =
            scope.ServiceProvider.GetRequiredService<AuthService>();

        var created = await authService.EnsureAdminExistsAsync(
            adminName,
            adminPhone,
            adminPassword);

        app.Logger.LogInformation(
            created
                ? "Admin seeded successfully (phone: {Phone})."
                : "Admin seed skipped: phone {Phone} already exists.",
            adminPhone);
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", async (AppDbContext dbContext, CancellationToken cancellationToken) =>
    await dbContext.Database.CanConnectAsync(cancellationToken)
        ? Results.Ok(new { status = "healthy" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable));
app.MapControllers();

app.Run();

// Exposes the entry point to WebApplicationFactory.
public partial class Program { }
