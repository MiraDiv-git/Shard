using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Engine;

public class Shader : IDisposable
{
    private readonly GL _gl;
    private uint _program;

    public Shader(string vertexSource, string fragmentSource) : this(GLContext.Current!, vertexSource, fragmentSource) { }

    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;

        uint vs = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fs = CompileShader(ShaderType.FragmentShader, fragmentSource);

        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vs);
        _gl.AttachShader(_program, fs);
        _gl.LinkProgram(_program);

        int linkStatus = _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus);
        if (linkStatus == 0)
        {
            string log = _gl.GetProgramInfoLog(_program);
            _gl.DeleteShader(vs);
            _gl.DeleteShader(fs);
            _gl.DeleteProgram(_program);
            throw new EngineException($"Shader link failed: {log}");
        }

        _gl.DeleteShader(vs);
        _gl.DeleteShader(fs);
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        int status = _gl.GetShader(shader, ShaderParameterName.CompileStatus);
        if (status == 0)
        {
            string log = _gl.GetShaderInfoLog(shader);
            _gl.DeleteShader(shader);
            throw new EngineException($"Shader compile failed ({type}): {log}");
        }

        return shader;
    }

    public void Use() => _gl.UseProgram(_program);

    public int GetUniformLocation(string name) => _gl.GetUniformLocation(_program, name);

    public void SetInt(string name, int value) => _gl.Uniform1(GetUniformLocation(name), value);
    public void SetFloat(string name, float value) => _gl.Uniform1(GetUniformLocation(name), value);

    public void SetMatrix4(string name, float[] matrix)
    {
        int loc = GetUniformLocation(name);
        _gl.UniformMatrix4(loc, 1, false, matrix);
    }

    public void SetVector4(string name, float x, float y, float z, float w)
        => _gl.Uniform4(GetUniformLocation(name), x, y, z, w);

    public void Dispose()
    {
        _gl.DeleteProgram(_program);
    }
}