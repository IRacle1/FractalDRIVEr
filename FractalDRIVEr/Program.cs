using System.Text.Json;

namespace FractalDRIVEr
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using MainWindow game = new(1280, 720, "FractalDRIVEr");
            game.Run();
        }
    }
}
