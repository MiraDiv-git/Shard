using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Engine;

public interface IRenderer
{
    void Render(float alpha);
}

public static class Config
{
    public static string Get(string path, string key)
    {
        foreach (var raw in File.ReadLines(path))
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

public class EngineRuntime
{
    private readonly IRenderer _renderer;
    private bool _running;

    public ScriptHost Scripts { get; } = new();

    public EngineRuntime(IRenderer renderer)
    {
        _renderer = renderer;
    }

    public void Stop() => _running = false;

    public void Run()
    {
        EngineDebug.PrintStartMessage();

        const double fixedDeltaTime = 1.0 / 60.0;
        const double maxFrameTime = 0.25;

        var accumulator = 0.0;
        var lastFrameTime = Stopwatch.GetTimestamp();
        _running = true;

        while (_running)
        {
            var currentFrameTime = Stopwatch.GetTimestamp();
            var deltaTime = (double)(currentFrameTime - lastFrameTime) / Stopwatch.Frequency;
            lastFrameTime = currentFrameTime;

            accumulator += Math.Min(deltaTime, maxFrameTime);

            Scripts.Flush();

            while (accumulator >= fixedDeltaTime)
            {
                Scripts.Update(fixedDeltaTime);
                accumulator -= fixedDeltaTime;
            }

            _renderer.Render((float)(accumulator / fixedDeltaTime));

            Thread.Sleep(1);
        }

        Scripts.Shutdown();
    }
}

public static class EngineDebug
{
    public static void PrintStartMessage()
    {
        Console.WriteLine("Shard Engine v 0.1.0");
        Console.WriteLine("(c) 2026 MiraDiv. All rights reserved.\n");
    }
}