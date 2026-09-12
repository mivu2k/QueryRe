using System.Text;
using Microsoft.Data.SqlClient;
using QueryRe.Core.Models;
using QueryRe.Core.Services;

namespace QueryRe.Data.Services;

public class SqlService
{
    private readonly string _connectionString;

    public SqlService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string> GetSchemaAsync()
    {
        const string schemaSql = """
            SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo'
            ORDER BY TABLE_NAME, ORDINAL_POSITION;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(schemaSql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var schema = new StringBuilder();
        string currentTable = "";

        while (await reader.ReadAsync())
        {
            string table = reader.GetString(1);

            if (table != currentTable)
            {
                currentTable = table;
                schema.AppendLine($"Table: dbo.{table}");
            }

            schema.AppendLine($"- {reader.GetString(2)} ({reader.GetString(3)})");
        }

        return schema.ToString();
    }

    public async Task<QueryAnswer> RunQueryAsync(string sql)
    {
        var answer = new QueryAnswer { Sql = sql };

        if (!SqlValidator.IsSafe(sql, out string reason))
        {
            answer.Error = reason;
            return answer;
        }

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.CommandTimeout = 30;

        await using var reader = await command.ExecuteReaderAsync();

        for (int i = 0; i < reader.FieldCount; i++)
            answer.Columns.Add(reader.GetName(i));

        // Display at most 100 rows so the browser stays responsive.
        while (answer.Rows.Count < 100 && await reader.ReadAsync())
        {
            var row = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
                row.Add(reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString() ?? "");

            answer.Rows.Add(row);
        }

        answer.Success = true;
        return answer;
    }
}
