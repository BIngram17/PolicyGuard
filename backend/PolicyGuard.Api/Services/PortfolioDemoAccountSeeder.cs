using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Data;
using PolicyGuard.Api.Models;

namespace PolicyGuard.Api.Services;

public static class PortfolioDemoAccountSeeder
{
    private const int MinimumPasswordLength = 12;

    public static async Task SeedAsync(
        PolicyGuardDbContext context,
        PasswordService passwordService,
        IConfiguration configuration,
        ILogger logger)
    {
        if (!configuration.GetValue<bool>("PortfolioDemo:Enabled"))
        {
            return;
        }

        var email = configuration["PortfolioDemo:Email"]?.Trim().ToLowerInvariant();
        var password = configuration["PortfolioDemo:Password"];
        var fullName = configuration["PortfolioDemo:FullName"]?.Trim();

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            logger.LogWarning(
                "Portfolio demo account was not provisioned because PortfolioDemo:Email is missing or invalid.");
            return;
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < MinimumPasswordLength)
        {
            logger.LogWarning(
                "Portfolio demo account was not provisioned because PortfolioDemo:Password must be at least {MinimumPasswordLength} characters.",
                MinimumPasswordLength);
            return;
        }

        var user = await context.AppUsers
            .FirstOrDefaultAsync(candidate => candidate.Email.ToLower() == email);

        if (user is null)
        {
            user = new AppUser
            {
                FullName = string.IsNullOrWhiteSpace(fullName) ? "Portfolio Reviewer" : fullName,
                Email = email,
                PasswordHash = passwordService.HashPassword(password),
                Role = "Reviewer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.AppUsers.Add(user);
        }
        else
        {
            user.FullName = string.IsNullOrWhiteSpace(fullName) ? "Portfolio Reviewer" : fullName;
            user.Role = "Reviewer";
            user.IsActive = true;

            if (!passwordService.VerifyPassword(password, user.PasswordHash))
            {
                user.PasswordHash = passwordService.HashPassword(password);
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation(
            "Portfolio demo account is ready for {DemoEmail} with the Reviewer role.",
            email);
    }
}
