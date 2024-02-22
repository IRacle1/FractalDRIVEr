using System.Collections;
using System.Diagnostics;
using System.Reflection;

using FractalDRIVEr.Enums;
using FractalDRIVEr.Extensions;
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

        public float Intensity { get; set; } = 1f;
        public int MaxIterations { get; set; } = 100;
        public bool SmoothMode { get; set; } = false;
        public ColoringType ColoringType { get; set; } = ColoringType.Default;

        public Vector2 Delta = (-1.5f, 0f);
        public float Scale { get; set; } = 2.3f;

        public Vector2 Pow = (2f, 0f);
        public Vector2 Constant = (0, 0);

        public FractType FractType 
        { 
            get
            {
                return (FractType)Behaviour[0];
            }
            set
            {
                Behaviour[0] = (int)value;
            }
        }

        public FunctionType MainFunctionType
        {
            get
            {
                return (FunctionType)Behaviour[01];
            }
            set
            {
                Behaviour[1] = (int)value;
            }
        }

        public FunctionType BeforeFunctionType
        {
            get
            {
                return (FunctionType)Behaviour[2];
            }
            set
            {
                Behaviour[2] = (int)value;
            }
        }

        public ConstantFlag ConstantFlag
        {
            get
            {
                return (ConstantFlag)Behaviour[3];
            }
            set
            {
                Behaviour[3] = (int)value;
            }
        }

        private int[] OldBehaviour = new int[4]
        {
            (int)FractType.MandelbrotSet,
            (int)FunctionType.None,
            (int)FunctionType.None,
            (int)ConstantFlag.Plus,
        };

        private int[] Behaviour = new int[4]
        {
            (int)FractType.MandelbrotSet,
            (int)FunctionType.None,
            (int)FunctionType.None,
            (int)ConstantFlag.Plus,
        };

        private float PeriodPersent { get; set; } = 0f;

        public float Barier { get; set; } = 4.0f;

        Vector2 mouse;

        private readonly FieldInfo keysField = typeof(KeyboardState).GetField("_keys", BindingFlags.NonPublic | BindingFlags.Instance)!;

        public BitArray DownedKeys => (BitArray)keysField.GetValue(KeyboardState)!;

        public MainWindow(int width, int height, string title) :
            base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title, Vsync = VSyncMode.On })
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
            if (KeyboardState.IsKeyPressed(Keys.F11))
            {
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }

            float speedModif = KeyboardState.IsKeyDown(Keys.LeftControl) ? 0.2f : (KeyboardState.IsKeyDown(Keys.LeftShift) ? 2f : 1f);
            
            if (KeyboardState.IsKeyDown(Keys.Q) || KeyboardState.IsKeyDown(Keys.E))
            {
                Vector2 rawDelta = Delta - new Vector2(Scale / 2) + mouse * (Scale / resolution.Y);
                Delta = MainExtensions.Lerp(Delta, rawDelta, 0.1f);

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
                Pow = (2f, 0f);
                Constant = (0, 0);
                MaxIterations = 100;
                Intensity = 1;
                FractType = FractType.MandelbrotSet;
                MainFunctionType = FunctionType.None;
                BeforeFunctionType = FunctionType.None;
                SmoothMode = false;
                Barier = 4.0f;
                ConstantFlag = ConstantFlag.Plus;
            }
            if (KeyboardState.IsKeyPressed(Keys.Z))
            {
                Scale = 2.3f;
                Delta = (-1.5f, 0.0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.X))
            {
                Pow = new(2f, 0f);
            }
            if (KeyboardState.IsKeyPressed(Keys.C))
            {
                MaxIterations = 100;
                Intensity = 1;
                ColoringType = ColoringType.Default;
                SmoothMode = false;
                Barier = 4.0f;
            }
            if (KeyboardState.IsKeyDown(Keys.Left))
            {
                Pow += -Vector2.UnitX * 0.01f * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.Right))
            {
                Pow += Vector2.UnitX * 0.01f * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.Up))
            {
                Pow += Vector2.UnitY * 0.01f * speedModif;
            }
            if (KeyboardState.IsKeyDown(Keys.Down))
            {
                Pow += -Vector2.UnitY * 0.01f * speedModif;
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
                ColoringType = ColoringType.EditEnum(val);
            }
            if (KeyboardState.IsKeyPressed(Keys.N))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                MainFunctionType = MainFunctionType.EditEnum(val);
            }
            if (KeyboardState.IsKeyPressed(Keys.M))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                BeforeFunctionType = BeforeFunctionType.EditEnum(val);
            }
            if (KeyboardState.IsKeyPressed(Keys.Comma))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                FractType = FractType.EditEnum(val);
            }
            if (KeyboardState.IsKeyPressed(Keys.Period))
            {
                for (int i = 0; i < 4; i++)
                {
                    OldBehaviour[i] = Behaviour[i];
                    Console.WriteLine(OldBehaviour[i]);
                }
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPadAdd))
            {
                PeriodPersent = MathF.Min(1.0f, PeriodPersent + 0.05f * speedModif);
            }
            if (KeyboardState.IsKeyDown(Keys.KeyPadSubtract))
            {
                PeriodPersent = MathF.Max(0.0f, PeriodPersent - 0.05f * speedModif);
            }
            if (KeyboardState.IsKeyPressed(Keys.R))
            {
                int val = KeyboardState.IsKeyDown(Keys.LeftShift) ? -1 : 1;
                ConstantFlag = ConstantFlag.EditEnum(val);
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            UpdateUniforms();

            base.OnUpdateFrame(e);
        }

        private void UpdateUniforms()
        {
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(MaxIterations)), MaxIterations);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, nameof(resolution)), ref resolution);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(Scale)), Scale);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, nameof(Delta)), ref Delta);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, nameof(Constant)), ref Constant);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(Intensity)), Intensity);
            GL.Uniform2(GL.GetUniformLocation(shader.Handle, nameof(Pow)), Pow);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(Behaviour)), Behaviour.Length, Behaviour);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(OldBehaviour)), OldBehaviour.Length, OldBehaviour);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(ColoringType)), (int)ColoringType);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(SmoothMode)), SmoothMode ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(Barier)), Barier);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, nameof(PeriodPersent)), PeriodPersent);

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

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouse = (e.Position.X, resolution.Y - e.Position.Y);
            base.OnMouseMove(e);
        }
        public FractInfo ToFractalInfo(bool keepPosition = false, bool keepColor = false)
        {
            var value = new FractInfo
            {
                FunctionBehaviour = new FunctionBehaviour
                {
                    BeforeFunctionType = FunctionType.None,
                    MainFunctionType = MainFunctionType,
                    ConstantFlag = ConstantFlag,
                    FractType = FractType,
                },
                Barier = Barier,
                Constant = Constant,
                Pow = Pow
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

        public void FromFractalInfo(FractInfo info)
        {
            Scale = info.PositionInfo.Scale;
            Delta = info.PositionInfo.Delta;
            Pow = info.Pow;
            Constant = info.Constant;
            MaxIterations = info.ColorInfo.MaxIterations;
            Intensity = info.ColorInfo.Intensity;
            FractType = info.FunctionBehaviour.FractType;
            MainFunctionType = info.FunctionBehaviour.MainFunctionType;
            ConstantFlag = info.FunctionBehaviour.ConstantFlag;
            SmoothMode = info.ColorInfo.SmoothMode;
            Barier = info.Barier;
        }
    }
}