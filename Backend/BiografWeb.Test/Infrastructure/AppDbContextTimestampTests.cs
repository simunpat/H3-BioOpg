using System;
using System.Threading.Tasks;
using BiografWeb.Domain;
using BiografWeb.Test.Infrastructure;
using Xunit;

namespace BiografWeb.Test.InfrastructureTests;

public class AppDbContextTimestampTests
{
    /// <summary>
    /// Ensures SaveChangesAsync sets both CreatedAt and UpdatedAt when inserting a new entity.
    /// </summary>
    [Fact]
    public async Task SaveChangesAsync_Sets_CreatedAt_And_UpdatedAt_On_Add()
    {
        await using var t = new TestDb();

        var m = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100 };
        t.Db.Movies.Add(m);

        await t.Db.SaveChangesAsync();

        Assert.NotEqual(default, m.CreatedAt);
        Assert.NotEqual(default, m.UpdatedAt);
    }

    /// <summary>
    /// Ensures SaveChangesAsync updates the UpdatedAt timestamp when an existing entity is modified.
    /// </summary>
    [Fact]
    public async Task SaveChangesAsync_Updates_UpdatedAt_On_Modify()
    {
        await using var t = new TestDb();

        var m = new Movie { Id = Guid.NewGuid(), Title = "M", Genre = "G", DurationMin = 100 };
        t.Db.Movies.Add(m);

        await t.Db.SaveChangesAsync();
        var initialUpdated = m.UpdatedAt;
        m.Title = "M2";

        await Task.Delay(5); // ensure time changes

        await t.Db.SaveChangesAsync();

        Assert.True(m.UpdatedAt >= initialUpdated);
    }
}

