using QueryRe.Core.Models;

namespace QueryRe.Data.Services;

public class HistoryService
{
    private readonly List<QueryLog> _items = new();

    public void Add(QueryLog item)
    {
        _items.Insert(0, item);

        // Keep only the latest 50 questions.
        if (_items.Count > 50)
            _items.RemoveAt(_items.Count - 1);
    }

    public List<QueryLog> GetAll() => _items.ToList();

    public void Clear() => _items.Clear();
}
