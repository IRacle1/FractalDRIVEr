using OpenTK.Mathematics;

namespace FractalDRIVEr.Extensions
{
    public readonly struct Vector2Serializable
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

        public float X { get; }
        public float Y { get; }

        public static Vector2Serializable From(Vector2 vector)
        {
            return new(vector.X, vector.Y);
        }

        public readonly Vector2 To()
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
