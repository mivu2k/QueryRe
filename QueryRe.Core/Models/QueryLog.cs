namespace QueryRe.Core.Models;

public class QueryLog
{
    public DateTime Time { get; set; } = DateTime.Now;
    public string Question { get; set; } = "";
    public string Sql { get; set; } = "";
    public bool Success { get; set; }
}
