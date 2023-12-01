using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace FractalDRIVEr
{
    public class Shader : IDisposable
    {
        public int Handle;

        int FragmentShader;
        int VertexShader;

        public Shader(string fragmentPath, string vertexPath)
        {
            string fragmentShaderSource = File.ReadAllText(fragmentPath);
            string vertexShaderSource = File.ReadAllText(vertexPath);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);

            GL.CompileShader(FragmentShader);

#if DEBUG
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int successF);

            Console.WriteLine(successF);

            Console.WriteLine(GL.GetShaderInfoLog(FragmentShader));
#endif

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vertexShaderSource);

            GL.CompileShader(VertexShader);

#if DEBUG
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int successV);
            
            Console.WriteLine(successV);

            Console.WriteLine(GL.GetShaderInfoLog(VertexShader));
#endif

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, FragmentShader);
            GL.AttachShader(Handle, VertexShader);

            GL.LinkProgram(Handle);

#if DEBUG
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int successProgmar);

            Console.WriteLine(successProgmar);

            Console.WriteLine(GL.GetProgramInfoLog(Handle));
#endif

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (!disposedValue)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
