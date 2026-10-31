namespace Engine;

public static class Config
{
    public static string Get(string path, string key)
    {
        foreach (var raw in File.ReadLines("./Game/config/" + path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] is ';' or '#' or '[') continue;

            var eq = line.IndexOf('=');
            if (eq < 0) continue;

            if (line[..eq].Trim() == key)
                return line[(eq + 1)..].Trim().Trim('"');
        }

        throw new Exception($"Key '{key}' not found in {path}");
    }
}