using Silk.NET.OpenGL;
using System.Numerics;

namespace Engine;

public class SpriteBatch : IDisposable
{
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly uint _vao;
    private readonly uint _vbo;
    private readonly uint _ibo;

    private const int MaxQuads = 10000;
    private const int MaxVertices = MaxQuads * 4;
    private const int MaxIndices = MaxQuads * 6;

    private readonly float[] _vertices = new float[MaxVertices * 8]; // pos2 + uv2 + color4
    private readonly uint[] _indices = new uint[MaxIndices];
    private int _quadCount;
    private Texture? _currentTexture;

    private readonly float[] _projection = new float[16];

    public unsafe SpriteBatch(Shader shader, int screenWidth, int screenHeight) : this(GLContext.Current!, shader, screenWidth, screenHeight) { }

    public unsafe SpriteBatch(GL gl, Shader shader, int screenWidth, int screenHeight)
    {
        _gl = gl;
        _shader = shader;

        // Generate buffers
        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();
        _ibo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(MaxVertices * 8 * sizeof(float)), null, BufferUsageARB.DynamicDraw);

        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ibo);
        // Pre-fill index buffer
        for (int i = 0, j = 0; i < MaxIndices; i += 6, j += 4)
        {
            _indices[i] = (uint)j;
            _indices[i + 1] = (uint)(j + 1);
            _indices[i + 2] = (uint)(j + 2);
            _indices[i + 3] = (uint)j;
            _indices[i + 4] = (uint)(j + 2);
            _indices[i + 5] = (uint)(j + 3);
        }
        unsafe
        {
            fixed (uint* ptr = _indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(MaxIndices * sizeof(uint)), ptr, BufferUsageARB.StaticDraw);
            }
        }

        // Vertex attributes: pos(2), uv(2), color(4)
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(2 * sizeof(float)));
        _gl.EnableVertexAttribArray(2);
        _gl.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(4 * sizeof(float)));

        _gl.BindVertexArray(0);

        SetProjection(screenWidth, screenHeight);
    }

    public void SetProjection(int width, int height)
    {
        // Orthographic projection, Y down
        // Matrix layout (column-major for OpenGL)
        float[] m = new float[16];
        m[0] = 2f / width;
        m[5] = -2f / height;
        m[10] = -1f;
        m[12] = -1f;
        m[13] = 1f;
        m[15] = 1f;
        Array.Copy(m, _projection, 16);
    }

    public void Begin()
    {
        _quadCount = 0;
        _currentTexture = null;
    }

    public void Draw(Texture texture, float x, float y, float w, float h, Color? color = null)
    {
        var c = color ?? Color.White;

        if (_currentTexture != null && _currentTexture != texture)
            Flush();

        _currentTexture = texture;

        if (_quadCount >= MaxQuads)
            Flush();

        int vi = _quadCount * 4 * 8;
        // Top-left
        _vertices[vi] = x; _vertices[vi + 1] = y;
        _vertices[vi + 2] = 0; _vertices[vi + 3] = 0;
        _vertices[vi + 4] = c.R; _vertices[vi + 5] = c.G; _vertices[vi + 6] = c.B; _vertices[vi + 7] = c.A;
        // Top-right
        _vertices[vi + 8] = x + w; _vertices[vi + 9] = y;
        _vertices[vi + 10] = 1; _vertices[vi + 11] = 0;
        _vertices[vi + 12] = c.R; _vertices[vi + 13] = c.G; _vertices[vi + 14] = c.B; _vertices[vi + 15] = c.A;
        // Bottom-right
        _vertices[vi + 16] = x + w; _vertices[vi + 17] = y + h;
        _vertices[vi + 18] = 1; _vertices[vi + 19] = 1;
        _vertices[vi + 20] = c.R; _vertices[vi + 21] = c.G; _vertices[vi + 22] = c.B; _vertices[vi + 23] = c.A;
        // Bottom-left
        _vertices[vi + 24] = x; _vertices[vi + 25] = y + h;
        _vertices[vi + 26] = 0; _vertices[vi + 27] = 1;
        _vertices[vi + 28] = c.R; _vertices[vi + 29] = c.G; _vertices[vi + 30] = c.B; _vertices[vi + 31] = c.A;

        _quadCount++;
    }

    public void Draw(Texture texture, float x, float y, float w, float h, float srcX, float srcY, float srcW, float srcH, Color? color = null)
    {
        if (_currentTexture != null && _currentTexture != texture)
            Flush();

        _currentTexture = texture;

        if (_quadCount >= MaxQuads)
            Flush();

        var c = color ?? Color.White;

        float u0 = srcX / texture.Width;
        float v0 = srcY / texture.Height;
        float u1 = (srcX + srcW) / texture.Width;
        float v1 = (srcY + srcH) / texture.Height;

        int vi = _quadCount * 4 * 8;
        _vertices[vi] = x; _vertices[vi + 1] = y;
        _vertices[vi + 2] = u0; _vertices[vi + 3] = v0;
        _vertices[vi + 4] = c.R; _vertices[vi + 5] = c.G; _vertices[vi + 6] = c.B; _vertices[vi + 7] = c.A;
        _vertices[vi + 8] = x + w; _vertices[vi + 9] = y;
        _vertices[vi + 10] = u1; _vertices[vi + 11] = v0;
        _vertices[vi + 12] = c.R; _vertices[vi + 13] = c.G; _vertices[vi + 14] = c.B; _vertices[vi + 15] = c.A;
        _vertices[vi + 16] = x + w; _vertices[vi + 17] = y + h;
        _vertices[vi + 18] = u1; _vertices[vi + 19] = v1;
        _vertices[vi + 20] = c.R; _vertices[vi + 21] = c.G; _vertices[vi + 22] = c.B; _vertices[vi + 23] = c.A;
        _vertices[vi + 24] = x; _vertices[vi + 25] = y + h;
        _vertices[vi + 26] = u0; _vertices[vi + 27] = v1;
        _vertices[vi + 28] = c.R; _vertices[vi + 29] = c.G; _vertices[vi + 30] = c.B; _vertices[vi + 31] = c.A;

        _quadCount++;
    }

    public unsafe void Flush()
    {
        if (_quadCount == 0) return;

        _shader.Use();
        _shader.SetMatrix4("uProjection", _projection);

        if (_currentTexture != null)
            _currentTexture.Bind(TextureUnit.Texture0);
        _shader.SetInt("uTexture", 0);

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (float* ptr = _vertices)
            {
                _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (nuint)(_quadCount * 4 * 8 * sizeof(float)), ptr);
            }
        }

        _gl.DrawElements(PrimitiveType.Triangles, (uint)(_quadCount * 6), DrawElementsType.UnsignedInt, null);

        _gl.BindVertexArray(0);
        _quadCount = 0;
    }

    public void End() => Flush();

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteBuffer(_ibo);
    }
}