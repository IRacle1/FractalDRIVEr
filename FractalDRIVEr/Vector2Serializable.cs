using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;

using OpenTK.Mathematics;

namespace FractalDRIVEr
{
    public struct Vector2Serializable
    {
        public Vector2Serializable()
        {
            
        }

        public Vector2Serializable(float x)
        {
            X = Y = x;
        }

        public Vector2Serializable(float x, float y)
        {
            X = x; 
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public static Vector2Serializable From(Vector2 vector)
        {
            return new(vector.X, vector.Y);
        }

        public Vector2 To()
        {
            return new(X, Y);
        }

        public static implicit operator Vector2Serializable(Vector2 v)
        {
            return new(v.X, v.Y);
        }

        public static implicit operator Vector2(Vector2Serializable v)
        {
            return v.To();
        }
    }
}
