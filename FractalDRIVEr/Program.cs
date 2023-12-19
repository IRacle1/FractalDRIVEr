using System.Text.Json;

namespace FractalDRIVEr
{
    internal class Program
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                string? text = Console.ReadLine();
                while (!string.IsNullOrEmpty(text))
                {
                    switch (text.ToLowerInvariant())
                    {
                        case "info":
                            Console.WriteLine(JsonSerializer.Serialize(MainWindow.Singleton.ToFractalInfo(), options));
                            break;
                    }
                    text = Console.ReadLine();
                    Console.WriteLine(text);
                }
            });
            using MainWindow game = new(1280, 720, "FractalDRIVEr");
            game.Run();
        }
    }
}
