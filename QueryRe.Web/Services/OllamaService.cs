using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace QueryRe.Web.Services;

public class OllamaService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public OllamaService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<string> MakeSqlAsync(string question, string schema)
    {
        string baseUrl = _configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
        string model = _configuration["Ollama:Model"] ?? "sqlcoder";

        string prompt = $"""
            You write Microsoft SQL Server SELECT queries.
            Return only one SQL query. Do not explain it.
            Never use INSERT, UPDATE, DELETE, DROP, ALTER or CREATE.

            DATABASE SCHEMA:
            {schema}

            QUESTION:
            {question}

            SQL:
            """;

        var request = new
        {
            model,
            prompt,
            stream = false,
            options = new { temperature = 0 }
        };

        HttpResponseMessage response = await _http.PostAsJsonAsync(
            $"{baseUrl.TrimEnd('/')}/api/generate", request);

        response.EnsureSuccessStatusCode();

        OllamaResponse? data = await response.Content.ReadFromJsonAsync<OllamaResponse>();
        return CleanSql(data?.Response ?? "");
    }

    private static string CleanSql(string text)
    {
        return text
            .Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "")
            .Trim();
    }

    private class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = "";
    }
}
