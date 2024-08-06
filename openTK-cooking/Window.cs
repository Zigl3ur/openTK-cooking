using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace openTK_cooking;

public class Window : GameWindow
{
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private Shader _shader;
    
    readonly float[] _vertices =
    [
        -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        0.5f, -0.5f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f  //Top vertex
    ];
    
    /// <summary>
    /// constructor to create a window
    /// </summary>
    /// <param name="width">window width</param>
    /// <param name="height">window height</param>
    /// <param name="title">window title</param>
    public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    { }
    
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    /// <summary>
    /// run on window open
    /// </summary>
    protected override void OnLoad()
    {
        base.OnLoad();

        _shader = new Shader(@"..\..\..\Resources\Shaders\shader.vert", @"..\..\..\Resources\Shaders\shader.frag");
        
        // VBO
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
        // VAO
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.ClearColor(0.2f, 0.6f, 0.1f, 1.0f);
    }
    
    /// <summary>
    ///  run on frame rendered
    /// </summary>
    /// <param name="e"></param>
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _shader.Use();
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        SwapBuffers();
    }
    
    /// <summary>
    /// run on window resize
    /// </summary>
    /// <param name="e"></param>
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    /// <summary>
    /// run on window closing
    /// </summary>
    protected override void OnUnload()
    {
        _shader.Dispose();
    }
}
    