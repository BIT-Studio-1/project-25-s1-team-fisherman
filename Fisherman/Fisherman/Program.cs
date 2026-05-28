

using System.Drawing;




using System;
using System.Numerics;

using System.Security;

using System.Text;

using test_fish;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Reflection.Metadata.Ecma335;
/*Put suggestions/bugs here 
 ln 1200 put a Console.Readline(); currently skips the text
 ln 1132 put a new line char before "the". The word wraps in the console and becomes Th and e 
 
 
 
 
 
 */



namespace Team_Fisherman
{
    internal class Program
    {
        //These are ANSI escape sequences, they are the same thing as \n and \t but use a slightly different format. these color the text placed after them until the RESET is added, there are other sequences like underlining and italics as well. Theses are needed because i am using string builders and Console.Color doesn't work with them. you can also combine them.
        //format for custom color is \x1b[38;5;{ID}m for foreground \x1b[48;5;{ID}m for background where {ID} is from the image in the discord
        //they are found here https://gist.github.com/fnky/458719343aabd01cfb17a3a4f7296797
        const string BLUE = "\x1b[38;5;73m";
        const string RED = "\x1b[91m";
        const string GREEN = "\x1b[32m";
        const string YELLOW = "\x1b[33m";
        const string MAGENTA = "\x1b[95m";
        const string WHITE = "\x1b[97m";
        const string RESET = "\x1b[0m";
        public static Vector2 screen_size = new Vector2(120, 28);

        public static Dictionary<string, int> inventory = new Dictionary<string, int>();
        public static int coins = 200;



