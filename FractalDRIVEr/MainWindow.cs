using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FractalDRIVEr
{
    internal class MainWindow : GameWindow
    {
        public Stopwatch Stopwatch { get; } = new Stopwatch();
        public static MainWindow Singleton { get; private set; } = null!;

        Shader shader = null!;

        int vao;

        public float Intensity { get; set; } = 1f;
        public int MaxIterations { get; set; } = 100;
        public float Scale { get; set; } = 2.3f;
        public Vector2 Delta = new(-1.5f, 0f);
        public Vector3 Color = new(1, 1, 1);
        public Vector2 resolution;
        public float Powing { get; set; } = 2f;
        public Vector2 Constant = (0, 0);
        public bool IsMandelbrot { get; set; } = true;

        Vector2 mouse;

        private FieldInfo keysField = typeof(KeyboardState).GetField("_keys", BindingFlags.NonPublic | BindingFlags.Instance)!;

        public BitArray DownedKeys => (BitArray)keysField.GetValue(KeyboardState)!;

        public MainWindow(int width, int height, string title) : 
            base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, Vsync = VSyncMode.On, WindowState = WindowState.Fullscreen })
        {
            resolution = new Vector2(width, height);
            Singleton = this;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            shader = new Shader(@"shader.frag", @"vertex.vert");

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            Stopwatch.Start();
        }

        protected override void OnUnload()
        {
            shader.Dispose();
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            float speedModif = KeyboardState.IsKeyDown(Keys.LeftShift) ? 2f : 1f;
            if (KeyboardState.IsKeyDown(Keys.Q) || KeyboardState.IsKeyDown(Keys.E))
            {
                Vector2 rawDelta = Delta - new Vector2(Scale / 2) + mouse * (Scale / resolution.Y);
                Delta = Lerp(Delta, rawDelta, 0.1f);
                if (KeyboardState.IsKeyDown(Keys.Q))
                {
                    Scale /= 1.01f;
                }
                else
                {
                    Scale *= 1.01f;
                }
            }
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                Delta += Vector2.UnitY * (Scale * 0.01f) * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                Delta += -Vector2.UnitX * (Scale * 0.01f) * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                Delta += -Vector2.UnitY * (Scale * 0.01f) * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                Delta += Vector2.UnitX * (Scale * 0.01f) * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.Enter))
            {
                Scale = 2.3f;
                Delta = (-1.5f, 0.0f);
                Powing = 2f;
                Constant = (0, 0);
                MaxIterations = 100;
                Intensity = 1;
            }
            if (KeyboardState.IsKeyDown(Keys.Left) || KeyboardState.IsKeyDown(Keys.Right))
            {
                Powing += (KeyboardState.IsKeyDown(Keys.Left) ? -0.01f : 0.01f) * speedModif;
            }
            if (KeyboardState.IsKeyPressed(Keys.Up) || KeyboardState.IsKeyPressed(Keys.Down))
            {
                if (KeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    Intensity += KeyboardState.IsKeyPressed(Keys.Down) ? -1 : 1;
                }
                else
                {
                    MaxIterations += KeyboardState.IsKeyPressed(Keys.Down) ? -10 : 10;
                }
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPad4) || KeyboardState.IsKeyDown(Keys.KeyPad6))
            {
                Constant += ((KeyboardState.IsKeyDown(Keys.KeyPad4) ? -0.006f : 0.006f) * speedModif, 0f);
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPad2) || KeyboardState.IsKeyDown(Keys.KeyPad8))
            {
                Constant += (0f, (KeyboardState.IsKeyDown(Keys.KeyPad2) ? -0.006f : 0.006f) * speedModif);
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPad5))
            {
                Constant = (0f, 0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.M))
            {
                IsMandelbrot = !IsMandelbrot;
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            Color.X = (float)Math.Abs(Math.Sin(Stopwatch.Elapsed.TotalSeconds));
            Color.Y = (float)Math.Abs(Math.Cos(Stopwatch.Elapsed.TotalSeconds));
            Color.Z = (float)Math.Abs(Math.Sin(Stopwatch.Elapsed.TotalSeconds + 0.25));

            UpdateUniforms();

            base.OnUpdateFrame(e);
        }

        private void UpdateUniforms()
        {
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "maxIterations"), MaxIterations);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, "resolution"), ref resolution);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "scale"), Scale);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, "delta"), ref Delta);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, "constant"), ref Constant);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "intensity"), Intensity);
            GL.Uniform3(GL.GetUniformLocation(shader.Handle, "color"), ref Color);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "powing"), Powing);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "isMandelbrot"), IsMandelbrot ? 1 : 0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            shader.Use();

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            resolution = (e.Width, e.Height);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.OffsetY > 0)
            {
                Scale /= 1.01f;
            }
            else
            {
                Scale *= 1.01f;
            }
            base.OnMouseWheel(e);
        }

        public static Vector2 Lerp(Vector2 firstFloat, Vector2 secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouse = (e.Position.X, resolution.Y - e.Position.Y);
            base.OnMouseMove(e);
        }

        private void SaveScreenshot(string filename, int windowWidth, int windowHeight)
        {
            var pixels = new byte[windowWidth * windowHeight * 3];

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.ReadBuffer(ReadBufferMode.Front);
            GL.ReadPixels(0, 0, windowWidth, windowHeight, PixelFormat.Bgr, PixelType.UnsignedByte, pixels);

            var width = BitConverter.GetBytes((short)windowWidth);
            var height = BitConverter.GetBytes((short)windowHeight);
            byte[] header = { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, width[0], width[1], height[0], height[1], 24, 0b00001000 };

            using MemoryStream ms = new();
            ms.Write(header, 0, header.Length);
            ms.Write(pixels, 0, pixels.Length);

            ms.Seek(0, SeekOrigin.Begin);

            using var image = Aspose.Imaging.Image.Load(ms);
            // Create an instance of PngOptions
            var exportOptions = new Aspose.Imaging.ImageOptions.PngOptions() { ColorType = Aspose.Imaging.FileFormats.Png.PngColorType.TruecolorWithAlpha };

            // Save tga to png
            image.Save(Path.Combine(filename) + ".png", exportOptions);
        }
    }
}