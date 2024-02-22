using System.Text.Json;

namespace FractalDRIVEr
{
    internal class Program
    {
        private static JsonSerializerOptions options = new()
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
                            Console.WriteLine($"Constant: {MainWindow.Singleton.Constant}");
                            Console.WriteLine($"Pow: {MainWindow.Singleton.Pow}");
                            Console.WriteLine($"ConstantFlag: {MainWindow.Singleton.ConstantFlag}");
                            Console.WriteLine($"FunctionType: {MainWindow.Singleton.MainFunctionType}");
                            Console.WriteLine($"FractType: {MainWindow.Singleton.FractType}");
                            Console.WriteLine($"Barier: {MainWindow.Singleton.Barier}");
                            Console.WriteLine($"Iterations: {MainWindow.Singleton.MaxIterations}");
                            break;
                        case "jsoninfo":
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
