using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            char[,] letters = new char[121, 29];
            bool firsst = true;
            foreach (string file in files)
            {
                if (File.Exists(path + file))
                {
                    string[] lines = File.ReadAllLines(path + file);
                    if (lines.Length > 29)
                    {

                        lines = lines[..29];
                    }

                    int y = 0;
                    foreach (string line in lines)
                    {
                        int x = 0;
                        foreach (char c in line)
                        {
                            if (c != ' ' || letters[x, y] != '\0')
                            {
                                letters[x, y] = c;
                                Console.Write(c);

                            }
                            x++;
                            x =Math.Min(x, 120);
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




            Console.ReadLine();

        }


        private static void Draw()
        {
            int frame = 0;
            string[] waves = new string[0];
            if (File.Exists("Map/Fishing/waves.txt"))
            {
                waves = File.ReadAllLines("Map/Fishing/waves.txt");
            }
            else
            {
                throw new Exception("File not found");
            }
            string[] clouds = new string[0];
            if (File.Exists("Map/Fishing/clouds.txt"))
            {
                clouds = File.ReadAllLines("Map/Fishing/clouds.txt");
            }
            else
            {
                throw new Exception("File not found");
            }
            string[] rain = new string[0];
            if (File.Exists("Map/Fishing/rain.txt"))
            {
                rain = File.ReadAllLines("Map/Fishing/rain.txt");
            }
            else
            {
                throw new Exception("File not found");
            }
            string[] menu = new string[0];
            if (File.Exists("Map/Fishing/menu.txt"))
            {
                menu = File.ReadAllLines("Map/Fishing/menu.txt");
            }
            else
            {
                throw new Exception("File not found");
            }

            char[] special = { '╥', 'B' };

            StringBuilder buffer = new StringBuilder();
            while (true)
            {
                //Console.Clear();
                //Console.Write("\x1b[3j");
                Console.SetCursorPosition(0, 0);
                buffer.Clear();
                //buffer.Append(Color_Helper(32,true));
                if (frame % 2 == 0)
                {
                    foreach (string wave in waves[..(waves.Length / 2)])
                    {
                        buffer.Append(wave);
                    }
                    buffer.Append('\n');
                }
                else
                {
                    foreach (string wave in waves[(waves.Length / 2)..])
                    {
                        buffer.Append(wave);
                    }
                    buffer.Append('\n');
                }
                int index = 0;
                foreach (string cloud in clouds)
                {
                    foreach (char c in cloud)
                    {
                        if (special.Contains(c))
                        {
                            buffer.Insert(index, c);
                        }
                        else if (c == ' ')
                        {
                        }
                        else
                        {
                            buffer[index] = c;
                        }
                        index++;
                    }
                }
                index = 0;
                string[] rain_change;
                if (frame % 2 == 0)
                {
                    rain_change = rain[..(rain.Length / 2)];
                }
                else
                {
                    rain_change = rain[(rain.Length / 2)..];
                }
                foreach (string cloud in rain_change)
                {
                    foreach (char c in cloud)
                    {
                        if (special.Contains(c))
                        {
                            buffer.Insert(index, c);
                        }
                        else if (c == ' ')
                        {
                        }
                        else
                        {
                            buffer[index] = c;
                        }
                        index++;
                    }
                }
                index = 0;
                foreach (string line in menu)
                {
                    foreach (char c in line)
                    {
                        if (special.Contains(c))
                        {
                            buffer.Insert(index, c);
                        }
                        if (c == ' ')
                        {

                        }
                        else
                        { buffer[index] = c; }
                        index++;
                    }
                }





                string text = buffer.ToString();

                text = text.Replace("B", Color_Helper(32, true));
                text = text.Replace("╥", RESET);


                Console.Write(text);




                //Thread.Sleep(500);

                Console.ReadLine();



                frame++;
            }
        }



    }
}
