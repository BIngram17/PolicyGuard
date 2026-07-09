using PolicyGuard.Api.Services;
using Xunit;

namespace PolicyGuard.Api.Tests.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _service = new();

    [Fact]
    public void HashPassword_WhenCalled_ReturnsSaltAndHashSeparatedByPeriod()
    {
        var storedHash = _service.HashPassword("CorrectHorseBatteryStaple!");

        var parts = storedHash.Split('.', 2);

        Assert.Equal(2, parts.Length);
        Assert.False(string.IsNullOrWhiteSpace(parts[0]));
        Assert.False(string.IsNullOrWhiteSpace(parts[1]));
    }

    [Fact]
    public void HashPassword_WhenSamePasswordIsHashedTwice_ProducesUniqueHashes()
    {
        var firstHash = _service.HashPassword("CorrectHorseBatteryStaple!");
        var secondHash = _service.HashPassword("CorrectHorseBatteryStaple!");

        Assert.NotEqual(firstHash, secondHash);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordMatchesStoredHash_ReturnsTrue()
    {
        var storedHash = _service.HashPassword("CorrectHorseBatteryStaple!");

        var isValid = _service.VerifyPassword("CorrectHorseBatteryStaple!", storedHash);

        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_WhenPasswordDoesNotMatchStoredHash_ReturnsFalse()
    {
        var storedHash = _service.HashPassword("CorrectHorseBatteryStaple!");

        var isValid = _service.VerifyPassword("WrongPassword!", storedHash);

        Assert.False(isValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-valid-hash")]
    [InlineData("not-base64.hash")]
    public void VerifyPassword_WhenStoredHashIsInvalid_ReturnsFalse(string storedHash)
    {
        var isValid = _service.VerifyPassword("CorrectHorseBatteryStaple!", storedHash);

        Assert.False(isValid);
    }
}
