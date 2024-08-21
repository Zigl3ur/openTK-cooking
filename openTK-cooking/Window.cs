using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;

namespace openTK_cooking
{
    public class Window : GameWindow
    {

        private readonly string _windowTitle;

        // private Stopwatch _timer;
        
        private Shader _shader;
        private Shape _shape;
        private Texture _texture0;
        private Texture _texture1;

        // fps counter vars
        private int _frameCount;
        private double _timePassed;
        private double _fps;
        
        private readonly float[] _vertices =
        [
            // positions        // colors
            0.5f,  0.5f, 0.0f,  1.0f, 0.8f, 0.0f, // top right
            0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.5f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.5f, 0.0f, 1.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.5f, 0.0f, 0.5f // top left
        ];

        private readonly uint[] _indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        private readonly float[] _triVertices =
        [
            // positions         // tex coords
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            0.5f, -0.5f, 0.0f,  0.0f, 0.0f, // bottom left
            0.0f,  0.5f, 0.0f,  0.5f, 1.0f // top
        ];

        private readonly uint[] _triIndices =
        [
            0, 1, 2
        ];

        private readonly float[] _texVertices =
        [
            //Position          Texture coordinates
            0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        ];
        
        private readonly float[] _vtexVertices =
        [
            //Position          Texture coordinates
            0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        ];
        
        /// <summary>
        /// constructor to create a window
        /// </summary>
        /// <param name="width">window width</param>
        /// <param name="height">window height</param>
        /// <param name="title">window title</param>
        public Window(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        {
            _windowTitle = title;
            // UpdateFrequency = 144.0;
            VSync = VSyncMode.On;
            // _timer = new Stopwatch();
        }

        /// <summary>
        /// run on window opening
        /// </summary>
        protected override void OnLoad()
        {
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetString(StringName.Version));

            base.OnLoad();

            // _timer.Start();

            _shader = new Shader(@"..\..\..\Resources\Shaders\shader.vert", @"..\..\..\Resources\Shaders\shader.frag");

            _texture0 = new Texture(@"..\..\..\Resources\Textures\container.png");
            _texture1 = new Texture(@"..\..\..\Resources\Textures\awesomeface.png");
            
            _shader.SetInt("texture0",0);
            _shader.SetInt("texture1",1);
            
            Matrix4 trans = Matrix4.CreateScale(1.0f, 1.0f, 1.0f);
            _shader.SetMatrix4("transform", trans);
            
            _shape = new Shape(_texVertices, _indices, _shader);
            _shape.InitializeBuffers();

            GL.ClearColor(0.2f, 0.0f, 0.6f, 1.0f);
        }

        /// <summary>
        /// run on frame updated
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyPressed(Keys.Space))
            {
                // _shape = new Shape(_texVertices, _indices, _shader);
                // _texture = new Texture(@"..\..\..\Resources\Textures\texture2.png");
                // _shape.InitializeBuffers();
                // _shape.Render();

                Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(180.0f));
                Matrix4 scale = Matrix4.CreateScale(1.0f, 1.0f, 1.0f);
                Matrix4 trans = rotation * scale;
                
                _shader.SetMatrix4("transform", trans);
            }
        }

        /// <summary>
        /// run on frame rendered
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            this.FpsCounter(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _texture0.Use(TextureUnit.Texture0);
            _texture1.Use(TextureUnit.Texture1);
            
            _shader.Use();

            // used to make color change over time
            // double timeValue = _timer.Elapsed.TotalSeconds;
            // float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
            // int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            // GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 0.1f);
            
            _shape.Render();
            
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
            base.OnUnload();

            _shader.Dispose();
            _shape.Dispose();
            _texture0.Dispose();
            _texture1.Dispose();
        }

        /// <summary>
        /// fps counter and ms / frame
        /// </summary>
        /// <param name="e"></param>
        private void FpsCounter(FrameEventArgs e)
        {
            _frameCount++;
            
            _timePassed += e.Time;
            
            if (_timePassed >= 1.0)
            {
                _fps = _frameCount / _timePassed;

                double msPerFrame = 1000.0 / _fps;
                
                this.Title = $"{_windowTitle} - {_fps:F2} FPS - {msPerFrame:F2} ms";
        
                _frameCount = 0;
                _timePassed = 0.0;
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
}
