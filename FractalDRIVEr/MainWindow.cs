using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using FractalDRIVEr.Enums;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
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
        public FractType FractType { get; set; } = FractType.MandelbrotSet;
        public HelpFunctionType FunctionType { get; set; } = HelpFunctionType.None;
        public ColoringType ColoringType { get; set; } = ColoringType.Default;
        public int SuperSampling { get; set; } = 1;

        Vector2 mouse;

        private FieldInfo keysField = typeof(KeyboardState).GetField("_keys", BindingFlags.NonPublic | BindingFlags.Instance)!;

        public BitArray DownedKeys => (BitArray)keysField.GetValue(KeyboardState)!;

        public MainWindow(int width, int height, string title) :
            base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, Vsync = VSyncMode.On })
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
            Title = Extensions.GetName(FractType, FunctionType, Powing, Constant);
            float speedModif = KeyboardState.IsKeyDown(Keys.LeftControl) ? 0.2f : (KeyboardState.IsKeyDown(Keys.LeftShift) ? 2f : 1f);
            if (KeyboardState.IsKeyDown(Keys.Q) || KeyboardState.IsKeyDown(Keys.E))
            {
                Vector2 rawDelta = Delta - new Vector2(Scale / 2) + mouse * (Scale / resolution.Y);
                Delta = Lerp(Delta, rawDelta, 0.1f);
                if (KeyboardState.IsKeyDown(Keys.Q))
                {
                    Scale /= 1.0f + (.01f * speedModif);
                }
                else
                {
                    Scale *= 1.0f + (.01f * speedModif);
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
            if (KeyboardState.IsKeyPressed(Keys.Enter))
            {
                Scale = 2.3f;
                Delta = (-1.5f, 0.0f);
                Powing = 2f;
                Constant = (0, 0);
                MaxIterations = 100;
                Intensity = 1;
                FractType = FractType.MandelbrotSet;
                FunctionType = HelpFunctionType.None;
                SuperSampling = 1;
            }
            if (KeyboardState.IsKeyPressed(Keys.Z))
            {
                Scale = 2.3f;
                Delta = (-1.5f, 0.0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.X))
            {
                Powing = 2f;
                Constant = (0, 0);
            }
            if (KeyboardState.IsKeyPressed(Keys.C))
            {
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
            if (KeyboardState.IsKeyPressed(Keys.KeyPad7) || KeyboardState.IsKeyPressed(Keys.KeyPad9))
            {
                SuperSampling += KeyboardState.IsKeyPressed(Keys.KeyPad7) ? -1 : 1;
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPad5))
            {
                Constant = (0f, 0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.B))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                ColoringType = EditEnum(ColoringType, val);
            }
            if (KeyboardState.IsKeyPressed(Keys.N))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                FunctionType = EditEnum(FunctionType, val);
            }
            if (KeyboardState.IsKeyPressed(Keys.M))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                FractType = EditEnum(FractType, val);
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
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "fractType"), (int)FractType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "functionType"), (int)FunctionType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "coloring"), (int)ColoringType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "superSampling"), SuperSampling);
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

        private T EditEnum<T>(T value, int addTo)
            where T : struct, Enum
        {
            var arr = Enum.GetValues<T>();

            int index = Array.FindIndex(arr, e => e.Equals(value));
            int newIndex = (index + addTo) % arr.Length;
            if (newIndex < 0)
            {
                return arr[arr.Length + newIndex];
            }
            return arr[newIndex];
        }
    }
}