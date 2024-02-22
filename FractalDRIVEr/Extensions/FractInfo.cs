using FractalDRIVEr.Enums;

namespace FractalDRIVEr.Extensions
{
    public class FractInfo
    {
        public Vector2Serializable Pow { get; set; } = new(2f, 0f);
        public Vector2Serializable Constant { get; set; } = new(0, 0);
        public float Barier { get; set; } = 4.0f;
        public FunctionBehaviour FunctionBehaviour { get; set; } = new();
        public PositionInfo PositionInfo { get; set; } = new PositionInfo();
        public ColorInfo ColorInfo { get; set; } = new ColorInfo();
    }

    public class FunctionBehaviour
    {
        public FractType FractType { get; set; } = FractType.MandelbrotSet;
        public FunctionType MainFunctionType { get; set; } = FunctionType.None;
        public FunctionType BeforeFunctionType { get; set; } = FunctionType.None;
        public ConstantFlag ConstantFlag { get; set; } = ConstantFlag.Plus;
    }

    public class PositionInfo
    {
        public float Scale { get; set; } = 2.3f;
        public Vector2Serializable Delta { get; set; } = new(-1.5f, 0f);
    }

    public class ColorInfo
    {
        public bool SmoothMode { get; set; } = false;
        public int MaxIterations { get; set; } = 100;
        public ColoringType ColoringType { get; set; } = ColoringType.Default;
        public float Intensity { get; set; } = 1f;
    }
}
