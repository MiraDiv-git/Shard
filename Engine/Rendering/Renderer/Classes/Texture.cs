using Silk.NET.OpenGL;
using StbImageSharp;

namespace Engine;

public class Texture : IDisposable
{
    private readonly GL _gl;
    private uint _id;
    public int Width { get; }
    public int Height { get; }

    public Texture(string path) : this(GLContext.Current!, path) { }

    public Texture(GL gl, string path)
    {
        _gl = gl;

        using var stream = File.OpenRead(path);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        Width = image.Width;
        Height = image.Height;

        _id = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _id);

        unsafe
        {
            fixed (byte* ptr = image.Data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba,
                    (uint)image.Width, (uint)image.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            }
        }

        int linear = (int)GLEnum.Linear;
        int clamp = (int)GLEnum.ClampToEdge;
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linear);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linear);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clamp);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clamp);

        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public Texture(int width, int height, byte[] data) : this(GLContext.Current!, width, height, data) { }

    public Texture(GL gl, int width, int height, byte[] data)
    {
        _gl = gl;
        Width = width;
        Height = height;

        _id = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _id);

        unsafe
        {
            fixed (byte* ptr = data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba,
                    (uint)width, (uint)height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            }
        }

        int linear = (int)GLEnum.Linear;
        int clamp = (int)GLEnum.ClampToEdge;
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linear);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linear);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clamp);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clamp);

        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, _id);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_id);
    }
}