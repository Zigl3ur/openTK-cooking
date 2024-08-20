using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace openTK_cooking
{
    public class Texture
    {
        public readonly int Handle;
        private bool _disposedValue;
        
        /// <summary>
        /// create a new texture
        /// </summary>
        /// <param name="texturePath"></param>
        public Texture(string texturePath)
        {
            Handle = GL.GenTexture();
            this.Use();

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);
        
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            // texture params
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        /// <summary>
        /// use the texture
        /// </summary>
        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                GL.DeleteTexture(Handle);

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
        ~Texture()
        {
            if (!_disposedValue)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }
    }
}