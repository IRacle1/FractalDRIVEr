namespace FractalDRIVEr
{
    internal class Program
    {
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
                            Console.WriteLine($"Pow: {MainWindow.Singleton.Powing}");
                            Console.WriteLine($"ConstantFlag: {MainWindow.Singleton.ConstantFlag}");
                            Console.WriteLine($"FunctionType: {MainWindow.Singleton.FunctionType}");
                            Console.WriteLine($"FractType: {MainWindow.Singleton.FractType}");
                            Console.WriteLine($"Barier: {MainWindow.Singleton.Barier}");
                            Console.WriteLine($"Iterations: {MainWindow.Singleton.MaxIterations}");

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
