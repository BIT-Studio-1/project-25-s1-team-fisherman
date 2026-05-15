using System.Numerics;
using System.Text;

namespace Team_Fisherman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //this is a stringbuilder that makes it easy to edit the console output. it works like an a array 
            StringBuilder buffer = new StringBuilder();
            Console.CursorVisible = false;
            Vector2 screen_size = new Vector2(120, 28);
            Vector2 player_pos = new Vector2(0,0);
            Vector2 half_screen = new Vector2(screen_size.X / 2, screen_size.Y / 2);
            while (true)
            {

                buffer.Clear();
                //sets the buffer to be a grid of ' ' characters with the size of the screen, this is the background of the game and will be overwritten with the player and any other objects in the game, you can change the character to whatever you want to make it look different
                for (int y = 0; y < screen_size.Y; y++)
                {
                    for (int x = 0; x < screen_size.X; x++)
                    {
                        buffer.Append(' ');
                    }
                    buffer.Append('\n');
                }
                Vector2 char_pos = new Vector2(0, 0);
                foreach (string line in File.ReadLines("Map/map.txt"))
                {
                    foreach (char c in line)
                    {
                        buffer[To_Index(char_pos, screen_size)] = c;
                        char_pos.X += 1;
                    }
                    char_pos.Y += 1;
                    char_pos.X = 0;
                }



                
                //This sets the player's pos
                buffer[To_Index(player_pos, screen_size)] = 'P';

                //buffer[To_Index(half_screen, screen_size)] = '#';

                //this removes the flickering that happens when you clear the console and redraw everything, instead of clearing the console we just move the cursor back to the top left and overwrite the existing output with the new output, this makes it look like the player is moving smoothly without any flickering
                Console.SetCursorPosition(0, 0);
                Console.Write(buffer.ToString());

                player_pos = Move(player_pos, buffer, screen_size);


            }

        }
        //this function moves the player based on the input and checks if the new position is valid before moving, if you want to add any keybindings just add a new case to the switch statement and call your method
        static Vector2 Move(Vector2 offset, StringBuilder buffer, Vector2 size)
        {
            var Key = Console.ReadKey(true).Key;
            switch (Key)
            {
                case ConsoleKey.W:
                   if (Is_Valid(new Vector2(offset.X, offset.Y - 1), size, buffer))
                        offset.Y -= 1;
                    break;
                case ConsoleKey.S:
                    if (Is_Valid(new Vector2(offset.X, offset.Y + 1), size, buffer))
                        offset.Y += 1;
                    break;
                case ConsoleKey.A:
                    if (Is_Valid(new Vector2(offset.X - 1, offset.Y), size, buffer))
                        offset.X -= 1;
                    break;
                case ConsoleKey.D:
                    if (Is_Valid(new Vector2(offset.X + 1, offset.Y), size, buffer))
                        offset.X += 1;
                    break;
            }

            offset = Vector2.Clamp(offset, Vector2.Zero, new Vector2(119, 27));
            return offset;
        }
        //Checks if the new position is valid by checking if there is a '#' in the buffer at the new position, if there is it returns false and the player will not move, if there isn't it returns true and the player will move to the new position, add another if statment 
        static bool Is_Valid(Vector2 coords, Vector2 size, StringBuilder buffer)
        {
            coords = Vector2.Clamp(coords, Vector2.Zero, new Vector2(119, 27));
            if (buffer[To_Index(coords, size)] == '#')
                return false;
            
            return true;
        }
        //this function converts a Vector2 into an index for thhe buffer, this means you can access the buffer my chosing the pixel in the console
        static int To_Index(Vector2 coords, Vector2 size)
        {
            int index = (int)(coords.X + (coords.Y * (size.X + 1)));
            return index;
        }

        static void Fishing()
        {



        }








    }
}