        static void Main(string[] args)
        {

            Random rand = new Random();
            //this is a string builder that makes it easy to edit the console output. it works like an a array 
            StringBuilder buffer = new StringBuilder();
            //Colored buffer just for display, don't use for any checks as To_Index wont work on it, holds the color codes as well so is longer than buffer 
            StringBuilder color_buffer = new StringBuilder();


            Console.CursorVisible = false;
            Console.Title = "Fishing";
            Console.OutputEncoding = Encoding.UTF8;

            //the coordinates are stored in a vector2, this has an X and Y value that represent the position on the screen, X is the number of characters from the left and Y is the number of lines from the top, the top left corner is (0,0) and the bottom right corner is (119,27) since the screen size is 120x28. please use integers for the coordinates to avoid issues with indexing.

            Vector2 screen_size = new Vector2(120, 28);
            Vector2 half_screen = new Vector2(screen_size.X / 2, screen_size.Y / 2);
            // set this to the starting position of the player, currently it is set to the middle of the screen but you can change it to whatever you want, just make sure it is within the bounds of the screen and not on a wall or other object in the map.
            Vector2 player_pos = half_screen;
            Vector2 map_tile = new Vector2(0, 0);
            Vector2 map_offset = new Vector2(0, 0);
            string path = "Map/map_start.txt";





            bool map_changed = false;




            Menu();
            //inventory_menu();
            //Wait();
            //GameIntro();
            Console.Write(RESET);
            Console.Clear();
            Console.Write("\x1b[3j");
            //Console.Clear();
            //Console.ReadLine();


            while (true)
            {
                path = Get_path(map_tile, path, out map_changed);
                if (map_changed)
                {
                    if (player_pos.X == 118)
                    {
                        player_pos.X = 1;
                    }
                    else if (player_pos.X == 0)
                    {
                        player_pos.X = 118;
                    }
                    else if (player_pos.Y == 0)
                    {
                        player_pos.Y = 27;
                    }
                    else if (player_pos.Y == 27)
                    {
                        player_pos.Y = 0;
                    }
                }

                buffer.Clear();
                color_buffer.Clear();
                //sets the buffer to be a grid of ' ' characters with the size of the screen, this is the background of the game and will be overwritten with the player and any other objects in the game, you can change the character to whatever you want to make it look different. not needed but will leave commented for reference.

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
                foreach (string line in File.ReadLines(path))
                {
                    foreach (char ch in line)
                    {
                        char c = ch;
                        if (c == '?')
                        {
                            if (rand.Next(41) == 1)
                            {
                                c = 'm';
                            }
                            else
                            {
                                c = ' ';
                            }

                        }
                        string color = RESET;
                        switch (c)
                        {
                            case '~':
                                color += BLUE;
                                break;
                            case 'm':
                                color += Color_Helper(0, true);
                                break;
                            case 's':
                                color += GREEN;
                                break;
                            case 'd':
                                color += MAGENTA;
                                break;
                            case '#':
                                color += WHITE;
                                break;
                            case '+':
                            case '"':
                                color += GREEN;
                                break;
                            case '>':
                            case '=':
                            case '<':
                                color += BLUE;
                                break;
                            case '▒':
                                color += Color_Helper(130, true) + "\x1b[2m";
                                break;
                            case '░':
                                color += Color_Helper(94, true) + "\x1b[2m";
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
                    color_buffer.Append(RESET);
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
                Vector2 t_pos = map_offset;
                player_pos = Move(player_pos, buffer, screen_size, out map_offset);
                //map_tile = Vector2.Clamp(map_tile, new Vector2(0, -1), Vector2.One);
                map_tile += map_offset;



            }

        }
        //this function moves the player based on the input and checks if the new position is valid before moving, if you want to add any key bindings just add a new case to the switch statement and call your method
        static Vector2 Move(Vector2 offset, StringBuilder buffer, Vector2 size, out Vector2 map_tile)
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
                case ConsoleKey.I:
                    inventory_menu();
                    break;

            }

            Vector2 clamped_offset = Vector2.Clamp(offset, Vector2.Zero, new Vector2(118, 27));
            map_tile = clamped_offset - offset;
            return clamped_offset;
        }

        //up 
        //start right
        //down
        static string Get_path(Vector2 map_pos, string current, out bool map_changed)
        {
            string path = current;
            map_changed = false;

            if (map_pos == Vector2.UnitY)
            {
                //Console.WriteLine("up");
                path = "Map/map_up.txt";
                if (current != path)
                {
                    map_changed = true;
                }
            }
            else if (map_pos == Vector2.Zero)
            {
                //Console.WriteLine("start");
                path = "Map/map_start.txt";
                if (current != path)
                {
                    map_changed = true;
                }
            }
            else if (map_pos == -Vector2.UnitX)
            {

                path = "Map/map_right.txt";
                if (current != path)
                {
                    map_changed = true;
                }
            }
            else
            {
                map_changed = false;
                path = current;
            }

            return path;
        }
        //Checks if the new position is valid by checking if there is a '#' in the buffer at the new position, if there is it returns false and the player will not move, if there isn't it returns true and the player will move to the new position, add another if statement 
        static bool Is_Valid(Vector2 coords, Vector2 size, StringBuilder buffer)
        {
            coords = Vector2.Clamp(coords, Vector2.Zero, new Vector2(119, 28));
            if (buffer[To_Index(coords, size)] == '#' || buffer[To_Index(coords, size)] == '+')
                return false;
            else if (buffer[To_Index(coords, size)] == '~')
            {
                Console.Clear();
                Fishing();
                return false;
            }
            else if (buffer[To_Index(coords, size)] == 'S')
            {
                Console.Clear();
                Shopping();
                return false;
            }
            else if (buffer[To_Index(coords, size)] == 'm')
            {
                Console.Clear();
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
        //this helper function converts a Vector2 into an index for the buffer, this means you can access the buffer my choosing the pixel in the console. it takes a Vector2 (coords) and a Vector2 (size) and returns an int (index) for accessing the buffer.
        static int To_Index(Vector2 coords, Vector2 size)
        {
            int index = (int)(coords.X + (coords.Y * (size.X + 1)));
            return index;
        }
        //This returns the escape code for the specified color based on the image in the discord, setting the is_foreground changes if the text color changes or the highlight color
        static string Color_Helper(int id, bool is_foreground)
        {
            string color = "";
            if (is_foreground)
            {
                color = $"\x1b[38;5;" + id + "m";
            }

            else
            {
                color = $"\x1b[48;5;" + id + "m";
            }


            return color;
        }

        //displays the menu and allows the player to choose between starting the game or exiting.

        static int Get_item_Count(string name)
        {
            int count = 0;
            inventory.TryGetValue(name, out count);

            return count;
        }
        static void Add_item(string name, int count)
        {
            if (inventory.ContainsKey(name))
            {
                inventory[name] += count;
            }
            else
            {
                inventory.Add(name, count);
            }
        }
        static void Remove_item(string name, int count)
        {
            if (inventory.ContainsKey(name))
            {
                inventory[name] -= count;
                if (inventory[name] <= 0)
                {
                    inventory.Remove(name);
                }
            }
        }
        static string[] display_inventory()
        {
            string[] buffer = new string[inventory.Count];
            foreach (var pair in inventory)
            {
                string item = pair.Key;
                string count = pair.Value.ToString();
                string line = item.PadRight(20) + ": " + count;

                buffer = buffer.Append(line).ToArray();
            }
            return buffer;
        }

        static void inventory_menu()
        {
            while (true)
            {


                string[] inv = display_inventory();
                Console.Clear();
                Console.WriteLine("Inventory");
                Console.WriteLine("=========");
                foreach (string item in inv)
                {
                    Console.WriteLine(item);
                }
                
                Console.WriteLine();
                Console.WriteLine("add");
                string add = Console.ReadLine();
                Console.WriteLine("item");
                string item_name = Console.ReadLine().Trim();
                Console.WriteLine("count");
                string count = Console.ReadLine();
                if (count != null && item_name != null)
                {
                    int int_count = Convert.ToInt32(count);
                    if (add == "add")
                    {
                        Add_item(item_name, int_count);
                    }
                    else if (add == "remove")
                    {
                        Remove_item(item_name, int_count);
                    }
                    else
                    {
                        break;
                    }
                    //Add_item(item_name, int_count);
                }


                //Console.ReadLine();
            }
        }


        static void Menu()
        {
            //this is a string builder that makes it easy to edit the console output. it works like an a array
            StringBuilder buffer = new StringBuilder();
            bool in_menu = true;
            //Vector2 screen_size = new Vector2(120, 28);
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


                foreach (string line in File.ReadLines("Map/menu.txt"))
                {
                    foreach (char c in line)
                    {

                        buffer.Append(c);
                    }
                    buffer.Append("\n");

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
                            //breaks the while statement and starts the game.
                            in_menu = false;
                            Console.Clear();
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

            Fishing_Base.Fishing();

            Thread.Sleep(500);
            Console.Clear();


        }
        static void Shop()
        {
            string buy;
            int count = 0;
            string[] m = { "Potion", "Fish Bait", "Jar of Dirt", "Protective Charm" };
            int[] p = { 15, 5, 1, 100 };
            Console.WriteLine("\"Looking for any supplies?\"");
            Console.WriteLine("========================BUY SYSTEM=========================");
            for (int i = 0; i < m.Length; i++)
            {
                Console.Write(i.ToString().PadRight(10));
                Console.Write(m[i].PadLeft(15));
                Console.WriteLine(p[i].ToString().PadLeft(25));
            }

            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"The sea is dangerous after dark.\"");
            Console.WriteLine("\"Buy what you need before heading out.\"");
            while (true)
            {
                buy = Console.ReadLine();
                switch (buy)
                {
                    case "Potion":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= p[0] * count;
                        if (coins > 0)
                        {
                            Add_item("Potion", count);
                        }
                        else
                        {
                            coins += p[0] * count;
                        }

                        break;

                    case "Fish Bait":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= p[1] * count;
                        if (coins > 0)
                        {
                            Add_item("Fish Bait", count);
                        }
                        else
                        {
                            coins += p[1] * count;
                        }
                        break;
                    case "Jar of Dirt":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= p[2] * count;
                        if (coins > 0)
                        {
                            Add_item("Jar of Dirt", count);
                        }
                        else
                        {
                            coins += p[2] * count;
                        }
                        break;

                    case "Protective Charm":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= p[3] * count;
                        if (coins > 0)
                        {
                            Add_item("Protective Charm", count);
                        }
                        else
                        {
                            coins += p[3] * count;
                        }
                        break;
                }
                Console.WriteLine(coins);
                if (Console.ReadLine().ToLower() == "exit") break;
            }
            Console.WriteLine("\"\"");
        }
        //Put the code for shopping in here.
        static void Shopping()
        {
            //Shop();
            string temp, shop, buy, talk1, talk2, check, check1, truth1;

            string[] c = { "Buy", "Sell", "Talk", "Exit", "Leave" };
            Console.WriteLine("Shopping");
            Console.WriteLine("\"Hi, nice to meet you. My name is Jane.\"");
            Console.WriteLine("\"Is there anything I can help you?\"");
            Console.WriteLine();
            Console.WriteLine("======JANE'S SHOP========");
            for (int i = 0; i < c.Length; i++)
            {
                Console.Write(i.ToString().PadRight(10));
                Console.WriteLine(c[i].PadLeft(10));
            }
            Console.WriteLine();
            shop = Console.ReadLine();


            switch (shop)
            {
                case "buy":
                    string[] m = { "Potion", "Fish Bait", "Torch", "Lantern", "Boots", "Jar of Dirt", "Protective Charm" };
                    int[] p = { 15, 5, 20, 40, 50, 1, 100 };
                    Console.WriteLine("\"Looking for any supplies?\"");
                    Console.WriteLine("========================BUY SYSTEM=========================");
                    for (int i = 0; i < m.Length; i++)
                    {
                        Console.Write(i.ToString().PadRight(10));
                        Console.Write(m[i].PadLeft(15));
                        Console.WriteLine(p[i].ToString().PadLeft(25));
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                    Thread.Sleep(1000);
                    Console.WriteLine("\"The sea is dangerous after dark.\"");
                    Console.WriteLine("\"Buy what you need before heading out.\"");
                    buy = Console.ReadLine();

                    Shop();
                    break;
                case "sell":
                    Console.WriteLine("\"What you want to sell?\"");
                    temp = Console.ReadLine();
                    Console.WriteLine("\"Wow, fresh catches\"");
                    Console.WriteLine("\"The villagers rely on fish more than you think.\"");
                    Console.WriteLine("\"I'll give you a fair prices\"");

                    break;

                case "talk":
                    Console.WriteLine();
                    Console.WriteLine("\"So, what's you want to talk to me?\"");
                    check1 = Console.ReadLine();
                    Console.WriteLine("\"Is this the first time we talk about this?*(YES/NO)*\"");
                    check = Console.ReadLine();

                    if (check == "yes")
                    {
                        Console.WriteLine("\"So, you want to know about what?\"");
                        talk1 = Console.ReadLine();

                        if (talk1 == "memory fish" || talk1 == "truth" || talk1 == "truth fragment")
                        {
                            Console.WriteLine("\"...So you've started seeing them too.\"");
                            Console.WriteLine();
                            Thread.Sleep(1000);
                            Console.WriteLine("\"Those fish only appear near the fog.\"");
                            Console.WriteLine("\"Some say they carry pieces of lost memories.\"");
                            Thread.Sleep(1000);
                            Console.WriteLine("\"Others say they show people things they were never meant to remember.\"");
                            Thread.Sleep(1500);
                            Console.WriteLine("\"Here.Take this.\"");
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Thread.Sleep(1000);
                            Console.WriteLine("*You received a TRUTH FRAGMENT*");
                            Console.WriteLine("\"Don't let others see this\"");
                            Console.WriteLine();
                            Console.WriteLine();
                            Thread.Sleep(1000);
                            Console.WriteLine("*WOULD YOU WANT TO OPEN IT KNOW (YES/NO)*");
                            truth1 = Console.ReadLine();
                            if (truth1 == "yes")
                            {
                                Thread.Sleep(1000);
                                Console.WriteLine("*THE SEA DOES NOT RELEASE WHAT IT TAKES. IT KEEPS THEM. IT RESHAPES THEM INTO PART OF THE VILLAGE.*");
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.WriteLine("Press Enter to close it");
                                Console.ReadLine();

                            }
                            else
                            {
                                Console.WriteLine();
                            }
                            Console.WriteLine();
                            Console.WriteLine();
                            Thread.Sleep(1500);
                            Console.WriteLine("\"It's weird right.\"");
                            Console.WriteLine("\"If you really want to know more.....\"");
                            Console.WriteLine();
                            Thread.Sleep(1500);
                            Console.WriteLine("\"You might want to check the forest near the shore....\"");
                            Console.WriteLine();
                            Thread.Sleep(1000);
                            Console.WriteLine("\"Strange things wash up there when the fog gets thick.\"");
                            Thread.Sleep(1000);
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine("Press Enter to exit");
                            Console.ReadLine();
                        }
                    }
                    else
                        Console.WriteLine("\"So, what's you want to talk to me?\"");
                    talk2 = Console.ReadLine();

                    if (talk2 == "memory fish")
                    {
                        Console.WriteLine("\"I don't know what's you are talking about.\"");
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("\"Maybe you should take a rest.\"");
                        Thread.Sleep(1000);
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("\"The fog does strange things to people's minds.\"");
                        Thread.Sleep(1000);
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("Press Enter to exit");
                        Console.ReadLine();
                    }
                    break;

                case "Exit":
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to exit");
                    Console.ReadLine();
                    break;

                case "leave":
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("\"......Leave??\"");
                    Thread.Sleep(2000);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("\"There is no such thing here.\"");
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    Console.WriteLine("\"Anyway come back if you need supplies.\"");
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to exit");
                    Console.ReadLine();
                    break;
            }





        }
        
        //Put the code for fighting in here.
        static void fishyGame()
        {

            
            string fishIcon1 = @" _              ";
            string fishIcon2 = @"\ \ ____|\____  ";
            string fishIcon3 = @" \ /        o \ ";
            string fishIcon4 = @"  (   ||       )";
            string fishIcon5 = @" / \__________/ ";
            string fishIcon6 = @"/_/             ";

            int fishX = 30;
            int barX = 30;
            bool yak = true;
            do
            {
                static void WriteTing(ref StringBuilder buff, string guy, Vector2 screen_size, Vector2 pos)
                {
                    int count = 0;
                    foreach (char h in guy)
                    {
                        //colBuff.Append($"{color}{ch}");
                        //color += RED;
                        buff[To_Index(new Vector2(count + pos.X, pos.Y), screen_size)] = h;
                        count++;
                    }
                }
                //color_buffer.Clear();
                StringBuilder buffer = new StringBuilder();
                
                buffer.Clear();
                Console.SetCursorPosition(0, 0);
                string[] fish = { fishIcon1, fishIcon2, fishIcon3, fishIcon4, fishIcon5, fishIcon6 };


                //string[] ben = { "unite", "talking", "benjamin", "army", "mountain", "glorious", "love", "fork", "creator", "yogurt", "computer" };

                foreach (string line in File.ReadLines("Map/fighting/fishMap.txt")) // loops through txt file
                {
                    foreach (char p in line) //each line
                    {
                        buffer.Append(p);
                    }
                    buffer.Append("\n");
                }

                WriteTing(ref buffer, fishIcon1, screen_size, new Vector2(fishX, 21));
                WriteTing(ref buffer, fishIcon2, screen_size, new Vector2(fishX, 22));
                WriteTing(ref buffer, fishIcon3, screen_size, new Vector2(fishX, 23));
                WriteTing(ref buffer, fishIcon4, screen_size, new Vector2(fishX, 24));
                WriteTing(ref buffer, fishIcon5, screen_size, new Vector2(fishX, 25));
                WriteTing(ref buffer, fishIcon6, screen_size, new Vector2(fishX, 26));

                WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 21));
                WriteTing(ref buffer ,"#####################", screen_size, new Vector2(barX, 22));
                WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 23));
                WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 24));
                WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 25));
                WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 26));
                //WriteTing(ref buffer, barX.ToString(), screen_size, new Vector2(barX, 15));
                Console.Write(buffer.ToString());

                ConsoleKeyInfo c = new ConsoleKeyInfo();
                c = Console.ReadKey(true);
                //WriteTing(ref buffer, barX.ToString(), screen_size, new Vector2(barX, 15));
                //Console.WriteLine(barX);
                //int waity = -1; 
                //int x = 0;
                //string badGuyAttack = "fuck this game";
                //while (Console.KeyAvailable == false)
                //{
                //    // running code
                //    if (waity == 0)
                //    {
                //        Console.Clear();
                //        badGuyAttack = "";
                //        WriteTing(ref buffer, "                                               ", screen_size, new Vector2(64, 16));
                //        Console.SetCursorPosition(0, 0);
                //        Console.Write(buffer.ToString());
                //    }
                //    waity--;
                //    Thread.Sleep(50);
                //    x++;
                //}
                //c = Console.ReadKey(true);
                if (c.Key == ConsoleKey.Spacebar)
                {
                    if (barX > 10)
                    {
                        barX -= 10;
                    }
                }

                while (Console.KeyAvailable == false )
                {
                    if (barX < 90)
                    {
                        Console.Clear();
                        barX += 1;
                        WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 21));
                        WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 22));
                        WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 23));
                        WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 24));
                        WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 25));
                        WriteTing(ref buffer, "#####################", screen_size, new Vector2(barX, 26));
       
                        Console.SetCursorPosition(0, 0);
                        Console.Write(buffer.ToString());
                    }
                    yak = false;
                    

                }

                if (c.Key == ConsoleKey.A) 
                {
                    if (fishX > 4) 
                    {
                        fishX -= 1;
                    }

                    fishIcon1 = @"              _ ";
                    fishIcon2 = @"  ____/|____ / /";
                    fishIcon3 = @" / o        \ / ";
                    fishIcon4 = @"(      ||    )  ";
                    fishIcon5 = @" \__________/ \ ";
                    fishIcon6 = @"             \_\";
                }
                
                if (c.Key == ConsoleKey.D)
                {
                    if (fishX < 100) 
                    {
                        fishX += 1;
                        
                    }
                    fishIcon1 = @" _              ";
                    fishIcon2 = @"\ \ ____|\____  ";
                    fishIcon3 = @" \ /        o \ ";
                    fishIcon4 = @"  (   ||       )";
                    fishIcon5 = @" / \__________/ ";
                    fishIcon6 = @"/_/              ";
                }
                yak = true;
            } while (true);
            
        }
        
        
            
       
        
        static void Fighting()
        {
            fishyGame();
            Thread.Sleep(2134567);

            static void WriteTing(ref StringBuilder buff, string guy, Vector2 screen_size, Vector2 pos)
            {
                int count = 0;
                foreach (char h in guy)
                {
                    //colBuff.Append($"{color}{ch}");
                    //color += RED;
                    buff[To_Index(new Vector2(count + pos.X, pos.Y), screen_size)] = h;
                    count++;
                }
            }
           
            string[] inventory = { "23", "fish", "2", "health potion", "56", "rock" };
            string[] attacks = { "slash", "jab", "bow", "other" };
            string[] defence = { "block", "dodge", "parry", "other" };

            //object[] mixedArray = new object[] { 10, "Hello World", 25, "C# Rocks" };


            ConsoleKeyInfo c = new ConsoleKeyInfo();

            Random random = new Random();
            int x = 0;

            int playerSpeed = 24;

            int badGuyHealth = 100;
            int badGuySpeed = 12;
            string badGuyAttack = "";
            string badGuyName = "evil guy of doom";
            int health = 100;
            bool gameRunning = true;
            int waity = -1;

            void alive()
            {
                if (badGuyHealth <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("the bad guy is dead you win");
                    Console.WriteLine("");
                    gameRunning = false;
                }
                else if (health <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("you are dead you lose");
                    Console.WriteLine("");
                    gameRunning = false;
                }
            }
            // the enemies attack  //code for dodging - returns an int depending on how much damage taken 0 dead 1 survived 2 perfect -1 something went wrong
            void Attack(int playerDamg)
            {
                int damage = 0; //random.Next(20, 40);
                if (playerSpeed >= badGuySpeed)
                {
                    badGuyHealth -= playerAttack(playerDamg);
                    alive();
                    health -= enemyAttack();
                    alive();
                }
                else
                {
                    health -= enemyAttack();
                    alive();
                    badGuyHealth -= playerAttack(playerDamg);
                    alive();
                }
            }

            static int playerAttack(int playerDamg)
            {

                return playerDamg;
            }

            int enemyAttack()
            {
                //damage !!!
                Random random = new Random();
                int damage = 0; //random.Next(20, 40);
                health = Dodging(health, 10);

                //switch (Dodging(health))
                //{
                //    case 0: //death
                //        return 2344;

                //    case 1: // failed
                //        return random.Next(20, 40);

                //    case 2: // perfect
                //        return 0;

                //    case -1: //error
                //        Console.WriteLine("error in the dodging");
                //        break;
                //}

                //health -= damage;
                //evil guy of doom does 33 damage |

                badGuyAttack = badGuyName + " does " + damage + " damage";
                waity = 100;
                return 3;
            }

            // this function converts the string num relating to the "item" to a int then adds or subtracts the "opper" amount
            void InventoryNum(string item, int opper)
            {
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] == item)
                    {
                        Console.WriteLine(inventory[i - 1]);

                        int int_inv = Convert.ToInt32(inventory[i - 1]);
                        if (int_inv > 0)
                        {
                            int_inv += opper;
                            inventory[i - 1] = int_inv.ToString();
                        }
                        else
                        {
                            Console.WriteLine("you don't have enough");
                        }
                        return;
                    }
                }
            }

            

            StringBuilder buffer = new StringBuilder();
            //StringBuilder color_buffer = new StringBuilder();
            bool in_menu = true;
            Vector2 screen_size = new Vector2(120, 28);
            Vector2 play_pos = new Vector2(34, 24);
            Vector2 exit_pos = new Vector2(62, 24);
            Vector2 current = play_pos;

            //string enemyIcon1 = "               ";
            //string enemyIcon2 = " (\\___/)      ";
            //string enemyIcon3 = " | >:) |    /  ";
            //string enemyIcon4 = "  -----    /   ";
            //string enemyIcon5 = "   |_____ /    ";
            //string enemyIcon6 = "   /\\    /\\    ";

            string enemyIcon1 = @" _                     0 ";
            string enemyIcon2 = @"\ \ ____|\____       0   ";
            string enemyIcon3 = @" \ /        o \   0   ";
            string enemyIcon4 = @"  (   ||       ) ";
            string enemyIcon5 = @" / \__________/      ";
            string enemyIcon6 = @"/_/             ";
            do
            {
                //Console.WriteLine("\nPress a key to display; " +
                //                  "press the 'z' key to quit.");

                //while (Console.KeyAvailable == false)
                //{
                //    //Console.WriteLine("running");
                //    //Console.WriteLine("ham");
                //    // running code
                //    Thread.Sleep(50);
                //    x++;
                //}

                // bad guy name

                // @@@

                //buffer.Clear();
                //Console.SetCursorPosition(0, 0);


                buffer.Clear();
                //color_buffer.Clear();
                Console.SetCursorPosition(0, 0);

                foreach (string line in File.ReadLines("Map/fighting/FightingMenu.txt")) // loops through txt file
                {
                    foreach (char p in line) //each line
                    {
                        buffer.Append(p);
                    }
                    buffer.Append("\n");
                }
                /*
                //WriteTing(ref buffer, enemyIcon1, screen_size, new Vector2(66, 7));
                //WriteTing(ref buffer, enemyIcon2, screen_size, new Vector2(66, 8));
                //WriteTing(ref buffer, enemyIcon3, screen_size, new Vector2(66, 9));
                //WriteTing(ref buffer, enemyIcon4, screen_size, new Vector2(66, 10));
                //WriteTing(ref buffer, enemyIcon5, screen_size, new Vector2(66, 11));
                //WriteTing(ref buffer, enemyIcon6, screen_size, new Vector2(66, 12));
                ////WriteTing(buffer, enemyIcon2, screen_size);
                //WriteTing(buffer, enemyIcon3, screen_size);
                //WriteTing(buffer, enemyIcon4, screen_size);
                //WriteTing(buffer, enemyIcon5, screen_size);
                //WriteTing(buffer, enemyIcon6, screen_size);
                //foreach (char ch in "-----")
                //{
                //    buffer[To_Index(new Vector2(count + 66, 8), screen_size)] = ch;
                //    count++;
                //}


                //for (int q = (int)current.X; q < current.X + 23; q++)
                //{
                //    //Console.WriteLine("ran");
                //    buffer[To_Index(new Vector2(q, 15), screen_size)] = '#';
                //}

                */
                //Console.Write(buffer.ToString()); // writes the thing
                //Console.ReadLine();

                WriteTing(ref buffer, enemyIcon1, screen_size, new Vector2(66, 7));
                WriteTing(ref buffer, enemyIcon2, screen_size, new Vector2(66, 8));
                WriteTing(ref buffer, enemyIcon3, screen_size, new Vector2(66, 9));
                WriteTing(ref buffer, enemyIcon4, screen_size, new Vector2(66, 10));
                WriteTing(ref buffer, enemyIcon5, screen_size, new Vector2(66, 11));
                WriteTing(ref buffer, enemyIcon6, screen_size, new Vector2(66, 12));
                WriteTing(ref buffer, badGuyName, screen_size, new Vector2(66, 13));
                WriteTing(ref buffer, "enemy health: " + badGuyHealth, screen_size, new Vector2(66, 14));

                WriteTing(ref buffer, badGuyAttack, screen_size, new Vector2(64, 16));
                //WriteTing(ref buffer, Color_Helper(88, true), screen_size, new Vector2(65, 16)); //86
                //WriteTing(ref buffer, RESET, screen_size, new Vector2(89, 17));

                //evil guy of doom does 33 damage
                //Console.WriteLine(badGuyAttack);
                WriteTing(ref buffer, "health: " + health, screen_size, new Vector2(10, 22));

                Console.Write(buffer.ToString()); // writes the thing
                                                  //Console.ReadLine();


                //Console.WriteLine("waiting");

                //string enemyIcon1 = "   _/\\___/\\_  ";
                //string enemyIcon2 = "  |  @ __ @ |   ";
                //string enemyIcon3 = "  |_________|   ";
                //string enemyIcon4 = "   /|.   .|\\   ";
                //string enemyIcon5 = "  / |_____| \\  ";
                //string enemyIcon6 = "    |     |     ";
                //Console.Clear();

                /*
                //Console.WriteLine("0~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~0"); // 1
                //Console.WriteLine("|                                                                                                                      |"); // 2
                //Console.WriteLine("|                                                                                                                      |"); // 3
                //Console.WriteLine("|                                                                                                                      |"); // 4
                //Console.WriteLine("|                                                                                                                      |"); // 5
                //Console.WriteLine("|                                                                                                                      |"); // 6
                //Console.WriteLine("|                                                                                 " + enemyIcon1.PadRight(30) + "       |"); // 7
                //Console.WriteLine("|                                                                                 " + enemyIcon2.PadRight(30) + "       |"); // 8
                //Console.WriteLine("|                                                                                 " + enemyIcon3.PadRight(30) + "       |"); // 9
                //Console.WriteLine("|                                                                                 " + enemyIcon4.PadRight(30) + "       |"); // 10
                //Console.WriteLine("|                                                                                 " + enemyIcon5.PadRight(30) + "       |"); // 11
                //Console.WriteLine("|                                                                                 " + enemyIcon6.PadRight(30) + "       |"); // 12
                //Console.WriteLine("|                                                                              " + badGuyName.PadRight(33) + "       |"); // 13
                //Console.WriteLine("|                                                                         enemy Health: " + badGuyHealth.ToString().PadRight(30) + " |"); // 14
                //Console.WriteLine("|                                                                                                                      |"); // 15
                //Console.WriteLine("|                                                                          " + badGuyAttack.PadRight(41) + "   |"); // 16
                //Console.WriteLine("|                                                                                                                      |"); // 17
                //Console.WriteLine("|                                                                                                                      |"); // 18
                //Console.WriteLine("|                                                                                                                      |"); // 19
                //Console.WriteLine("|                                                                                                                      |"); // 20
                //Console.WriteLine("|                                                                                                                      |"); // 21
                //Console.WriteLine("|                                                                                                                      |"); // 22
                //Console.WriteLine("|        Health: " + health.ToString().PadRight(34) + "                                                                    |"); // 23
                //Console.WriteLine("|         A.attacks                                                                                                    |"); // 24
                //Console.WriteLine("|         B.inventory                                                                                                  |"); // 25
                //Console.WriteLine("|                                                                                                                      |"); // 26
                //Console.WriteLine("|                                                                                                                      |"); // 27
                //Console.WriteLine("|                                                                                                                      |"); // 28
                //Console.WriteLine("0~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~0"); // 29
                */

                //do
                //{
                //    Console.WriteLine("waiting");
                //} while (waity < 0);
                while (Console.KeyAvailable == false)
                {
                    // running code
                    if (waity == 0)
                    {
                        Console.Clear();
                        badGuyAttack = "";
                        WriteTing(ref buffer, "                                               ", screen_size, new Vector2(64, 16));
                        Console.SetCursorPosition(0, 0);
                        Console.Write(buffer.ToString());
                    }
                    waity--;
                    Thread.Sleep(50);
                    x++;
                }
                c = Console.ReadKey(true);

                //Console.WriteLine("You pressed the '{0}' key.", c.Key);
                if (c.Key == ConsoleKey.A)
                {
                    Console.WriteLine("  === attacks ===");
                    Console.WriteLine("######################");

                    for (int i = 0; i < attacks.Length; i++)
                    {
                        Console.Write("## ".PadRight(3) + attacks[i].PadRight(5) + " ##");
                        // && i !> 0
                        if (((i + 1) % 2) == 0)
                        {
                            Console.Write("\n");
                            Console.WriteLine("######################");
                        }
                    }
                    //Thread.Sleep(5000);
                    Console.Write("what attack do you want to do: ");
                    string attack = Console.ReadLine();
                    //Console.WriteLine(attack);

                    switch (attack)
                    {
                        case "slash":
                            Console.Write("you do the slash");
                            //badGuyHealth -= 31;
                            Attack(31);
                            break;
                        case "jab":
                            Console.Write("you do the jab");
                            //badGuyHealth -= 20;
                            Attack(20);
                            break;
                        case "bow":
                            Console.Write("you shoot the bow");
                            //badGuyHealth -= 15;
                            Attack(15);
                            break;
                        case "other":
                            Console.Write("you do the other");
                            badGuyHealth -= 3;
                            Attack(3);
                            break;
                        case "exit":
                            Console.WriteLine("exit");
                            break;
                        default:
                            Console.WriteLine("incorrect input");
                            break;
                    }

                    Console.ReadLine();
                }
                else if (c.Key == ConsoleKey.D)
                {
                    Console.WriteLine("    ==== defence ====     ");
                    Console.WriteLine("##########################");
                    for (int i = 0; i < defence.Length; i++)
                    {
                        Console.Write("## ".PadRight(3) + defence[i].PadRight(5) + " ##");
                        // && i !> 0
                        if (((i + 1) % 2) == 0)
                        {
                            Console.Write("\n");
                            Console.WriteLine("######################");
                        }
                    }
                    Console.Write("how do you want to defend: ");
                    string defen = Console.ReadLine();
                    //Console.WriteLine(attack);

                    switch (defen)
                    {
                        case "dodge":
                            Console.Write("you do the dodge yuh");
                            //enemyAttack();
                            break;
                        case "block":
                            Console.Write("you do the block");
                            //enemyAttack();
                            break;
                        case "parry":
                            Console.Write("you parry");
                            //enemyAttack();
                            break;
                        case "other":
                            Console.Write("you do the other");
                            //enemyAttack();
                            break;
                        case "exit":
                            Console.WriteLine("exit");
                            break;
                        default:
                            Console.WriteLine("incorrect input");
                            break;
                    }
                    Console.ReadLine();
                }

                else if (c.Key == ConsoleKey.S)
                {
                    Console.Write("\n");
                    Console.WriteLine("    ==== inventory ====");
                    Console.WriteLine("##########################");

                    //Console.WriteLine("###        ######                 ###");
                    for (int i = 0; i < inventory.Length; i++)
                    {
                        bool result = int.TryParse(inventory[i], out int P);
                        if (result == false) // string
                        {
                            Console.Write(" # ".PadRight(3) + inventory[i].PadRight(13) + " ###");
                            Console.Write("\n");
                            Console.WriteLine("##########################");
                        }
                        else  //number
                        {
                            Console.Write("### ".PadRight(4) + inventory[i].PadRight(2));
                        }
                    }
                    Console.Write("what item do you want to use: ");
                    string inv = Console.ReadLine();
                    Console.WriteLine(inv);

                    switch (inv)
                    {
                        case "fish":
                            Console.WriteLine("you eat the fish");
                            InventoryNum("fish", -1);
                            break;
                        case "health potion":
                            Console.WriteLine("you drink potion");
                            InventoryNum("health potion", -1);
                            break;
                        case "rock":
                            Console.WriteLine("you rock");
                            InventoryNum("rock", -1);
                            break;
                        case "exit":
                            Console.WriteLine("exit");
                            break;
                        default:
                            Console.WriteLine("incorrect input");
                            break;
                    }

                    // item stuff
                    Console.ReadLine();

                }



                if (c.Key == ConsoleKey.P)
                {
                    Console.WriteLine("you have a reaction time of " + x + " seconds");
                    Console.WriteLine("yuh");
                    Console.ReadLine();

                    Console.Clear();
                }





            } while (gameRunning == true);
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }
        //Dodging();

        //code for dodging - returns an int depending on how much damage taken 0 dead 1 survived 2 perfect -1 something went wrong
        static int Dodging(int playerHealth, int damagePerHit)
        {


            Console.CursorVisible = false;

            // 10 rows by 31 columns grid.
            // 0 = empty, 1 = obstacle present. 
            int[,] grid = new int[10, 31];
            int loopCounter = 0, playerPos = 15;
            int score = 0;


            int gridXLimit = 31, gridYLimit = 9;
            int xNumber = 3;
            bool isRunning = true;
            Random rand = new();

            while (isRunning)
            {
                // Input Control 
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.A && playerPos > 0)
                        playerPos--;
                    if (key == ConsoleKey.D && playerPos < 30)
                        playerPos++;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                }
                // Game Logic: Move obstacles down (Iterating TOP to BOTTOM to prevent double-moving) 
                for (int i = gridYLimit; i >= 0; i--)
                {
                    for (int x = 0; x < gridXLimit; x++)
                    {
                        if (grid[i, x] == 1)
                        {
                            grid[i, x] = 0; // Clear current position 
                            if (i < gridYLimit)
                            {
                                grid[i + 1, x] = 1; // Move down one row safely
                            }
                            else
                            {
                                score++; // Cleared the bottom row successfully
                                if ((score % 50 == 0) && (score < 150))
                                {
                                    xNumber--;
                                }
                                if (score == 150)
                                {
                                    isRunning = false;
                                }
                            }
                        }
                    }
                }
                // Spawn Logic (drop an X at row 0)
                if (score < 141)
                {
                    loopCounter++;
                    if (loopCounter % xNumber == 0)
                    {
                        int spawnPos = rand.Next(0, gridXLimit);
                        grid[0, spawnPos] = 1;
                    }
                }
                // Collision Detection (Player is always on row 9) 
                if (grid[9, playerPos] == 1)
                {
                    Console.Beep();
                    playerHealth -= damagePerHit;
                    Thread.Sleep(100);

                    if (playerHealth <= 0)
                    {
                        isRunning = false;
                    }
                    else
                    {
                        grid[9, playerPos] = 0; // Clear the obstacle so it doesn't instantly hit again
                        score += 1;
                    }
                }
                else
                {
                    // Rendering What will be displayed on the screen
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"X's missed: {score}\t\tHealth: {playerHealth}\n----------------------------------");

                    for (int i = 0; i < gridYLimit + 1; i++)
                    {
                        for (int j = 0; j < gridXLimit; j++)
                        {
                            if (i == gridYLimit && j == playerPos)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("P");
                            }
                            else if (grid[i, j] == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("X");
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        Console.WriteLine();
                    }
                    Thread.Sleep(100);
                }
            }

            // Game Over Screen
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            return playerHealth;
            if (playerHealth >= 3)
            {
                Console.WriteLine($"You did very bad!\n\nPress Enter to exit...");
                return 0;
            }
            else if ((playerHealth > 0) && (playerHealth < 3))
            {
                Console.WriteLine($"You Survived! Score: {score}\n\nPress Enter to exit...");
                return 1;
            }
            else if (playerHealth == 0)
            {
                Console.WriteLine($"Perfect Dodge! Score: {score} Health: {playerHealth}\n\nPress Enter to exit...");
                return 2;
            }
            Console.ReadLine();
            return -1;
        }



        //Game instruction here 
        static void Wait()
        {
            //Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Color_Helper(17, false));
            //Console.Clear();

            // this line is the ANSI Escape sequence for clearing the console, it is needed as the regular console.clear() removes the formatting causing color differences.
            Console.Write("\x1b[2J");
            //this sets the cursor position to 1,1
            Console.Write("\x1b[H");

            Console.WriteLine("A violent storm swallows the sea.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(2000);
            Console.WriteLine("You - A simple fisherman, out alone on a late-night fishing trip. What began as a calm evening quickly turns into chaos as dark clouds cover the sky and enormous waves crash against your boat.");
            Console.WriteLine();
            Thread.Sleep(2000);
            Console.WriteLine("Just as you try to turn back, a massive wave smashes into the ship.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(3000);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine(".....");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("                                                Everything goes black                                               ");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();

        }

        static void GameIntro()
        {
            Console.Clear();
            Console.Write(Color_Helper(231, false));
            Console.Write(Color_Helper(16, true));
            //Console.Clear();

            // this line is the ANSI Escape sequence for clearing the console, it is needed as the regular console.clear() removes the formatting causing color differences.
            Console.Write("\x1b[2J");
            //this sets the cursor position to 1,1
            Console.Write("\x1b[H");

            Console.WriteLine();
            Console.WriteLine("When you finally wake up, the sound of the ocean is the only thing you hear.");
            Console.WriteLine("You find yourself lying on a strange shore surrounded by broken wood and wreckage. A thick fog covers the sea, and in the distance sits a small unfamiliar village.");
            Thread.Sleep(3500);
            Console.WriteLine();
            Console.WriteLine("\"Are you okay?\"");
            Thread.Sleep(1000);
            Console.WriteLine();
            Console.WriteLine("\"You've finally returned.\"");
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("You open your eyes, and the villagers are stare at you as if they already know who you are.");
            Console.WriteLine("But you have never been here before.");
            Console.WriteLine();

            Thread.Sleep(4000);
            Console.WriteLine("As the day pass...");
            Thread.Sleep(1500);
            Console.WriteLine("The island begins to feel wrong.");
            Thread.Sleep(1500);
            Console.WriteLine();
            Console.WriteLine("Nobody talks about leaving the island.");
            Thread.Sleep(1500);
            Console.WriteLine();
            Console.WriteLine("Some villagers repeat the exact same phrases every day.");
            Thread.Sleep(1500);
            Console.WriteLine();
            Console.WriteLine("The sea is always hidden behind heavy fog.");
            Thread.Sleep(1500);
            Console.WriteLine();
            Console.WriteLine("Strange whispers can be heard near the shore at night.");
            Thread.Sleep(1500);
            Console.WriteLine();
            Console.WriteLine("In the center of the village stands a gravestone...");
            Thread.Sleep(2500);
            Console.WriteLine();
            Console.WriteLine(".....with YOUR NAME on it.");
            Thread.Sleep(3000);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("                                    Find the truth.                                 ");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("                          Before the island claims you forever.                     ");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

        }
        // Player gets the Truth Fragment 1 When fishing
        static void Truth()
        {
            string truth2;
            Console.WriteLine("\".....Where did you catch that?\"");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"....That's impossible...\"");
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"These fish only appear to people who still remember.\"");
            Console.WriteLine();
            Console.WriteLine("\"The truth the island wants us to forget.\"");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"There was never any storm.\"");
            Thread.Sleep(1000);
            Console.WriteLine("\"You weren't the first person to arrive here.\"");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"Take this. If you want to know something...\"");
            Thread.Sleep(1500);
            Console.WriteLine("*You received a TRUTH FRAGMENT*");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("*WOULD YOU WANT TO OPEN IT KNOW (YES/NO)*");
            truth2 = Console.ReadLine();
            if (truth2 == "yes")
            {
                Thread.Sleep(1000);
                Console.WriteLine("*YOU ARE NOT IN THE REAL WORLD*");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Press Enter to close it");
                Console.ReadLine();

            }

            Console.WriteLine("\"...I can only tell this much.\"");
            Thread.Sleep(1000);
            Console.WriteLine("You ask for more details");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"...Go to the village shop.\"");
            Thread.Sleep(500);
            Console.WriteLine("\"She pretend not to know everything\"");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("\"But she keeps things people were never supposed to see.\"");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }

        // Player gets the Truth Fragment 3 in the forest or something
        static void fragment()
        {
            string truth3;
            Console.WriteLine("*You received a TRUTH FRAGMENT*");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1000);
            Console.WriteLine("*WOULD YOU WANT TO OPEN IT KNOW (YES/NO)*");
            truth3 = Console.ReadLine();
            if (truth3 == "yes")
            {
                Thread.Sleep(1000);
                Console.WriteLine("*YOU ALREADY DIED*");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Press Enter to close it");
                Console.ReadLine();
            }

        }

    }
}
