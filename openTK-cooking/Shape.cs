using OpenTK.Graphics.OpenGL4;

namespace openTK_cooking
{
    public class Shape
    {
        // render vars
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
    
        // data vars
        private readonly float[] _vertices;
        private readonly uint[] _indices;
        
        private bool _disposedValue;
        
        public Shape(float[] vertices, uint[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }
    
        /// <summary>
        ///  bind and init all objects
        /// </summary>
        public void InitializeBuffers()
        {
            // bind VBO, VAO and EBO
    
            // VBO
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
    
            // VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
        
            // vertex attribs
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
    
            // EBO
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
    
            // Unbind VAO
            GL.BindVertexArray(0);
        }
    
        /// <summary>
        /// Render the shape
        /// </summary>
        public void Render()
        {
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                GL.DeleteBuffer(_vertexBufferObject);
                GL.DeleteBuffer(_elementBufferObject);
                GL.DeleteVertexArray(_vertexArrayObject);

                _disposedValue = true;
            }
        }
        
        /// <summary>
        /// free all used resources from the shape
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// destructor
        /// </summary>
        ~Shape()
        {
            if (_disposedValue)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }
    }
}