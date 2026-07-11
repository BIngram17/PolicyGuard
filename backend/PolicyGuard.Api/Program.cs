using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PolicyGuard.Api.Data;
using PolicyGuard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = ResolveAllowedOrigins(builder.Configuration, builder.Environment);
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT key is missing. Set Jwt:Key locally or Jwt__Key as a cloud app setting.");
}

builder.Services.AddControllers();

builder.Services.AddDbContext<PolicyGuardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PolicyAnalyzerService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var issuer = builder.Configuration["Jwt:Issuer"] ?? "PolicyGuard";
    var audience = builder.Configuration["Jwt:Audience"] ?? "PolicyGuardClient";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfiguredOrigins", policy =>
    {
        // Enterprise deployments should explicitly list the frontend URL.
        // In Azure App Service, set Cors__AllowedOrigins__0=https://your-frontend-url.
        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();

            return;
        }

        // No origins means browser CORS requests are denied by default.
        // This is intentionally safer than allowing every origin in production.
        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PolicyGuard API",
        Version = "v1",
        Description = "Policy compliance analyzer API with JWT authentication and role-based access control."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token like this: Bearer YOUR_TOKEN_HERE",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed portfolio-ready default checklist templates at startup.
// This is safe to run repeatedly because the seeder checks for existing checklist names.
await SeedDefaultChecklistsAsync(app);

app.UseCors("AllowConfiguredOrigins");

var swaggerEnabled = app.Environment.IsDevelopment()
    || string.Equals(builder.Configuration["Swagger:Enabled"], "true", StringComparison.OrdinalIgnoreCase);

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new
{
    service = "PolicyGuard.Api",
    status = "Healthy",
    environment = app.Environment.EnvironmentName,
    timestampUtc = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();

static async Task SeedDefaultChecklistsAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PolicyGuardDbContext>();

        await DefaultChecklistSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        // Seeding should not prevent the API from starting. If the database is temporarily
        // unavailable during startup, the existing app routes should still remain reachable.
        app.Logger.LogError(ex, "Default checklist seeding failed.");
    }
}

static string[] ResolveAllowedOrigins(IConfiguration configuration, IWebHostEnvironment environment)
{
    var configuredOrigins = configuration
        .GetSection("Cors:AllowedOrigins")
        .GetChildren()
        .Select(child => child.Value)
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value!.Trim().TrimEnd('/'))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    if (configuredOrigins.Length > 0)
    {
        return configuredOrigins;
    }

    return environment.IsDevelopment()
        ? new[] { "http://localhost:5173" }
        : Array.Empty<string>();
}
