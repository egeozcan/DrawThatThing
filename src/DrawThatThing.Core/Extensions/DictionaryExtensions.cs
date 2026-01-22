namespace DrawThatThing.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static string GetStringOrDefault(this IDictionary<string, string>? dictionary, string key, string defaultValue = "")
    {
        if (dictionary == null) return defaultValue;
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static int GetIntOrDefault(this IDictionary<string, string>? dictionary, string key, int defaultValue = 0)
    {
        if (dictionary == null) return defaultValue;
        return dictionary.TryGetValue(key, out var value) ? value.ToInt(defaultValue) : defaultValue;
    }

    public static bool GetBoolOrDefault(this IDictionary<string, string>? dictionary, string key, bool defaultValue = false)
    {
        if (dictionary == null) return defaultValue;
        return dictionary.TryGetValue(key, out var value) ? value.ToBool(defaultValue) : defaultValue;
    }
}
