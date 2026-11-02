using Silk.NET.OpenGL;

namespace Engine;

public class GlRenderer : IRenderer, IDisposable
{
    public Window Window { get; }
    public GL GL { get; }
    public Shader Shader { get; }
    public Texture WhiteTexture { get; }
    public SpriteBatch Batch { get; }

    private readonly string _vertShader = @"
#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aColor;
out vec2 vTexCoord;
out vec4 vColor;
uniform mat4 uProjection;
void main() {
    gl_Position = uProjection * vec4(aPos, 0.0, 1.0);
    vTexCoord = aTexCoord;
    vColor = aColor;
}";

    private readonly string _fragShader = @"
#version 330 core
in vec2 vTexCoord;
in vec4 vColor;
out vec4 FragColor;
uniform sampler2D uTexture;
void main() {
    FragColor = texture(uTexture, vTexCoord) * vColor;
}";

    public int Width { get; }
    public int Height { get; }

    public GlRenderer(int width, int height, string title)
    {
        Width = width;
        Height = height;
        Window = new Window(width, height, title);
        GL = Window.CreateGL();
        GLContext.Current = GL;

        Color clearColor = new(0.1f, 0.1f, 0.15f, 1.0f);
        try
        {
            var value = Config.GetString("window.cfg", "CLEAR_COLOR").Trim();
            if (value.StartsWith("#"))
            {
                clearColor = Color.FromHex(value);
            }
            else if (!value.Contains(','))
            {
                clearColor = Color.FromName(value);
            }
            else
            {
                var parts = value.Split(',');
                if (parts.Length >= 3)
                {
                    float r = float.Parse(parts[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    float g = float.Parse(parts[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    float b = float.Parse(parts[2].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    float a = parts.Length >= 4 ? float.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture) : 1f;
                    // If any value > 1, treat as 0-255
                    if (r > 1f || g > 1f || b > 1f || a > 1f)
                        clearColor = Color.FromBytes((byte)r, (byte)g, (byte)b, (byte)a);
                    else
                        clearColor = new Color(r, g, b, a);
                }
            }
        }
        catch { /* use defaults */ }

        GL.ClearColor(clearColor.R, clearColor.G, clearColor.B, clearColor.A);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        Shader = new Shader(_vertShader, _fragShader);
        WhiteTexture = new Texture(1, 1, new byte[] { 255, 255, 255, 255 });
        Batch = new SpriteBatch(Shader, width, height);
    }

    public void Clear()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Viewport(new System.Drawing.Size(Width, Height));
        Batch.Begin();
    }

    public void Present()
    {
        Batch.End();
        Window.SwapBuffers();
        Window.PollEvents();
    }

    public void Render(float alpha)
    {
        Clear();
        Present();
    }

    public bool ShouldClose => Window.ShouldClose;

    public void Dispose()
    {
        Batch.Dispose();
        WhiteTexture.Dispose();
        Shader.Dispose();
        Window.Dispose();
    }
}