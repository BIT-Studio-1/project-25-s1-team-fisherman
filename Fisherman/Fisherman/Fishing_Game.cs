using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fisherman
{
    public class Fishing_Game
    {

        public Fishing_Game() {
            Draw();
        
        
        }



        private static void Draw()
        {
            if (File.Exists("Map/Fishing/waves.txt"))
            {
                string[] waves = File.ReadAllLines("Map/Fishing/waves.txt");
                foreach (string wave in waves)
                {
                    Console.WriteLine(wave);
                }
            }
            Console.ReadLine();




        }




    }
}
