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

    public EngineRuntime(int width = 1280, int height = 720, string title = "Shard Engine", IRenderer? renderer = null)
    {
        _renderer = renderer ?? new GlRenderer(width, height, title);
    }

    public void Stop() => _running = false;

    public void Run()
    {
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

            if (_renderer is GlRenderer gr && gr.Window.ShouldClose)
            {
                _running = false;
                Logger.Info("EngineRuntime", "Close requested, shutting down...");
            }

            Thread.Sleep(1);
        }

        Scripts.Shutdown();
    }
}