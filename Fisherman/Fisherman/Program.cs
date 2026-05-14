using System.Numerics;
using System.Text;

namespace Team_Fisherman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //this is a stringbuilder that makes it easy to edit the console output.
            StringBuilder buffer = new StringBuilder();
            Console.CursorVisible = false;
            Vector2 screen_size = new Vector2(120, 28);
            Vector2 player_pos = new Vector2(0,0);
            Vector2 half_screen = new Vector2(screen_size.X / 2, screen_size.Y / 2);
            while (true)
            {
                buffer.Clear();
                for (int y = 0; y < screen_size.Y; y++)
                {
                    for (int x = 0; x < screen_size.X; x++)
                    {
                        buffer.Append('.');
                    }
                    buffer.Append('\n');
                }




                

                buffer[To_Index(player_pos, screen_size)] = 'P';
                buffer[To_Index(half_screen, screen_size)] = '#';
                Console.SetCursorPosition(0, 0);
                Console.Write(buffer.ToString());
                player_pos = Move(player_pos, buffer, screen_size);


            }

        }
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
        static bool Is_Valid(Vector2 coords, Vector2 size, StringBuilder buffer)
        {
            coords = Vector2.Clamp(coords, Vector2.Zero, new Vector2(119, 27));
            if (buffer[To_Index(coords, size)] == '#')
                return false;
            return true;
        }
        static int To_Index(Vector2 coords, Vector2 size)
        {
            int index = (int)(coords.X + (coords.Y * (size.X + 1)));
            return index;
        }
    }
}
