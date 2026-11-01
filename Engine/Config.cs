using System.Reflection;

namespace Engine;

public static class Config
{
    public static string GetString(string path, string key)
    {
        var lines = ReadResourceLines(path);
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] is ';' or '#' or '[') continue;

            var eq = line.IndexOf('=');
            if (eq < 0) continue;

            if (line[..eq].Trim() == key)
                return line[(eq + 1)..].Trim().Trim('"');
        }

        throw new EngineException($"Config: key '{key}' not found in {path}");
    }

    public static bool GetBool(string path, string key)
    {
        var value = GetString(path, key);
        if (bool.TryParse(value, out var result))
            return result;

        return value.ToLowerInvariant() is "1" or "yes" or "on";
    }

    public static int GetInt(string path, string key)
    {
        var value = GetString(path, key);
        if (int.TryParse(value, out var result))
            return result;

        throw new EngineException($"Config: '{value}' is not a valid integer for key '{key}'");
    }

    private static IEnumerable<string> ReadResourceLines(string path)
    {
        var normalized = path.Replace('\\', '/');
        try { Assembly.Load("Game"); } catch { }
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var name in asm.GetManifestResourceNames())
            {
                if (name.EndsWith(normalized, StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith(normalized.Replace('/', '.'), StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = asm.GetManifestResourceStream(name);
                    if (stream is null) continue;
                    using var reader = new StreamReader(stream);
                    string? line;
                    while ((line = reader.ReadLine()) is not null)
                        yield return line;
                    yield break;
                }
            }
        }

        throw new EngineException($"Config: resource '{path}' not found");
    }
}