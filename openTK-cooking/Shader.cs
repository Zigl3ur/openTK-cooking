using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace openTK_cooking;

public class Shader
{
    // _handle is the result of the both shaders compiled
    // _uniformLocations is where shaders are
    public readonly int Handle;
    private readonly Dictionary<string, int> _uniformLocations;
    private bool _disposedValue;

    public Shader(string vertexPath, string fragmentPath)
    {

        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        // compile vertex shader and log
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int successVertexShader);
        if (successVertexShader == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(infoLog);
        }
        
        // compile fragment shader and log
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int successFragmentShader);
        if (successFragmentShader == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(infoLog);
        }

        // link both shaders together
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int shadersLinkSuccess);
        if (shadersLinkSuccess == 0)
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }
        
        // clean up
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
        // get number of active uniforms in the shader
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);
        
        _uniformLocations = new Dictionary<string, int>();

        for (int i = 0; i < numberOfUniforms; i++)
        {
            string key = GL.GetActiveUniform(Handle, i, out _, out _);
            int location = GL.GetUniformLocation(Handle, key);
            _uniformLocations.Add(key, location);
        }
    }

    /// <summary>
    /// get shader var location
    /// </summary>
    /// <param name="attribName"> var name in the shader</param>
    /// <returns></returns>
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }
    
    /// <summary>
    /// use the shader
    /// </summary>
    public void Use()
    {
        GL.UseProgram(Handle);
    }

    // set up shaders uniforms
    public void SetInt(string name, int data)
    {
        GL.UseProgram(Handle);
        GL.Uniform1(_uniformLocations[name], data);
    }

    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(Handle);

            _disposedValue = true;
        }
    }

    /// <summary>
    /// clean it
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// destructor
    /// </summary>
    ~Shader()
    {
        if (!_disposedValue)
        {
            Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }
    
    private void CheckError(string msg)
    {
        ErrorCode errorCode = GL.GetError();
        if (errorCode != ErrorCode.NoError)
        {
            Console.WriteLine($"{msg} => {errorCode}");
        }
    }
}