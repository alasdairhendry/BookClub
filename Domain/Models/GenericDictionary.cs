namespace Domain.Models;

public class GenericDictionary<TKey> where TKey : class
{
    private Dictionary<TKey, object> _dict = new Dictionary<TKey, object>();

    public void Add<T>(TKey key, T value) where T : class
    {
        _dict.TryAdd(key, value);
    }

    public T? GetValue<T>(TKey key) where T : class
    {
        if (_dict.TryGetValue(key, out var value))
            return value as T;

        return null;
    }
}