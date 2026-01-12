using System.Data.Common;
using BiografWeb.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiografWeb.Api.Controllers;

[ApiController]
[Route("hack")]
public class HackController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    public sealed record SqlRequest(string Sql);

    [HttpPost("sql")]
    public async Task<IActionResult> Execute([FromBody] SqlRequest request, CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Sql))
        {
            return BadRequest(new { error = "sql is required" });
        }

        await using var connection = _db.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(ct);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = request.Sql;

        // Simple heuristic: if it looks like a SELECT, return rows; otherwise return affected count
        var isQuery = request.Sql.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase);

        try
        {
            if (!isQuery)
            {
                var affected = await command.ExecuteNonQueryAsync(ct);
                return Ok(new { affected });
            }

            await using var reader = await command.ExecuteReaderAsync(ct);
            var rows = await ReadAllRowsAsync(reader, ct);

            return Ok(rows);
        }
        catch (Exception ex)
        {
            // Intentionally return error details to make the lab observable
            return BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<List<Dictionary<string, object?>>> ReadAllRowsAsync(DbDataReader reader, CancellationToken ct)
    {
        var results = new List<Dictionary<string, object?>>();

        while (await reader.ReadAsync(ct))
        {
            var row = new Dictionary<string, object?>(reader.FieldCount, StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var isNull = await reader.IsDBNullAsync(i, ct);
                row[reader.GetName(i)] = isNull ? null : reader.GetValue(i);
            }

            results.Add(row);
        }

        return results;
    }
}

