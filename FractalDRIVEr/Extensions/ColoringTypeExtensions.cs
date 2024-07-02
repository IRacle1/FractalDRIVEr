using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FractalDRIVEr.Enums;

using OpenTK.Mathematics;

namespace FractalDRIVEr.Extensions
{
    public static class ColoringTypeExtensions
    {
        public static Color4 ToColor4(this ColoringType coloringType) => coloringType switch
        {
            ColoringType.Black => new(1f, 1f, 1f, 1f),
            ColoringType.Default => new(0.5f, 0.5f, 0.5f, 1f),
            ColoringType.Pink => new(1f, 0.5f, 0.5f, 1f),
            ColoringType.Green => new(0.5f, 1f, 1f, 1f),
            ColoringType.Blue => new(0.5f, 0.5f, 1f, 1f),
            ColoringType.Aqua => new(0.5f, 1f, 1f, 1f),
            ColoringType.Lime => new(1f, 1f, 1f, 1f),
            ColoringType.Purple => new(1f, 0.5f, 0.5f, 1f),
            _ => new(1f, 1f, 1f, 1f),
        };
    }
}
