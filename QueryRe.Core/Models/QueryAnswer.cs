namespace QueryRe.Core.Models;

// One model carries everything the page needs to display.
public class QueryAnswer
{
    public bool Success { get; set; }
    public string Question { get; set; } = "";
    public string Sql { get; set; } = "";
    public string Error { get; set; } = "";
    public List<string> Columns { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
}
