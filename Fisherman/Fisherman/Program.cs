using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Team_Fisherman
{
    internal class Program
    {
        //These are ANSI escape sequences, they are the same thing as \n and \t but use a slightly different format. these color the text placed after them until the RESET is added, there are other sequences like underlining and italics as well. Theses are needed beacuse i am using string builders and Console.Color doesnt work with them. you can also combine them.
        //format for custom color is \x1b[38;5;{ID}m for foreground \x1b[48;5;{ID}m for background where {ID} is from the image in the discord
        const string BLUE = "\x1b[38;5;73m";
        const string RED = "\x1b[91m";
        const string GREEN = "\x1b[32m";
        const string YELLOW = "\x1b[33m";
        const string MAGENTA = "\x1b[35m";
        const string WHITE = "\x1b[97m";
        const string RESET = "\x1b[0m";
        static void Main(string[] args)
        {
            //this is a stringbuilder that makes it easy to edit the console output. it works like an a array 
            StringBuilder buffer = new StringBuilder();
            //Colored buffer just for display, dont use for any checks as To_Index wont work on it, holds the color codes as well so is longer than buffer 
            StringBuilder color_buffer = new StringBuilder();
            

            Console.CursorVisible = false;
            Console.Title = "Fishing";

            //the coordenets are stroed in a vector2, this has an X and Y value that represent the position on the screen, X is the number of charcters from the left and Y is the number of lines from the top, the top left corner is (0,0) and the bottom right corner is (119,27) since the screen size is 120x28. please use integers for the coordenets to avoid issues with indexing.

            Vector2 screen_size = new Vector2(120, 28);
            Vector2 half_screen = new Vector2(screen_size.X / 2, screen_size.Y / 2);
            // set this to the starting position of the player, currently it is set to the middle of the screen but you can change it to whatever you want, just make sure it is within the bounds of the screen and not on a wall or other object in the map.
            Vector2 player_pos = half_screen;

            Menu();
            while (true)
            {

                buffer.Clear();
                color_buffer.Clear();
                //sets the buffer to be a grid of ' ' characters with the size of the screen, this is the background of the game and will be overwritten with the player and any other objects in the game, you can change the character to whatever you want to make it look different. not needed but will leave commented for refrence.

                //for (int y = 0; y < screen_size.Y; y++)
                //{
                //    for (int x = 0; x < screen_size.X; x++)
                //    {
                //        buffer.Append(' ');
                //    }
                //    buffer.Append('\n');
                //}

                //allows you to edit the map by changing the text in the map.txt file, it will read the file and draw it to the console, you can change the layout of the map and the characters used for different objects by editing the file, just make sure to keep the player character in mind when designing your map so you don't accidentally block off areas or make it impossible for the player to move.
                //if you add a file like this make sure to add it to the project and set it to copy to the output directory so it can be read by the program, you can do this by right clicking on the file in the solution explorer and going to properties, then setting "Copy to Output Directory" to "Copy always".
                Vector2 char_pos = new Vector2(0, 0);
                foreach (string line in File.ReadLines("Map/map.txt"))
                {
                    foreach (char c in line)
                    {

                        string color = "";
                        switch (c)
                        {
                            case '~':
                                color = BLUE;
                                break;
                            case 'm':
                                color = RED;
                                break;
                            case 's':
                                color = GREEN;
                                break;
                            case 'd':
                                color = MAGENTA;
                                break;
                            case '#':
                                color = WHITE;
                                break;
                            default:
                                color = RESET;
                                break;
                        }
                        if (char_pos == player_pos)
                        {
                            color_buffer.Append($"{RESET}P");
                            buffer.Append(c);
                        }
                        else
                        {
                            color_buffer.Append($"{color}{c}");
                            buffer.Append(c);
                        }

                            
                        //buffer.Append(c);
                        char_pos.X += 1;
                    }
                    color_buffer.Append('\n');
                    buffer.Append('\n');
                    char_pos.Y += 1;
                    char_pos.X = 0;
                }




                //This sets the player's pos
                buffer[To_Index(player_pos, screen_size)] = 'P';

                //buffer[To_Index(half_screen, screen_size)] = '#';

                //this removes the flickering that happens when you clear the console and redraw everything, instead of clearing the console we just move the cursor back to the top left and overwrite the existing output with the new output, this makes it look like the player is moving smoothly without any flickering
                Console.SetCursorPosition(0, 0);
                Console.Write(color_buffer.ToString());

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
                case ConsoleKey.Escape:

                    Menu();
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
            else if (buffer[To_Index(coords, size)] == '~')
            {
                Fishing();
                return false;
            }
            else if (buffer[To_Index(coords, size)] == 's')
            {
                Shopping();
                return false;
            }
            else if (buffer[To_Index(coords, size)] == 'm')
            {
                Fighting();
                return false;
            }
            else if (buffer[To_Index(coords, size)] == 'd')
            {
                //add logic for key to unlock a door.
                //return true means the player can stand on that tile but it will trigger the door logic, you can change this to false if you want the player to not be able to stand on the tile and just trigger the door logic when they are next to it.
                return true;
            }


            return true;
        }
        //this helper function converts a Vector2 into an index for the buffer, this means you can access the buffer my chosing the pixel in the console. it takes a Vector2 (coords) and a Vector2 (size) and returns an int (index) for accessing the buffer.
        static int To_Index(Vector2 coords, Vector2 size)
        {
            int index = (int)(coords.X + (coords.Y * (size.X + 1)));
            return index;
        }

        //displays the menu and allows the player to choose between starting the game or exiting.
        static void Menu()
        {
            //this is a stringbuilder that makes it easy to edit the console output. it works like an a array
            StringBuilder buffer = new StringBuilder();
            bool in_menu = true;
            Vector2 screen_size = new Vector2(120, 28);
            Vector2 play_pos = new Vector2(34, 24);
            Vector2 exit_pos = new Vector2(62, 24);
            Vector2 current = play_pos;

            


            //this is the menu loop, it will keep running until the player chooses to start the game or exit, it works by drawing the menu and then checking for input, if the player presses A or D it will move the current selection left or right, if they press any other key it will check which option is currently selected and either start the game or exit based on that
            while (in_menu)
            {
                buffer.Clear();
                //for (int y = 0; y < screen_size.Y; y++)
                //{
                //    for (int x = 0; x < screen_size.X; x++)
                //    {
                //        buffer.Append(' ');
                //    }
                //    buffer.Append('\n');
                //}

                //allows you to edit the menu by changing the text in the menu.txt file, it will read the file and draw it to the console, you can change the text and layout of the menu by editing the file, just make sure to keep the play and exit options in the same place or update the play_pos and exit_pos variables to match the new positions. File.ReadLines("Map/menu.txt") returns an array of strings, each string is a line in the file, the foreach loop goes through each line and then through each character in the line and draws it to the buffer at the correct position based on the char_pos variable which is updated as it goes through the characters and lines.
                Vector2 char_pos = new Vector2(0, 0);
                foreach (string line in File.ReadLines("Map/menu.txt"))
                {
                    foreach (char c in line)
                    {
                        




                        buffer.Append( c);
                        char_pos.X += 1;
                    }
                    buffer.Append("\n");
                    char_pos.Y += 1;
                    char_pos.X = 0;
                }



                current = Vector2.Clamp(current, play_pos, exit_pos);

                //adds an underline to the current selection by stepping through the array and changing the characters to '#' for the length of the text.

                for (int x = (int)current.X; x < current.X + 23; x++)
                {
                    buffer[To_Index(new Vector2(x, 24), screen_size)] = '#';
                }


                //this removes the flickering that happens when you clear the console and redraw everything, instead of clearing the console we just move the cursor back to the top left and overwrite the existing output with the new output, this makes it look like the menu is moving smoothly without any flickering
                Console.SetCursorPosition(0, 0);
                Console.Write(buffer.ToString());

                //this gets the input from the player and updates the current selection based on the input, if they press A it will move the selection left, if they press D it will move the selection right;
                var Key = Console.ReadKey(true).Key;
                switch (Key)
                {
                    case ConsoleKey.A:
                        current.X -= 28;
                        break;
                    case ConsoleKey.D:
                        current.X += 28;
                        break;
                    default:
                        if (current == play_pos)
                        {
                            //breakes the while statement and starts the game.
                            in_menu = false;
                        }
                        else if (current == exit_pos)
                        {
                            //closes the game window.
                            Environment.Exit(0);
                        }
                        break;

                }
            }
        }
        //Put the code for fishing in here.
        static void Fishing()
        {



            Thread.Sleep(2000);
            Console.Clear();

        }
        //Put the code for shopping in here.
        static void Shopping()
        {


            Console.WriteLine("Shopping");
            Thread.Sleep(2000);


        }
        //Put the code for fighting in here.
        static void Fighting()
        {
            Console.WriteLine("Fighting");

            Thread.Sleep(2000);
            //Dodging();
        }
        //Put the code for dodging in here.
        static void Dodging()
        {

            Console.WriteLine("Dodging");
            Thread.Sleep(2000);
        }



    }
}
