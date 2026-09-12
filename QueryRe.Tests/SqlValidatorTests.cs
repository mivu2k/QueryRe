using QueryRe.Core.Services;
using Xunit;

namespace QueryRe.Tests;

public class SqlValidatorTests
{
    [Fact]
    public void Select_is_allowed()
    {
        bool safe = SqlValidator.IsSafe("SELECT * FROM Customer", out _);
        Assert.True(safe);
    }

    [Fact]
    public void Delete_is_blocked()
    {
        bool safe = SqlValidator.IsSafe("DELETE FROM Customer", out _);
        Assert.False(safe);
    }

    [Fact]
    public void Select_with_delete_is_blocked()
    {
        bool safe = SqlValidator.IsSafe(
            "SELECT * FROM Customer; DELETE FROM Customer", out _);
        Assert.False(safe);
    }

    [Fact]
    public void Empty_sql_is_blocked()
    {
        bool safe = SqlValidator.IsSafe("", out _);
        Assert.False(safe);
    }

    [Fact]
    public void With_query_is_allowed()
    {
        bool safe = SqlValidator.IsSafe(
            "WITH totals AS (SELECT 1 AS Number) SELECT * FROM totals", out _);
        Assert.True(safe);
    }
}
