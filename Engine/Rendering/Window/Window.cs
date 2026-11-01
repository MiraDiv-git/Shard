using Silk.NET.GLFW;

namespace Engine;

public class Window : IDisposable
{
    private readonly Glfw _glfw = Glfw.GetApi();
    private unsafe WindowHandle* _handle;

    public unsafe Window(int width, int height, string title)
    {
        if (!_glfw.Init())
        {
            throw new EngineException("Could not initialize GLFW");
        }

        Logger.Info("Window", "Creating window...");
        _handle = _glfw.CreateWindow(width, height, title, null, null);
        if (_handle == null)
        {
            throw new EngineException("Could not create GLFW window");
        }

        _glfw.MakeContextCurrent(_handle);
    }

    public void PollEvents() => _glfw.PollEvents();

    public unsafe void SwapBuffers() => _glfw.SwapBuffers(_handle);

    public unsafe bool ShouldClose => _glfw.WindowShouldClose(_handle);

    public unsafe void Dispose()
    {
        _glfw.DestroyWindow(_handle);
        _glfw.Terminate();
    }
}