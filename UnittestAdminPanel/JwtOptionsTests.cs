using Diplom.Data.Auth;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TestProject1;

public class JwtOptionsTests
{
    [Fact(DisplayName = "JWT-ключ короче 32 символов вызывает ошибку")]
    public void CreateSecurityKey_ThrowsWhenSecretIsTooShort()
    {
        var options = new JwtOptions
        {
            SecretKey = "short"
        };

        var exception = Assert.Throws<InvalidOperationException>(options.CreateSecurityKey);

        Assert.Contains("32 characters", exception.Message);
    }

    [Fact(DisplayName = "Параметры JWT используют заданные issuer и audience")]
    public void CreateValidationParameters_UsesConfiguredIssuerAudienceAndLifetime()
    {
        var options = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "TestSecretKey_32_Characters_Minimum"
        };

        var parameters = options.CreateValidationParameters();

        Assert.True(parameters.ValidateIssuer);
        Assert.Equal("TestIssuer", parameters.ValidIssuer);
        Assert.True(parameters.ValidateAudience);
        Assert.Equal("TestAudience", parameters.ValidAudience);
        Assert.True(parameters.ValidateLifetime);
        Assert.IsType<SymmetricSecurityKey>(parameters.IssuerSigningKey);
    }

    [Fact(DisplayName = "JWT-ключ создаётся из заданного секрета")]
    public void CreateSecurityKey_UsesConfiguredSecret()
    {
        const string secret = "AnotherTestSecretKey_32_Characters";
        var options = new JwtOptions
        {
            SecretKey = secret
        };

        var key = options.CreateSecurityKey();

        Assert.Equal(Encoding.UTF8.GetByteCount(secret), key.KeySize / 8);
    }

    [Fact(DisplayName = "Параметры JWT допускают расхождение времени в одну минуту")]
    public void CreateValidationParameters_UsesOneMinuteClockSkew()
    {
        var options = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "ClockSkewSecretKey_32_Characters_Min"
        };

        var parameters = options.CreateValidationParameters();

        Assert.Equal(TimeSpan.FromMinutes(1), parameters.ClockSkew);
    }
}
