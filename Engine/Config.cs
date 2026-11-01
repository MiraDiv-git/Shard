namespace Engine;

public static class Config
{
    public static string Get(string path, string key)
    {
        var targetPath = "./Game/config/" + path;
        if (!File.Exists(targetPath))
            throw new EngineException($"Config: file '{targetPath}' not found");
            
        foreach (var raw in File.ReadLines(targetPath))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] is ';' or '#' or '[') continue;

            var eq = line.IndexOf('=');
            if (eq < 0) continue;

            if (line[..eq].Trim() == key)
                return line[(eq + 1)..].Trim().Trim('"');
        }

        throw new EngineException($"Config: key '{key}' not found in {targetPath}");
    }
}