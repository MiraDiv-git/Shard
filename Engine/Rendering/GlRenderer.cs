namespace Engine;

public class GlRenderer: IRenderer
{
    public Window Window { get; }
    public GlRenderer(int width, int height, string title)
    {
        Window = new Window(width, height, title);
    }

    public void Render(float alpha)
    {
        Window.SwapBuffers();
        Window.PollEvents();
    }

    public bool ShouldClose => Window.ShouldClose;
}