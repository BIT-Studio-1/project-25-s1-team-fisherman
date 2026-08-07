using System.Text;
using static Team_Fisherman.Program;
namespace Fisherman
{
    public class Fishing_Game
    {
        const string RESET = "\x1b[0m";
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
                    throw new FileNotFoundException($"File not found at {path+file}");
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
                    if (File.Exists(path + file))
                    {
                        string[] lines = File.ReadAllLines(path + file);
                        if (lines.Length > 29)
                        {
                            if (flip)
                            {
                                lines = lines[..29];
                            }
                            else
                            {
                                lines = lines[28..];
                            }
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
                                    //Console.Write(c);
                                }
                                x++;
                                x = Math.Min(x, 120);
                            }
                            y++;
                        }
                    }
                    else
                    {
                        throw new Exception($"File not found at path {path + file}");
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
                    sb.Append(letters[x, y]);
                }
                sb.Append('\n');
            }
            string text = sb.ToString();
            text = text.Replace("B", Color_Helper(32, true) + " ");
            text = text.Replace("╥", RESET + " ");
            return text.Trim('\n');
        }
    }
}
