using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Application.Users;
using BiografWeb.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace BiografWeb.Test.Api;

public class AuthControllerTests
{
    private static string Sha256Hex(string s)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));

        return string.Concat(bytes.Select(b => b.ToString("x2")));
    }

    private static IConfiguration BuildJwtConfig()
    {
        var dict = new System.Collections.Generic.Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "test-iss",
            ["Jwt:Audience"] = "test-aud",
            ["Jwt:Key"] = "0123456789abcdef0123456789abcdef0123456789abcdef"
        };

        return new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();
    }

    /// <summary>
    /// Verifies that Login returns HTTP 400 BadRequest when either email or password is missing.
    /// </summary>
    [Theory]
    [InlineData("", "pw")]
    [InlineData("a@b.c", "")]
    public async Task Login_Returns_BadRequest_On_Missing_Fields(string email, string password)
    {
        var svc = Substitute.For<IUsersService>();
        var ctl = new AuthController(svc, BuildJwtConfig());
        var res = await ctl.Login(new AuthController.LoginRequest(email, password), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(res.Result);
    }

    /// <summary>
    /// Verifies that Login returns HTTP 401 Unauthorized when the supplied password is incorrect.
    /// </summary>
    [Fact]
    public async Task Login_Unauthorized_On_Wrong_Password()
    {
        var svc = Substitute.For<IUsersService>();
        var salt = Guid.NewGuid().ToString();
        var rightPassword = "secret";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "u@test",
            IsAdmin = false,
            PasswordSalt = salt,
            PasswordHash = Sha256Hex(rightPassword + salt),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        svc.FindByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        var ctl = new AuthController(svc, BuildJwtConfig());
        var res = await ctl.Login(new AuthController.LoginRequest(user.Email, "wrong"), CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(res.Result);
    }

    /// <summary>
    /// Verifies that Login returns HTTP 200 OK and a JWT containing the expected claims (sub/email/isAdmin) when credentials are valid.
    /// </summary>
    [Fact]
    public async Task Login_Returns_Jwt_On_Success_With_Claims()
    {
        var svc = Substitute.For<IUsersService>();
        var salt = Guid.NewGuid().ToString();
        var password = "secret";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "u@test",
            IsAdmin = true,
            PasswordSalt = salt,
            PasswordHash = Sha256Hex(password + salt),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        svc.FindByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);

        var ctl = new AuthController(svc, BuildJwtConfig());
        var res = await ctl.Login(new AuthController.LoginRequest(user.Email, password), CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(res.Result);
        var payload = Assert.IsType<AuthController.LoginResponse>(ok.Value);

        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.Contains(".", payload.Token);

        // Decode middle part of JWT to verify claims exist (not verifying signature)
        var parts = payload.Token.Split('.');
        Assert.True(parts.Length >= 2);

        string B64UrlToString(string s)
        {
            string p = s.Replace('-', '+').Replace('_', '/');
            switch (p.Length % 4) { case 2: p += "=="; break; case 3: p += "="; break; }
            return Encoding.UTF8.GetString(Convert.FromBase64String(p));
        }

        var claimsJson = B64UrlToString(parts[1]);
        using var doc = JsonDocument.Parse(claimsJson);

        Assert.Equal(user.Id.ToString(), doc.RootElement.GetProperty("sub").GetString());
        Assert.Equal(user.Email, doc.RootElement.GetProperty("email").GetString());
        Assert.Equal("true", doc.RootElement.GetProperty("isAdmin").GetString());
    }
}

