using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace QueryRe.Web.Services;

public class GeminiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public GeminiService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<string> MakeSqlAsync(string question, string schema)
    {
        string apiKey = _configuration["Gemini:ApiKey"]
            ?? throw new Exception("The Gemini API key is missing.");
        string baseUrl = _configuration["Gemini:BaseUrl"]
            ?? "https://generativelanguage.googleapis.com/v1beta";
        string model = _configuration["Gemini:Model"]
            ?? "gemini-3.1-flash-lite";

        string prompt = $"""
            DATABASE SCHEMA:
            {schema}

            QUESTION:
            {question}
            """;

        var requestBody = new
        {
            model,
            system_instruction = "Write one Microsoft SQL Server SELECT query. " +
                "Return only SQL. Never use INSERT, UPDATE, DELETE, DROP, ALTER or CREATE.",
            input = prompt,
            generation_config = new { temperature = 0 }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/interactions");
        request.Headers.Add("x-goog-api-key", apiKey);
        request.Content = JsonContent.Create(requestBody);

        using HttpResponseMessage response = await _http.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Gemini API error: {response.StatusCode} {responseBody}");

        using JsonDocument json = JsonDocument.Parse(responseBody);
        string output = ReadModelOutput(json.RootElement);

        if (string.IsNullOrWhiteSpace(output))
            throw new Exception("Gemini did not return SQL.");

        return CleanSql(output);
    }

    private static string ReadModelOutput(JsonElement root)
    {
        if (!root.TryGetProperty("steps", out JsonElement steps))
            return "";

        var result = new StringBuilder();

        foreach (JsonElement step in steps.EnumerateArray())
        {
            if (!step.TryGetProperty("type", out JsonElement stepType) ||
                stepType.GetString() != "model_output" ||
                !step.TryGetProperty("content", out JsonElement content))
            {
                continue;
            }

            foreach (JsonElement item in content.EnumerateArray())
            {
                if (item.TryGetProperty("type", out JsonElement itemType) &&
                    itemType.GetString() == "text" &&
                    item.TryGetProperty("text", out JsonElement text))
                {
                    result.Append(text.GetString());
                }
            }
        }

        return result.ToString();
    }

    private static string CleanSql(string text)
    {
        string cleaned = text
            .Replace("```sql", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "")
            .Trim();

        int selectPosition = cleaned.IndexOf(
            "SELECT", StringComparison.OrdinalIgnoreCase);
        int withPosition = cleaned.IndexOf(
            "WITH", StringComparison.OrdinalIgnoreCase);

        int startPosition = selectPosition;
        if (withPosition >= 0 && (selectPosition < 0 || withPosition < selectPosition))
            startPosition = withPosition;

        if (startPosition >= 0)
            cleaned = cleaned[startPosition..];

        int semicolonPosition = cleaned.IndexOf(';');
        if (semicolonPosition >= 0)
            cleaned = cleaned[..(semicolonPosition + 1)];

        return cleaned.Trim();
    }
}
