using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BiografWeb.Api.Controllers;
using BiografWeb.Test.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BiografWeb.Test.Api;

public class HackControllerTests
{
    /// <summary>
    /// Ensures a simple SELECT returns HTTP 200 with an enumerable result payload.
    /// </summary>
    [Fact]
    public async Task Execute_Select_Returns_Array()
    {
        await using var t = new TestDb();

        var ctl = new HackController(t.Db);
        var res = await ctl.Execute(new HackController.SqlRequest("select 1 as x"), CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(res);
        var list = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);

        Assert.True(list.Cast<object>().Any());
    }

    /// <summary>
    /// Ensures a non-SELECT statement returns HTTP 200 and an object containing the number of affected rows.
    /// </summary>
    [Fact]
    public async Task Execute_NonSelect_Returns_Affected()
    {
        await using var t = new TestDb();

        var ctl = new HackController(t.Db);

        // Create table
        var r1 = await ctl.Execute(new HackController.SqlRequest("create table t(x int)"), CancellationToken.None);
        Assert.IsType<OkObjectResult>(r1);

        // Insert row
        var r2 = await ctl.Execute(new HackController.SqlRequest("insert into t(x) values (1)"), CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(r2);
        var affected = ok.Value!.GetType().GetProperty("affected")!.GetValue(ok.Value, null);

        Assert.Equal(1, affected);
    }

    /// <summary>
    /// Ensures invalid SQL returns HTTP 400 with an 'error' field in the response.
    /// </summary>
    [Fact]
    public async Task Execute_InvalidSql_Returns_BadRequest_With_Error()
    {
        await using var t = new TestDb();

        var ctl = new HackController(t.Db);
        var res = await ctl.Execute(new HackController.SqlRequest("select * from non_existing"), CancellationToken.None);
        var bad = Assert.IsType<BadRequestObjectResult>(res);
        var errorProp = bad.Value!.GetType().GetProperty("error");

        Assert.NotNull(errorProp);
    }
}

