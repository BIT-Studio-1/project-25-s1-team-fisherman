using System.Text;
using test_fish;
using static Team_Fisherman.Program;
namespace Fisherman
{
    public class Fishing_Game
    {
        const string RESET = "\x1b[0m";
        public static Fish[] fish =
            {
                new Fish ("Carp",        10, 11, .25 ),
                new Fish ("Cat fish",    20, 173, .1 ),
                new Fish ("Bass",        15, 106, .2 ),
                new Fish ("Sturgeon",    17, 146, .15 ),
                new Fish ("boot",        1, 249, .35 ),
                new Fish ("Sardine",     5,  249, .3 )
            };
        public Fishing_Game()
        {
            Draw2();
            // make index per line
        }
        private static void Draw2()
        {
            //29 lines y
            string path = "Map/Fishing/";
            string[] files = { "waves.txt", "clouds.txt", "rain.txt", "menu.txt" };
            int frame = 500;
            bool flip = false;
            Dictionary<string, string[]> file_cache = new Dictionary<string, string[]>();
            foreach (string file in files)
            {
                if (!File.Exists(path + file))
                {
                    throw new FileNotFoundException($"File not found at {path + file}");
                }
                file_cache[file] = File.ReadAllLines(path + file);
            }
            char[,] letters = new char[121, 29];
            while (true)
            {
                Thread.Sleep(1);
                frame++;
                if (frame > 50)
                {
                    frame = 0;
                    flip = !flip;
                }
                else if (Console.KeyAvailable)
                {
                    Console.SetCursorPosition(7, 26);
                    ConsoleKey key = Console.ReadKey().Key;
                    switch (key)
                    {
                        case ConsoleKey.X:
                            Fishes();
                            break;
                        case ConsoleKey.Q:
                            return;
                    }
                    continue;
                }
                else
                {
                    continue;
                }
                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < 29; y++)
                    for (int x = 0; x < 121; x++)
                        letters[x, y] = ' ';
                foreach (string file in files)
                {
                    string[] lines = file_cache[file];
                    if (lines.Length > 29)
                    {
                        if (flip)
                            lines = lines[..29];
                        else
                            lines = lines[28..];
                    }
                    int y = 0;
                    foreach (string line in lines)
                    {
                        int x = 0;
                        foreach (char c in line)
                        {
                            if (c != ' ')
                            {
                                letters[x, y] = c;
                            }
                            x++;
                            x = Math.Min(x, 120);
                        }
                        y++;
                    }
                }
                //Console.WriteLine(sb.ToString());
                Console.WriteLine(ToText(letters));
                //Console.ReadLine();   
            }
        }
        private static string ToText(char[,] letters)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 120; x++)
                {
                    char c = letters[x, y];
                    if (c == 'B')
                        sb.Append(Color_Helper(38, true) + " ");
                    else if (c == '╥')
                        sb.Append(RESET + " ");
                    else
                        sb.Append(c);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
        public struct Fish
        {
            public string name;
            public int coins;
            public int color;
            public double weight;
            public Fish(string Name, int Coins, int Color, double Weight)
            {
                this.name = Name;
                this.coins = Coins;
                this.color = Color;
                this.weight = Weight;
            }

        }
        private static void Fishes()
        {
            double normazing_weight = fish.Sum(n => n.weight);
            Random rand = new Random();
            int c = 0;
            foreach (Fish item  in fish)
            {
                fish[c].weight = item.weight / normazing_weight;


                c++;
            }
            



        }
    }
}
