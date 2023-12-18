using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FractalDRIVEr.Enums;

using OpenTK.Mathematics;

namespace FractalDRIVEr
{
    public class FractInfo
    {
        public FractType FractType { get; set; } = FractType.MandelbrotSet;
        public HelpFunctionType FunctionType { get; set; } = HelpFunctionType.None;
        public ConstantFlags ConstantFlags { get; set; } = ConstantFlags.Plus;
        public Vector2 Powing { get; set; } = new(2f, 0f);
        public Vector2 Constant { get; set; } = (0, 0);
        public float Barier { get; set; } = 4.0f;
        public PositionInfo PositionInfo { get; set; } = new PositionInfo();
        public ColorInfo ColorInfo { get; set; } = new ColorInfo();
    }

    public class PositionInfo
    {
        public float Scale { get; set; } = 2.3f;
        public Vector2 Delta { get; set; } = new(-1.5f, 0f);
    }

    public class ColorInfo
    {
        public bool SmoothMode { get; set; } = false;
        public int MaxIterations { get; set; } = 100;
        public ColoringType ColoringType { get; set; } = ColoringType.Default;
        public float Intensity { get; set; } = 1f;

    }
}
