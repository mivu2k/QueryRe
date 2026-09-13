namespace QueryRe.Core.Services;

public static class SqlValidator
{
    // This is intentionally small enough to explain in a viva.
    // SQL Server permissions are the real final safety layer.
    private static readonly string[] BlockedWords =
    {
        "insert", "update", "delete", "drop", "alter",
        "create", "truncate", "merge", "exec", "execute",
        "grant", "revoke", "into", "openrowset",
        "opendatasource", "xp_cmdshell", "waitfor"
    };

    public static bool IsSafe(string sql, out string reason)
    {
        reason = "";

        if (string.IsNullOrWhiteSpace(sql))
        {
            reason = "The model did not return SQL.";
            return false;
        }

        string clean = sql.Trim();
        bool startsCorrectly = clean.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
            || clean.StartsWith("WITH", StringComparison.OrdinalIgnoreCase);

        if (!startsCorrectly)
        {
            reason = "Only SELECT queries are allowed.";
            return false;
        }

        // A semicolon is allowed only as the final character.
        string withoutFinalSemicolon = clean.TrimEnd(';').TrimEnd();
        if (withoutFinalSemicolon.Contains(';'))
        {
            reason = "Multiple SQL statements are not allowed.";
            return false;
        }

        foreach (string word in BlockedWords)
        {
            if (ContainsWord(withoutFinalSemicolon, word))
            {
                reason = $"The word '{word}' is blocked.";
                return false;
            }
        }

        return true;
    }

    private static bool ContainsWord(string text, string word)
    {
        string[] parts = text.Split(
            new[] { ' ', '\t', '\r', '\n', '(', ')', ',', '.', '[', ']' },
            StringSplitOptions.RemoveEmptyEntries);

        return parts.Any(part => part.Equals(word, StringComparison.OrdinalIgnoreCase));
    }
}
