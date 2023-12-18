using System.Collections;
using System.Diagnostics;
using System.Reflection;

using FractalDRIVEr.Enums;

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

        public Vector2 resolution;
        public Vector3 color = new(1, 1, 1);

        public float Intensity { get; set; } = 1f;
        public int MaxIterations { get; set; } = 100;
        public float Scale { get; set; } = 2.3f;
        public Vector2 Delta = new(-1.5f, 0f);
        public Vector2 Powing = new(2f, 0f);
        public Vector2 Constant = (0, 0);
        public FractType FractType { get; set; } = FractType.MandelbrotSet;
        public HelpFunctionType FunctionType { get; set; } = HelpFunctionType.None;
        public ConstantFlags ConstantFlags { get; set; } = ConstantFlags.Plus;
        public ColoringType ColoringType { get; set; } = ColoringType.Default;
        public bool SmoothMode { get; set; } = false;
        public float Barier { get; set; } = 4.0f;

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
            //Title = Extensions.GetName(FractType, FunctionType, Powing, Constant);
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
            if (KeyboardState.IsKeyDown(Keys.L)) 
            {
                Constant = Delta - new Vector2(Scale / 2) + mouse * (Scale / resolution.Y);
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
                Powing = (2f, 0f);
                Constant = (0, 0);
                MaxIterations = 100;
                Intensity = 1;
                FractType = FractType.MandelbrotSet;
                FunctionType = HelpFunctionType.None;
                SmoothMode = false;
                Barier = 4.0f;
                ConstantFlags = ConstantFlags.Plus;
            }
            if (KeyboardState.IsKeyPressed(Keys.Z))
            {
                Scale = 2.3f;
                Delta = (-1.5f, 0.0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.X))
            {
                Powing = new(2f, 0f);
                Constant = (0, 0);
            }
            if (KeyboardState.IsKeyPressed(Keys.C))
            {
                MaxIterations = 100;
                Intensity = 1;
                ColoringType = ColoringType.Default;
                SmoothMode = false;
                Barier = 4.0f;
            }
            if (KeyboardState.IsKeyDown(Keys.Left) || KeyboardState.IsKeyDown(Keys.Right))
            {
                Powing += new Vector2((KeyboardState.IsKeyDown(Keys.Left) ? -0.01f : 0.01f) * speedModif, 0f);
            }
            if (KeyboardState.IsKeyDown(Keys.Up) || KeyboardState.IsKeyDown(Keys.Down))
            {
                Powing += new Vector2(0f, (KeyboardState.IsKeyDown(Keys.Down) ? -0.01f : 0.01f) * speedModif);
            }
            if (KeyboardState.IsKeyPressed(Keys.KeyPad1) || KeyboardState.IsKeyPressed(Keys.KeyPad7))
            {
                if (KeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    Intensity += KeyboardState.IsKeyPressed(Keys.KeyPad1) ? -1 : 1;
                }
                else
                {
                    MaxIterations += KeyboardState.IsKeyPressed(Keys.KeyPad1) ? -10 : 10;
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
            if (KeyboardState.IsKeyDown(Keys.KeyPad3) || KeyboardState.IsKeyDown(Keys.KeyPad9))
            {
                Barier += (KeyboardState.IsKeyDown(Keys.KeyPad3) ? -0.2f : 0.2f) * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPad5))
            {
                Constant = (0f, 0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.V))
            {
                SmoothMode = !SmoothMode;
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
            if (KeyboardState.IsKeyPressed(Keys.R))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                ConstantFlags = EditEnum(ConstantFlags, val);
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            color.X = (float)Math.Abs(Math.Sin(Stopwatch.Elapsed.TotalSeconds));
            color.Y = (float)Math.Abs(Math.Cos(Stopwatch.Elapsed.TotalSeconds));
            color.Z = (float)Math.Abs(Math.Sin(Stopwatch.Elapsed.TotalSeconds + 0.25));

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
            GL.Uniform3(GL.GetUniformLocation(shader.Handle, "color"), ref color);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, "powing"), Powing);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "fractType"), (int)FractType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "functionType"), (int)FunctionType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "coloring"), (int)ColoringType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "smoothMode"), SmoothMode ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "barier"), Barier);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "constantFlags"), (int)ConstantFlags);

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

        private FractInfo ToFractalInfo(bool keepPosition = false, bool keepColor = false)
        {
            var value = new FractInfo
            {
                FractType = FractType,
                Barier = Barier,
                Constant = Constant,
                ConstantFlags = ConstantFlags,
                FunctionType = FunctionType,
                Powing = Powing
            };

            if (keepPosition)
            {
                value.PositionInfo = new PositionInfo
                {
                    Delta = Delta,
                    Scale = Scale,
                };
            }
            if (keepColor)
            {
                value.ColorInfo = new ColorInfo
                {
                    ColoringType = ColoringType,
                    Intensity = Intensity,
                    MaxIterations = MaxIterations,
                    SmoothMode = SmoothMode,
                };
            }

            return value;
        }

        private void FromFractalInfo(FractInfo info)
        {
            Scale = info.PositionInfo.Scale;
            Delta = info.PositionInfo.Delta;
            Powing = info.Powing;
            Constant = info.Constant;
            MaxIterations = info.ColorInfo.MaxIterations;
            Intensity = info.ColorInfo.Intensity;
            FractType = info.FractType;
            FunctionType = info.FunctionType;
            SmoothMode = info.ColorInfo.SmoothMode;
            Barier = info.Barier;
            ConstantFlags = info.ConstantFlags;
        }
    }
}