using System.Reflection;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FractalDRIVEr.Extensions;

public static class MainExtensions
{
    private static readonly List<MethodInfo> methods = typeof(GL).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name.StartsWith("Uniform")).ToList();
    private static readonly Dictionary<Type, MethodInfo> filledMethods = new();

    public static Vector2 Lerp(Vector2 firstFloat, Vector2 secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }

    public static T EditEnum<T>(this T value, int addTo)
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

    public static void UpdateUniform<T>(int program, T value, string name)
        where T : notnull
    {
        int location = GL.GetUniformLocation(program, name);
        Type type = value.GetType() switch
        {
            Type boolType when boolType == typeof(bool) => typeof(int),
            Type { IsEnum: true } enumType => typeof(int),
            Type normal => normal,
        };

        if (!filledMethods.TryGetValue(type, out MethodInfo? target))
        {
            List<MethodInfo> founded = methods.Where(m => {
                var parameters = m.GetParameters();
                return parameters.Length == 2 && parameters[1].ParameterType == type;
            }).ToList();

            filledMethods[type] = target = founded[0];
        }

        target.Invoke(null, new object[] { location, value });
    }
}
