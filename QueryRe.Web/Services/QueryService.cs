using QueryRe.Core.Models;
using QueryRe.Data.Services;

namespace QueryRe.Web.Services;

public class QueryService
{
    private readonly SqlService _sql;
    private readonly GeminiService _gemini;
    private readonly HistoryService _history;

    public QueryService(SqlService sql, GeminiService gemini, HistoryService history)
    {
        _sql = sql;
        _gemini = gemini;
        _history = history;
    }

    public async Task<QueryAnswer> AskAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return new QueryAnswer { Error = "Please enter a question." };

        var answer = new QueryAnswer { Question = question };

        try
        {
            // The whole application flow is these three easy steps.
            string schema = await _sql.GetSchemaAsync();
            string generatedSql = await _gemini.MakeSqlAsync(question, schema);
            answer = await _sql.RunQueryAsync(generatedSql);
            answer.Question = question;
        }
        catch (Exception ex)
        {
            answer.Error = $"The request failed: {ex.Message}";
        }

        _history.Add(new QueryLog
        {
            Question = question,
            Sql = answer.Sql,
            Success = answer.Success
        });

        return answer;
    }
}
