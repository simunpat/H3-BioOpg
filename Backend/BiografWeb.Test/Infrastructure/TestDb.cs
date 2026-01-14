using System;
using System.Threading.Tasks;
using BiografWeb.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;

namespace BiografWeb.Test.Infrastructure;

public sealed class TestDb : IAsyncDisposable
{
    public AppDbContext Db { get; }
    private readonly SqliteConnection _connection;

    public TestDb()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseSnakeCaseNamingConvention()
            .Options;

        Db = new AppDbContext(options);
        Db.Database.Migrate();
    }

    public async ValueTask DisposeAsync()
    {
        await Db.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

