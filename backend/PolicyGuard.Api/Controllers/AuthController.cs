using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Data;
using PolicyGuard.Api.DTOs;
using PolicyGuard.Api.Models;
using PolicyGuard.Api.Services;

namespace PolicyGuard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly PolicyGuardDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly AuditLogService _auditLogService;
    private readonly IConfiguration _configuration;

    public AuthController(
        PolicyGuardDbContext context,
        PasswordService passwordService,
        JwtTokenService jwtTokenService,
        AuditLogService auditLogService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _auditLogService = auditLogService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(appUser => appUser.Email.ToLower() == normalizedEmail);

        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var passwordIsValid = _passwordService.VerifyPassword(
            request.Password,
            user.PasswordHash
        );

        if (!passwordIsValid)
        {
            await _auditLogService.LogAsync(
                action: "LOGIN_FAILED",
                entityType: "AppUser",
                entityId: user.Id,
                summary: $"Failed login attempt for {user.Email}",
                details: "Password verification failed.",
                performedBy: user.Email
            );

            return Unauthorized(new { message = "Invalid email or password." });
        }

        return await CreateLoginResponseAsync(user, $"Role: {user.Role}");
    }

    [HttpPost("demo-login")]
    public async Task<IActionResult> DemoLogin([FromBody] DemoLoginRequest request)
    {
        var demoEnabled = _configuration.GetValue<bool>("PortfolioDemo:Enabled");
        var configuredToken = _configuration["PortfolioDemo:AccessToken"];
        var demoEmail = _configuration["PortfolioDemo:Email"]?.Trim().ToLowerInvariant();

        if (!demoEnabled ||
            string.IsNullOrWhiteSpace(configuredToken) ||
            string.IsNullOrWhiteSpace(demoEmail) ||
            string.IsNullOrWhiteSpace(request.AccessToken) ||
            !TokensMatch(request.AccessToken, configuredToken))
        {
            return Unauthorized(new { message = "This demo link is invalid or has expired." });
        }

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(appUser => appUser.Email.ToLower() == demoEmail);

        if (user == null || !user.IsActive || user.Role != "Reviewer")
        {
            return Unauthorized(new { message = "Demo access is temporarily unavailable." });
        }

        return await CreateLoginResponseAsync(
            user,
            "Role: Reviewer; Method: Portfolio demo link.");
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { message = "Full name is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Email is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return BadRequest(new { message = "Password must be at least 8 characters." });
        }

        var allowedRoles = new[] { "Admin", "Reviewer", "Auditor" };

        if (!allowedRoles.Contains(request.Role))
        {
            return BadRequest(new { message = "Role must be Admin, Reviewer, or Auditor." });
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        var emailExists = await _context.AppUsers
            .AnyAsync(user => user.Email.ToLower() == normalizedEmail);

        if (emailExists)
        {
            return BadRequest(new { message = "A user with this email already exists." });
        }

        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            action: "USER_REGISTERED",
            entityType: "AppUser",
            entityId: user.Id,
            summary: $"User registered: {user.Email}",
            details: $"Role: {user.Role}",
            performedBy: user.Email
        );

        return CreatedAtAction(
            nameof(Login),
            new { email = user.Email },
            new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.CreatedAt
            }
        );
    }

    private async Task<IActionResult> CreateLoginResponseAsync(AppUser user, string auditDetails)
    {
        var expirationMinutesRaw = _configuration["Jwt:ExpirationMinutes"];
        var expirationMinutes = int.TryParse(expirationMinutesRaw, out var parsedMinutes)
            ? parsedMinutes
            : 480;

        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var token = _jwtTokenService.GenerateToken(user, expiresAt);

        await _auditLogService.LogAsync(
            action: "LOGIN_SUCCESS",
            entityType: "AppUser",
            entityId: user.Id,
            summary: $"User logged in: {user.Email}",
            details: auditDetails,
            performedBy: user.Email
        );

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        });
    }

    private static bool TokensMatch(string suppliedToken, string configuredToken)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(suppliedToken));
        var configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(configuredToken));

        return CryptographicOperations.FixedTimeEquals(suppliedHash, configuredHash);
    }
}
