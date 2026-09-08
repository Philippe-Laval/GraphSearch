namespace GraphSearch.Library.Query.Resolution;

using Microsoft.Data.Sqlite;

/*
 * See sql.md for the schema
   and the data:
   
GraphEntity
   
   Id      Name          Type
   ---------------------------------
   1       Microsoft     Organization
   2       .NET          Technology
   3       .NET 10       Technology
   4       C#            Language
   
   and:
   
   GraphEntityAlias
   
   EntityId    Alias
   -------------------------
   2           DotNet
   2           .NET Framework
   3           DotNet 10
   4           C Sharp  
*/

// This implementation is deliberately simple.
// 
// For a production SQL implementation, I'd use SQLite FTS5 rather than %LIKE% once you have a significant number of entities. SQLite's standard .NET provider can be used with FTS5.

public sealed class SqlEntityIndex : IEntityIndex
{
    private readonly string _connectionString;

    public SqlEntityIndex(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<EntityCandidate>> SearchAsync(
        string text,
        string? entityType,
        int topK,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (topK <= 0)
            return [];

        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            connection.CreateCommand();

        command.CommandText = """
            SELECT
                e.Id,
                e.Name,
                e.Type,

                CASE
                    WHEN lower(e.Name) = lower($text)
                        THEN 1.0

                    WHEN lower(a.Alias) = lower($text)
                        THEN 0.95

                    WHEN lower(e.Name) LIKE lower($prefix)
                        THEN 0.80

                    WHEN lower(a.Alias) LIKE lower($prefix)
                        THEN 0.75

                    ELSE 0.50
                END AS Score

            FROM GraphEntity e

            LEFT JOIN GraphEntityAlias a
                ON a.EntityId = e.Id

            WHERE
                (
                    lower(e.Name) = lower($text)
                    OR lower(a.Alias) = lower($text)

                    OR lower(e.Name) LIKE lower($prefix)
                    OR lower(a.Alias) LIKE lower($prefix)

                    OR lower(e.Name) LIKE lower($contains)
                    OR lower(a.Alias) LIKE lower($contains)
                )

                AND
                (
                    $type IS NULL
                    OR e.Type = $type
                )

            ORDER BY Score DESC, e.Name

            LIMIT $limit;
            """;

        command.Parameters.AddWithValue(
            "$text",
            text);

        command.Parameters.AddWithValue(
            "$prefix",
            $"{text}%");

        command.Parameters.AddWithValue(
            "$contains",
            $"%{text}%");

        command.Parameters.AddWithValue(
            "$type",
            (object?)entityType ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "$limit",
            topK);

        var results = new List<EntityCandidate>();

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(
                new EntityCandidate(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDouble(3)));
        }

        return results;
    }
}