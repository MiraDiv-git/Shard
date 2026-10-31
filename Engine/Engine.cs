using System.Diagnostics;

namespace Engine;

public interface IRenderer
{
    void Render(float alpha);
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