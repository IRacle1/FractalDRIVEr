using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FractalDRIVEr.Enums;

using OpenTK.Mathematics;

namespace FractalDRIVEr
{
    public static class Extensions
    {
        public static string GetName(FractType fractType, HelpFunctionType functionType, float powing, Vector2 c)
        {
            string raw = "((z)^{1}){0}";
            if (fractType == FractType.MandelbrotSet)
            {
                raw += " + c";
            }
            if (functionType != HelpFunctionType.None)
            {
                raw = functionType.ToString() + raw;
            }

            string vecString = string.Empty;
            if (c != Vector2.Zero)
            {
                vecString = $" + {MathF.Round(c.X, 2)} + {MathF.Round(c.Y, 2)}i";
            }

            return string.Format(raw, vecString, MathF.Round(powing, 2));
        }
    }
}
