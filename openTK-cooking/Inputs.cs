using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace openTK_cooking;

public class Inputs
{
    public readonly float _speed;
    private readonly float _sensitivity = 0.2f;

    public Vector3 _position;
    public readonly Vector3 _front;
    public readonly Vector3 _up;

    private bool _firstMove;
    private Vector2 _lastPos;

    public Inputs(float speed, Vector3 position, Vector3 front, Vector3 up)
    {
        _speed = speed;
        _position = position;
        _front = front;
        _up = up;
    }

    public void ListenInputs(Window window, Camera camera, KeyboardState keyboard, MouseState mouse, bool focus)
    {
        if (!focus)
        {
            return;
        }

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }
        
        if (keyboard.IsKeyDown(Keys.Space))
        {
            camera.Position += _front * _speed;
        }

        if (keyboard.IsKeyDown(Keys.LeftShift))
        {
            camera.Position -= _front * _speed;
        }

        if (keyboard.IsKeyDown(Keys.D))
        {
            camera.Position -= Vector3.Normalize(Vector3.Cross(_front, _up)) * _speed;
        }

        if (keyboard.IsKeyDown(Keys.A))
        {
            camera.Position += Vector3.Normalize(Vector3.Cross(_front, _up)) * _speed;
            Console.WriteLine("Q");
        }

        if (keyboard.IsKeyDown(Keys.W))
        {
            camera.Position += _up * _speed;
        }

        if (keyboard.IsKeyDown(Keys.S))
        {
            camera.Position -= _up * _speed;
        }
        
        if (_firstMove) // This bool variable is initially set to true.
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            // Calculate the offset of the mouse position
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
            camera.Yaw += deltaX * _sensitivity;
            camera.Pitch -= deltaY * _sensitivity; // Reversed since y-coordinates range from bottom to top
        }
    }
}