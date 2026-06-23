namespace SunamoWinStd;

internal static class Net48CollectionPolyfillExtensions
{
    internal static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            return false;
        }
        dictionary.Add(key, value);
        return true;
    }
}