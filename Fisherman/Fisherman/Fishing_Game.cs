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

        public Fishing_Game()
        {
            Draw();


        }



        private static void Draw()
        {
            int frame = 0;
            string[] waves = new string[0];
            if (File.Exists("Map/Fishing/waves.txt"))
            {
                waves = File.ReadAllLines("Map/Fishing/waves.txt");
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
            StringBuilder buffer = new StringBuilder();
            while (true)
            {
                //Console.Clear();
                //Console.Write("\x1b[3j");
                Console.SetCursorPosition(0, 0);
                buffer.Clear();

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
                    foreach(char  c in cloud)
                    {
                        if (c != ' ')
                        {
                            buffer[index] = c;

                        }
                        
                        index++;
                    }
                }








                Console.Write(buffer.ToString());

                Console.ReadLine();



                frame++;
            }
        }



    }
}
