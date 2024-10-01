using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace openTK_cooking
{
    public class Window : GameWindow
    {

        // window vars
        private readonly string _windowTitle;
        private readonly int _width;
        private readonly int _height;

        private double _time;

        // Matrixs vars
        private Matrix4 _model;
        private Matrix4 _view;
        private Matrix4 _projection;

        // camera / contreols vars
        private Camera _camera;
        private Inputs _inputs;

        // render object vars
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
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        ];

        private readonly uint[] _indices =
        [
            0, 1, 3,
            1, 2, 3
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
            _width = width;
            _height = height;
            // UpdateFrequency = 144.0;
            VSync = VSyncMode.On;

            CursorState = CursorState.Grabbed;
        }

        /// <summary>
        /// run on window opening
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();

            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetString(StringName.Version));

            GL.Enable(EnableCap.DepthTest);

            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), _width / _height, 0.1f, 100.0f);

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            _inputs = new Inputs(0.1f, _camera.Position, _camera.Up, _camera.Front);

            _shader = new Shader(@"..\..\..\Resources\Shaders\shader.vert", @"..\..\..\Resources\Shaders\shader.frag");

            _texture0 = new Texture(@"..\..\..\Resources\Textures\container.png");
            _texture1 = new Texture(@"..\..\..\Resources\Textures\container.png");

            _shader.SetInt("texture0", 0);

            _shape = new Shape(_vertices, _indices, _shader);

            _shape.InitializeBuffers();

            GL.ClearColor(0.2f, 0.0f, 0.6f, 1.0f);
        }

        /// <summary>
        /// run on frame updated
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _inputs.ListenInputs(this, _camera, KeyboardState, MouseState, IsFocused);
        }

        /// <summary>
        /// run on frame rendered
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _time += 25.0 * e.Time;

            Console.WriteLine(_camera.Position);

            this.FpsCounter(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _texture0.Use(TextureUnit.Texture0);

            _model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time)) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));

            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _shader.Use();

            _shape.Render();

            _texture1.Use(TextureUnit.Texture0);

            Matrix4 model2 = Matrix4.Identity * Matrix4.CreateTranslation(new Vector3(1.5f, 0.0f, 0.0f)) * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time)) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(-_time));
            _shader.SetMatrix4("model", model2);
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
