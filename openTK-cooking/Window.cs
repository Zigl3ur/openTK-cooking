using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace openTK_cooking
{
    public class Window : GameWindow
    {

        private readonly string _windowTitle;

        private Stopwatch _timer;
        
        private Shader _shader;
        private Shape _shape;
        private Texture _texture;

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
        
        private readonly float[] _VtexVertices =
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
            _timer = new Stopwatch();
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

            _texture = new Texture(@"..\..\..\Resources\Textures\texture1.png");
            
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
                _shape = new Shape(_texVertices, _indices, _shader);
                _texture = new Texture(@"..\..\..\Resources\Textures\texture3.png");
                _shape.InitializeBuffers();
                _shape.Render();
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

            _shader.Use();

            double timeValue = _timer.Elapsed.TotalSeconds;
            float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 0.1f);
            
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
            _texture.Dispose();
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
    }
}
