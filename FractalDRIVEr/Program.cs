namespace FractalDRIVEr
{
    internal class Program
    {
        private static bool forceEnd = false;
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                while (!forceEnd)
                {
                    string? text = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(text))
                        continue;
                    string[] parms = text.Split(' ');
                    switch (parms[0])
                    {
                        case "e" or "exit":
                            forceEnd = true;
                            MainWindow.Singleton.Close();
                            break;
                        case "intensity" or "i" when parms.Length > 1:
                            MainWindow.Singleton.Intensity = float.Parse(parms[1]);
                            break;
                        case "max" or "m" when parms.Length > 1:
                            MainWindow.Singleton.MaxIterations = int.Parse(parms[1]);
                            break;
                    }
                }
            });

            using MainWindow game = new(1280, 720, "FractalDRIVEr");
            game.Run();
        }
    }
}
