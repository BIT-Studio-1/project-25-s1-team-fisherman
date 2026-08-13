

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using test_fish;
using static System.Net.Mime.MediaTypeNames;
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
        public static int coins = 0;
        public static bool memory1 = false;
        public static bool viewed_memory1 = false;
        public static bool memory2 = false;
        public static bool viewed_memory2 = false;
        public static bool memory3 = false; // this is the memory you get for fighting
        public static int enemies_killed = 0;
        public static bool show_intro = true;

        static void Main(string[] args)
        {

            Random rand = new Random();
            //this is a string builder that makes it easy to edit the console output. it works like an a array 
            StringBuilder buffer = new StringBuilder();
            //Colored buffer just for display, don't use for any checks as To_Index wont work on it, holds the color codes as well so is longer than buffer 
            StringBuilder color_buffer = new StringBuilder();


            Console.CursorVisible = false;
            Console.Title = "Dead Tide";
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
            if (show_intro)
            {
                Wait();
                GameIntro();
            }
            Console.Write(RESET);
            Console.Clear();
            Console.Write("\x1b[3j");
            //Console.Clear();
            //Console.ReadLine();


            while (true)
            {
                path = Get_Path(map_tile, path, out map_changed);
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
                    Inventory_Menu();
                    break;
                case ConsoleKey.H:
                    Key_Binds();
                    break;

            }

            Vector2 clamped_offset = Vector2.Clamp(offset, Vector2.Zero, new Vector2(118, 27));
            map_tile = clamped_offset - offset;
            return clamped_offset;
        }
        static void Key_Binds()
        {
            Console.Write(RESET);
            Console.Clear();
            Console.Write("\x1b[3j");
            Console.WriteLine("Key Binds");
            Console.WriteLine($"{"Move",-15} WASD");
            Console.WriteLine($"{"Inventory",-15} I");
            Console.WriteLine($"{"Menu",-15} ESC");
            Console.WriteLine($"{"Start Fish Game",-15} Space");
            Console.WriteLine();
            Console.WriteLine("To Interact with stuff walk into it");
            Console.WriteLine("The goal is to find the truth, The say that you can find that in the sea, the Shop Keeper, the forest");

            Console.WriteLine("Press Enter To Exit");

            Console.ReadLine();
        }

        //up 
        //start right
        //down
        static string Get_Path(Vector2 map_pos, string current, out bool map_changed)
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
            char c = buffer[To_Index(coords, size)];
            switch (c)
            {
                case '#':
                case '+':
                    return false;
                case '~':
                    Console.Clear();
                    Fishing();
                    return false;
                case 'S':
                    Console.Clear();
                    Shopping();
                    return false;
                case 'm':
                    Console.Clear();
                    Fighting();
                    return false;
                case 'T':
                    fragment();
                    return false;
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
        //╥
        static int Get_Item_Count(string name)
        {
            int count = 0;
            inventory.TryGetValue(name, out count);

            return count;
        }
        static string Get_Item_Name(int index)
        {
            int c = 0;
            foreach (string item in inventory.Keys)
            {
                if (c == index)
                    return item;
                c++;
            }

            return "";
        }
        static void Add_Item(string name, int count)
        {
            name = name.ToLower();
            if (inventory.ContainsKey(name))
            {
                inventory[name] += count;
            }
            else
            {
                inventory.Add(name, count);
            }
        }
        static void Remove_Item(string name, int count)
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
        static string[] Display_Inventory()
        {
            string[] buffer = Array.Empty<string>();
            foreach (KeyValuePair<string, int> pair in inventory)
            {
                string item = pair.Key;
                string count = pair.Value.ToString();
                string line = item.PadRight(20) + ": " + count;

                buffer = buffer.Append(line).ToArray();
            }
            return buffer;
        }

        static void Inventory_Menu()
        {
            while (true)
            {
                string[] inv = Display_Inventory();
                Console.Clear();
                Console.WriteLine("Inventory");
                Console.WriteLine("=========");
                foreach (string item in inv)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine();
                Console.WriteLine($"Coins: {coins}");
                Console.WriteLine("Press enter to exit");
                string add = Console.ReadLine();
                if (add != "add" && add != "remove")
                    break;
                Console.WriteLine("item");
                string item_name = Console.ReadLine().Trim();
                Console.WriteLine("count");
                string count = Console.ReadLine();
                if (count != null && item_name != null)
                {
                    int int_count = Convert.ToInt32(count);
                    if (add == "add")
                    {
                        Add_Item(item_name, int_count);
                    }
                    else if (add == "remove")
                    {
                        Remove_Item(item_name, int_count);
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
            Vector2 play_pos = new Vector2(45, 24);
            Vector2 exit_pos = new Vector2(73, 24);
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

                buffer.Append(Color_Helper(196, true));
                foreach (string line in File.ReadLines("Map/menu.txt"))
                {
                    foreach (char c in line)
                    {
                        if (c == '@')
                        {
                            buffer.Append(Color_Helper(255, true) + " ");
                        }
                        else
                        {
                            buffer.Append(c);
                        }
                    }
                    buffer.Append("\n");
                }


                current = Vector2.Clamp(current, play_pos, exit_pos);

                //adds an underline to the current selection by stepping through the array and changing the characters to '#' for the length of the text.

                for (int x = (int)current.X; x < current.X + 23; x++)
                {
                    buffer[To_Index(new Vector2(x + 11, 24), screen_size)] = '#';
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
                    case ConsoleKey.H:
                        Key_Binds();
                        break;
                    case ConsoleKey.S:
                        show_intro = !show_intro;
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
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
            Fishing_Base.coins = coins;
            Fishing_Base.Fishing();
            coins = Fishing_Base.coins;
            memory1 = Fishing_Base.memory;
            Thread.Sleep(500);
            Console.Clear();
        }
        static void Shop()
        {
            string buy;

            int[] p = { 0 };
            Dictionary<string, int> items = new Dictionary<string, int>
            {
                {"health potion", 15 },
                {"fish bait", 5 },
                {"jar of dirt", 1},
                {"protective charm", 100 },
                {"truth", 50 },
                {"exit", 0 }

            };
            //Used to covert the first leter of each word to uppercase
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            while (true)
            {
                int count = 0;
                Console.WriteLine("\"Looking for any supplies?\"");
                Console.WriteLine("========================BUY SYSTEM=========================");

                foreach (KeyValuePair<string, int> item in items)
                {
                    Console.Write(count.ToString().PadRight(10));
                    Console.Write(textInfo.ToTitleCase(item.Key).PadLeft(15));
                    Console.WriteLine(item.Value.ToString().PadLeft(25));
                    count++;
                }

                Console.WriteLine($"You have {coins} coins");
                Console.WriteLine();
                Thread.Sleep(1000);
                Console.WriteLine("\"The sea is dangerous after dark.\"");
                Console.WriteLine("\"Buy what you need before heading out.\"");
                buy = Console.ReadLine()?.ToLower() ?? "";
                switch (buy)
                {
                    case "0":
                    case "health potion":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= items["health potion"] * count;
                        if (coins > 0)
                        {
                            Add_Item("health potion", count);
                        }
                        else
                        {
                            coins += items["health potion"] * count;
                        }

                        break;
                    case "1":
                    case "fish bait":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= items["fish bait"] * count;
                        if (coins > 0)
                        {
                            Add_Item("fish bait", count);
                        }
                        else
                        {
                            coins += items["fish bait"] * count;
                        }
                        break;
                    case "2":
                    case "jar of dirt":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= items["jar of dirt"] * count;
                        if (coins > 0)
                        {
                            Add_Item("Jar of Dirt", count);
                        }
                        else
                        {
                            coins += items["jar of dirt"] * count;
                        }
                        break;
                    case "3":
                    case "protective charm":
                        Console.WriteLine("\"\"");
                        Console.WriteLine("How many");
                        count = Convert.ToInt32(Console.ReadLine());
                        coins -= items["protective charm"] * count;
                        if (coins > 0)
                        {
                            Add_Item("Protective Charm", count);
                        }
                        else
                        {
                            coins += items["protective charm"] * count;
                        }
                        break;
                    case "4":
                    case "truth":
                        Console.WriteLine("\"\"");
                        coins -= p[4];
                        if (coins > 0)
                        {
                            Add_Item("Truth", 1);
                        }
                        else
                        {
                            coins += p[4];
                        }
                        break;
                    case "5":
                    case "exit":
                        return;
                }



            }

        }

        //Put the code for shopping in here.
        static void Shopping()
        {
            //Shop();
            string shop, check, check1, truth1;

            string[] c = { "Buy", "Talk", "Exit", "Leave" };
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
                case "0":
                case "buy":
                    Shop();
                    break;

                case "1":
                case "talk":
                    if (memory1 && !viewed_memory1)
                    {
                        Truth();
                        viewed_memory1 = true;
                        break;
                    }

                    Console.WriteLine();
                    Console.WriteLine("\"So, what's you want to talk to me?\"");
                    check1 = Console.ReadLine();
                    Console.WriteLine("\"Is this the first time we talk about this?*(YES/NO)*\"");
                    check = Console.ReadLine().ToLower().Trim();

                    if (check == "yes")
                    {
                        Console.WriteLine("\"So, you want to know about what?\"");
                        Thread.Sleep(500);
                        Console.WriteLine("Truth");
                        Thread.Sleep(1000);
                        //talk1 = Console.ReadLine();

                        if (Get_Item_Count("Truth") != 0)
                        {
                            memory2 = true;
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
                            Console.WriteLine("*WOULD YOU WANT TO OPEN IT (YES/NO)*");
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
                        else
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
                    }
                    else
                    {
                        Console.WriteLine("So you have nothing to say.");
                        Console.WriteLine("Then get out of here.");
                        Console.ReadLine();
                    }


                    break;
                case "2":
                case "Exit":
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to exit");
                    Console.ReadLine();
                    break;
                case "3":
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

        static void Write_Ting(ref StringBuilder buff, string guy, Vector2 screen_size, Vector2 pos)
        {
            int count = 0;
            foreach (char h in guy)
            {
                buff[To_Index(new Vector2(count + pos.X, pos.Y), screen_size)] = h;
                count++;
            }
        }
        static void Fish_Draw(ref StringBuilder buff, int lines, List<string> Draw, int X, int Y)
        {
            for (int i = 0; i < lines; i++)
            {
                Write_Ting(ref buff, Draw[i], screen_size, new Vector2(X, Y + i));
            }
        }
        static void Fish_Sprite_Draw(ref StringBuilder buff, int lines, string Draw, int X, int Y)
        {
            for (int i = 0; i < lines; i++)
            {
                Write_Ting(ref buff, Draw, screen_size, new Vector2(X, Y + i));
            }
        }
        public static bool Fishy_Game() //function for the fish mini game
        {
            List<string> Fish_Sprite = new List<string>// fish sprite
            {
                @" _              ",
                @"\ \ ____|\____  ",
                @" \ /        o \ ",
                @"  (   ||       )",
                @" / \__________/ ",
                @"/_/             "
            };

            int fish_x = 30; // X position of the fish
            int bar_x = 30; // X position of the bar
            int waity = 200; // a way for the program to slow down
            const string fish_bar_size = "#############################"; // the size of the bar
            int fish_dih = 1; // fish direction 
            int fish_score = 100; // how close the fish is to being caught
            bool fish_game_running = true; // if the game is running or not

            string Print_Line() // prints the progress bar
            {
                fish_score = Math.Max(0, fish_score);
                string line = "";
                for (int i = 0; i <= fish_score / 4; i++)
                {
                    line += "+";
                }
                return line;
            }

            void Fish_Move() // moves the fish
            {
                if (fish_x > bar_x && fish_x < bar_x + 29 && fish_score < 453) // checks if the fish is in the bar or not and then adjusts the score
                {
                    fish_score += 2;
                }
                else // this takes away your score when the bar isn't on the sprite
                {
                    fish_score--;
                }

                Random random = new Random();
                if (random.Next(0, 15) == 1) // randomly picks what way the fish goes
                {
                    fish_dih = fish_dih * -1;
                }

                if (fish_x > 6 && fish_dih == -1) // swaps the direction the fish sprite faces
                {
                    fish_x += fish_dih;
                    Fish_Sprite[0] = @"              _ ";
                    Fish_Sprite[1] = @"  ____/|____ / /";
                    Fish_Sprite[2] = @" / o        \ / ";
                    Fish_Sprite[3] = @"(      ||    )  ";
                    Fish_Sprite[4] = @" \__________/ \ ";
                    Fish_Sprite[5] = @"             \_\";
                }
                else if (fish_x < 98 && fish_dih == 1)
                {
                    fish_x += fish_dih;
                    Fish_Sprite[0] = @" _              ";
                    Fish_Sprite[1] = @"\ \ ____|\____  ";
                    Fish_Sprite[2] = @" \ /        o \ ";
                    Fish_Sprite[3] = @"  (   ||       )";
                    Fish_Sprite[4] = @" / \__________/ ";
                    Fish_Sprite[5] = @"/_/             ";
                }
            }

            do
            {
                StringBuilder buffer = new StringBuilder(); // creates the string buffer 

                if (fish_score <= 1) // ends the fish game if fishScore less or equal then one
                {
                    fish_game_running = false;
                    return false;
                }

                buffer.Clear(); // clears the buffer
                Console.SetCursorPosition(0, 0); // this prevents some flickering 

                // draws the backdrop for the fishing mini game
                foreach (string line in File.ReadLines("Map/fighting/fishMap.txt")) // loops through each line in a txt file
                {
                    foreach (char p in line) //this loops across the lines
                    {
                        buffer.Append(p); // this adds each char into a StringBuilder
                    }
                    buffer.Append("\n"); // this starts the new line
                }

                Write_Ting(ref buffer, "completion bar:", screen_size, new Vector2(4, 13));
                Write_Ting(ref buffer, fish_score.ToString(), screen_size, new Vector2(20, 15));
                Write_Ting(ref buffer, Print_Line(), screen_size, new Vector2(2, 14));

                Fish_Sprite_Draw(ref buffer, 8, fish_bar_size, bar_x, 20); // this draw the fish bar

                Fish_Draw(ref buffer, 6, Fish_Sprite, fish_x, 21); // this draws the fish sprite
                Console.Clear();
                Console.Write(buffer.ToString());

                ConsoleKeyInfo c = new ConsoleKeyInfo();
                c = Console.ReadKey(true);

                while (Console.KeyAvailable == false)
                {
                    Thread.Sleep(25);

                    buffer.Clear(); // this clears the buffer
                    foreach (string line in File.ReadLines("Map/fighting/fishMap.txt")) // loops through txt file
                    {
                        foreach (char p in line) //each line
                        {
                            buffer.Append(p);
                        }
                        buffer.Append("\n");
                    }

                    Fish_Move();//calls fish move 
                    Write_Ting(ref buffer, "completion bar:", screen_size, new Vector2(4, 13));
                    Write_Ting(ref buffer, Print_Line(), screen_size, new Vector2(2, 14));
                    Write_Ting(ref buffer, fish_score.ToString(), screen_size, new Vector2(20, 15));

                    //this clears the area where the fish might be
                    Fish_Sprite_Draw(ref buffer, 6, "                     ", fish_x, 21);

                    Fish_Sprite_Draw(ref buffer, 8, fish_bar_size, bar_x, 20);

                    Fish_Draw(ref buffer, 6, Fish_Sprite, fish_x, 21);

                    if (fish_score < 1) // this checks if the fishScore is less then one and if so it ends the program and returns false
                    {
                        fish_game_running = false;
                        return false;
                    }

                    Console.SetCursorPosition(0, 0);
                    Console.Write(buffer.ToString()); // prints the buffer
                    if (bar_x < 90)
                    {
                        buffer.Clear();
                        foreach (string line in File.ReadLines("Map/fighting/fishMap.txt")) // loops through txt file
                        {
                            foreach (char p in line) //each line
                            {
                                buffer.Append(p);
                            }
                            buffer.Append("\n");
                        }
                        Write_Ting(ref buffer, "completion bar:", screen_size, new Vector2(4, 13));
                        Write_Ting(ref buffer, Print_Line(), screen_size, new Vector2(2, 14));
                        Write_Ting(ref buffer, fish_score.ToString(), screen_size, new Vector2(20, 15));

                        Fish_Sprite_Draw(ref buffer, 6, "                     ", bar_x, 21); //clears where the fish might be

                        bar_x += 2; // this slowly moves the bar back to its starting point

                        Fish_Sprite_Draw(ref buffer, 8, fish_bar_size, bar_x, 20); //this draws the fish bar on screen

                        Fish_Draw(ref buffer, 6, Fish_Sprite, fish_x, 21); //this draws the fish on screen

                        Console.SetCursorPosition(0, 0);
                        Console.Write(buffer.ToString()); // this writes the buffer to the screen
                        waity = 1000;
                    }
                    else
                    {
                        waity--;
                    }
                }
                waity--;

                Fish_Move();
                if (Console.KeyAvailable)
                {
                    if (c.Key == ConsoleKey.Spacebar)
                    {
                        waity = 200;
                        for (int i = 0; i != 20; i++)
                        {
                            if (bar_x > 2)
                            {
                                bar_x -= 1;
                            }
                        }
                    }
                }

                if (fish_score > 452) // win condition 
                {
                    fish_game_running = false;
                    return true;
                }

            } while (fish_game_running == true);
            return true;
        }

        static void Fighting()
        {
            static void WriteTing(ref StringBuilder buff, string guy, Vector2 screen_size, Vector2 pos)
            {
                int count = 0;
                foreach (char h in guy)
                {
                    buff[To_Index(new Vector2(count + pos.X, pos.Y), screen_size)] = h;
                    count++;
                }
            }

            string[] inventory = { "23", "fish", "2", "health potion", "56", "rock" };
            string[] attacks = { "0 slash (31 damage)", "1 jab (20 - 40 damage)","2 Exit", "3 bow(25 - 36 damage)", " 4 poison (10 damage per round) "};
            string[] defence = { "block", "dodge", "parry", "other" };

            ConsoleKeyInfo c = new ConsoleKeyInfo();

            Random random = new Random();
            int x = 0;
            int player_speed = 10;

            int bad_guy_health = 100;
            int bad_guy_speed = 12;
            string bad_guy_name = "evil guy of doom";
            string bad_guy_attack = "";
            int bad_guy_damage = 10;

            int health = 100;
            bool game_running = true;
            int waity = -1;
            int poison_damage = 0;

            bool protection = false;

            void alive()
            {
                if (bad_guy_health <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("the bad guy is dead you win");
                    enemies_killed++;
                    Console.ReadLine();
                    if (enemies_killed > 2)
                    {
                        // code for memory
                        Console.WriteLine("You got the last memory");
                        Console.ReadLine();
                        memory3 = true;
                    }
                    Console.WriteLine("");
                    game_running = false;
                }
                else if (health <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("you are dead you lose");
                    Console.WriteLine("");
                    Console.ReadLine();
                    game_running = false;
                    Environment.Exit(0); // closes the console window
                }
            }
            // the enemies attack  //code for dodging - returns an int depending on how much damage taken 0 dead 1 survived 2 perfect -1 something went wrong
            void Attack(int player_damg)
            {
                bad_guy_health -= poison_damage;

                if (player_damg == -1)
                {
                    poison_damage += 10;
                    player_damg = 0;
                }

                int damage = 0; //random.Next(20, 40);
                if (player_speed >= bad_guy_speed)
                {
                    bad_guy_health -= playerAttack(player_damg);
                    alive();
                    health -= enemyAttack();
                    alive();
                }
                else
                {
                    health -= enemyAttack();
                    alive();
                    bad_guy_health -= playerAttack(player_damg);
                    alive();
                }
            }

            static int playerAttack(int player_damg)
            {
                return player_damg;
            }

            int enemyAttack()
            {
                //damage !!!
                Random random = new Random();
                int damage = 0; //random.Next(20, 40);
                damage = Dodging(health, bad_guy_damage);
                health = health - damage;
                bad_guy_attack = bad_guy_name + " does " + damage + " damage";
                waity = 100;
                return damage;
            }

            StringBuilder buffer = new StringBuilder();
            bool in_menu = true;
            Vector2 screen_size = new Vector2(120, 28);
            Vector2 play_pos = new Vector2(34, 24);
            Vector2 exit_pos = new Vector2(62, 24);
            Vector2 current = play_pos;
            string enemy_icon1 = "";
            String enemy_icon2 = "";
            string enemy_icon3 = "";
            string enemy_icon4 = "";
            string enemy_icon5 = "";
            string enemy_icon6 = "";

            int rand_enemy = random.Next(0, 5); //randomly picks a enemy for the player to fight

            switch (rand_enemy)
            {
                case 0:
                    bad_guy_health = 100;
                    bad_guy_speed = 12;
                    bad_guy_damage = 15;
                    bad_guy_name = "evil cat";

                    enemy_icon1 = @"              ";
                    enemy_icon2 = @" (\___/)      ";
                    enemy_icon3 = @" | >:) |    / ";
                    enemy_icon4 = @"  -----    /  ";
                    enemy_icon5 = @"   |_____ /   ";
                    enemy_icon6 = @"   /\    /\   ";
                    break;
                case 1:
                    bad_guy_health = 80;
                    bad_guy_speed = 8;
                    bad_guy_damage = 10;
                    bad_guy_name = "goblin";

                    enemy_icon1 = @"   _/\___/\_  ";
                    enemy_icon2 = @"  |  @ __ @ | ";
                    enemy_icon3 = @"  |_________| ";
                    enemy_icon4 = @"   /|.   .|\  ";
                    enemy_icon5 = @"  / |_____| \ ";
                    enemy_icon6 = @"    |     |   ";
                    break;
                case 2:
                    bad_guy_health = 120;
                    bad_guy_speed = 18;
                    bad_guy_damage = 20;
                    bad_guy_name = "thing";

                    enemy_icon1 = @" ( | )      ( | )";
                    enemy_icon2 = @"    ||______||   ";
                    enemy_icon3 = @"   /         \   ";
                    enemy_icon4 = @"  | )-----(   |  ";
                    enemy_icon5 = @"   \_________/   ";
                    enemy_icon6 = @"   / / | | \ \   ";
                    break;
                case 3:
                    bad_guy_health = 200;
                    bad_guy_speed = 4;
                    bad_guy_damage = 30;
                    bad_guy_name = "zombie";

                    enemy_icon1 = @"   _\__|__/_   ";
                    enemy_icon2 = @"  |(o\ O /.)|  ";
                    enemy_icon3 = @"  |_________|  ";
                    enemy_icon4 = @"   /|     |\   ";
                    enemy_icon5 = @"  / |_____| \  ";
                    enemy_icon6 = @"    |  |  |    ";
                    break;
                case 4:
                    bad_guy_health = 50;
                    bad_guy_speed = 1000;
                    bad_guy_damage = 100;
                    bad_guy_name = "worm of doom";

                    enemy_icon1 = @"                ";
                    enemy_icon2 = @"                ";
                    enemy_icon3 = @"         \      ";
                    enemy_icon4 = @"         /      ";
                    enemy_icon5 = @"                ";
                    enemy_icon6 = @"                ";
                    break;
            }

            do // the main loop for the fighting
            {
                if (protection)
                {
                    bad_guy_damage = bad_guy_damage / 2;
                }
                // @@@
                buffer.Clear();
                Console.SetCursorPosition(0, 0);

                foreach (string line in File.ReadLines("Map/fighting/FightingMenu.txt")) // loops through txt file
                {
                    foreach (char p in line) //each line
                    {
                        buffer.Append(p);
                    }
                    buffer.Append("\n");
                }

                //prints the enemy on the screen
                WriteTing(ref buffer, enemy_icon1, screen_size, new Vector2(66, 7));
                WriteTing(ref buffer, enemy_icon2, screen_size, new Vector2(66, 8));
                WriteTing(ref buffer, enemy_icon3, screen_size, new Vector2(66, 9));
                WriteTing(ref buffer, enemy_icon4, screen_size, new Vector2(66, 10));
                WriteTing(ref buffer, enemy_icon5, screen_size, new Vector2(66, 11));
                WriteTing(ref buffer, enemy_icon6, screen_size, new Vector2(66, 12));

                //prints other UI elements
                WriteTing(ref buffer, bad_guy_name, screen_size, new Vector2(66, 13));
                WriteTing(ref buffer, "enemy health: " + bad_guy_health, screen_size, new Vector2(66, 14));

                //WriteTing(ref buffer, "0====================0", screen_size, new Vector2(66, 14));
                WriteTing(ref buffer, bad_guy_attack, screen_size, new Vector2(64, 16));
                WriteTing(ref buffer, "health: " + health, screen_size, new Vector2(10, 22));


                Console.Clear();
                Console.Write(buffer.ToString()); // writes the thing

                while (Console.KeyAvailable == false)// if there is no key input
                {
                    if (waity == 0)
                    {
                        Console.Clear();
                        bad_guy_attack = "";
                        WriteTing(ref buffer, "                                               ", screen_size, new Vector2(64, 16));
                        Console.SetCursorPosition(0, 0);
                        Console.Write(buffer.ToString());
                    }
                    waity--;
                    Thread.Sleep(50);
                    x++;
                }
                c = Console.ReadKey(true);

                if (c.Key == ConsoleKey.Z) // if the A key is pressed
                {
                    //Console.WriteLine("               === attacks ===");
                    //Console.WriteLine("#################################################################");

                    WriteTing(ref buffer, "=== attacks ===", screen_size, new Vector2(24, 19));
                    WriteTing(ref buffer, "#################################################################", screen_size, new Vector2(8, 20));
                    Console.Clear();
                    Console.Write(buffer.ToString());
                    int fightingUIx = 8;
                    int fightingUIy = 21;
                    for (int i = 0; i < attacks.Length; i++)
                    {
                        //Console.Write("## ".PadRight(3) + attacks[i].PadRight(5) + " ##");
                        WriteTing(ref buffer, "## ".PadRight(3) + attacks[i].PadRight(5) + " ##", screen_size, new Vector2(fightingUIx, fightingUIy));
                        fightingUIx += ("## ".PadRight(3) + attacks[i].PadRight(5) + " ##").Count();

                        if (((i + 1) % 3) == 0)
                        {
                            fightingUIx = 8;

                            //Console.WriteLine("#################################################################");
                            fightingUIy++;
                            WriteTing(ref buffer, "#################################################################", screen_size, new Vector2(fightingUIx, fightingUIy));
                            fightingUIy++;
                        }
                    }
                    //Console.WriteLine("#################################################################");
                    WriteTing(ref buffer, "#################################################################", screen_size, new Vector2(8, fightingUIy+1));

                    //WriteTing(ref buffer, "what attack do you want to do: ", screen_size, new Vector2(20, 24));
                    Console.Clear();
                    Console.Write(buffer.ToString());
                    Console.Write("what attack do you want to do: ");
                    string attack = Console.ReadLine();

                    WriteTing(ref buffer, "0==============================0", screen_size, new Vector2(80, 21));
                    WriteTing(ref buffer, "0==============================0", screen_size, new Vector2(80, 23));
                    WriteTing(ref buffer, "|", screen_size, new Vector2(80, 22));
                    WriteTing(ref buffer, "|", screen_size, new Vector2(111, 22));
                    switch (attack)
                    {
                        case "0":
                        case "slash":
                            //Console.Write("you do the slash");
                            WriteTing(ref buffer, "you do the slash", screen_size, new Vector2(85, 22));
                            Console.Clear();
                            Console.Write(buffer.ToString());
                            Attack(31);
                            break;
                        case "1":
                        case "jab":
                            //Console.Write("you do the jab");
                            WriteTing(ref buffer, "you do the jab", screen_size, new Vector2(85, 22));

                            Console.Clear();
                            Console.Write(buffer.ToString());
                            Attack(random.Next(21, 41));
                            break;
                        case "3":
                        case "bow":
                            //Console.Write("you shoot the bow");
                            WriteTing(ref buffer, "you shoot the bow", screen_size, new Vector2(85, 22));
                            Console.Clear();
                            Console.Write(buffer.ToString());
                            Attack(random.Next(25, 36));
                            
                            break;
                        case "4":
                        case "poison":
                            if (poison_damage != 0)
                            {
                                //Console.WriteLine("the bad guy was already poisoned");
                                //Console.Write("but you throw the poison anyway and make it stronger");

                                WriteTing(ref buffer, "you throw the poison again", screen_size, new Vector2(85, 22));
                            }
                            else
                            {
                                //Console.Write("you throw the poison");
                                WriteTing(ref buffer, "you throw the poison", screen_size, new Vector2(85, 22));
                            }
                            Console.Clear();
                            Console.Write(buffer.ToString());
                            Attack(-1);
                            break;
                        case "2":
                        case "exit":
                            //Console.WriteLine("exit");
                            WriteTing(ref buffer, "exit", screen_size, new Vector2(85, 22));
                            Console.Clear();
                            Console.Write(buffer.ToString());
                            Thread.Sleep(1000);

                            break;
                        default:
                            //Console.WriteLine("incorrect input");
                            WriteTing(ref buffer, "incorrect input", screen_size, new Vector2(85, 22));
                            Console.Clear();
                            Console.Write(buffer.ToString());
                            Thread.Sleep(1000);
                            break;
                    }
                    Console.Clear();
                    Console.Write(buffer.ToString());
                }
                /*
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
                    switch (defen)
                    {
                        case "dodge":
                            Console.Write("you do the dodge yuh");
                            break;
                        case "block":
                            Console.Write("you do the block");
                            break;
                        case "parry":
                            Console.Write("you parry");
                            break;
                        case "other":
                            Console.Write("you do the other");
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
                */
                else if (c.Key == ConsoleKey.X) // if the D key is pressed
                {
                    //Console.Write("\n");

                    //Console.WriteLine("    ==== inventory ====");
                    //Console.WriteLine("##########################");


                    WriteTing(ref buffer, "     ==== inventory ====", screen_size, new Vector2(10, 21));
                    WriteTing(ref buffer, "##############################", screen_size, new Vector2(8, 22));

                    string[] inventory_array = Display_Inventory();
                    int count = 0;
                    if (inventory_array.Length > 0)
                    {
                        foreach (string item in inventory_array)
                        {
                            //Console.Write(count + " ");
                            //Console.WriteLine(item);

                            WriteTing(ref buffer, count.ToString() + "", screen_size, new Vector2(8, 23 + count));
                            WriteTing(ref buffer, item, screen_size, new Vector2(10, 23 + count));
                            count++;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("You dont have any items to use");

                        WriteTing(ref buffer, "You dont have any items to use", screen_size, new Vector2(8, 23 + count));
                        Console.Clear();
                        Console.Write(buffer.ToString());
                        Console.ReadLine();
                        continue;
                    }
                    Console.Clear();
                    Console.Write(buffer.ToString());
                    Console.ReadLine();

                    Console.Write("what item do you want to use: ");
                    string inv = Console.ReadLine() ?? "";
                    count = 0;

                    //converts a string into the coresponding int
                    int inv_int = -1;
                    bool found = true;
                    if (!int.TryParse(inv, out inv_int))
                    {
                        found = false;
                        foreach (string item in inventory_array)
                        {
                            int index = item.IndexOf(':');
                            if (item.Remove(index).Trim() == inv.ToLower().Trim())
                            {
                                found = true;
                                inv_int = count;
                            }
                            count++;
                        }
                    }
                    if (inv_int == 0 && !found)
                    {
                        inv_int = -1;
                    }

                    Console.WriteLine(inv_int);

                    Console.WriteLine(inv);

                    Dictionary<string, string> inventory_quotes = new Dictionary<string, string>
                    {
                        {"fish", "you eat the fish"},
                        {"health potion", "you drink potion and regained 20 health"},
                        {"rock", "you eat the rock, it hurts"},
                        {"protective charm", "You feel safer during this fight"}

                    };

                    string name = Get_Item_Name(inv_int);
                    string quote = "";
                    _ = inventory_quotes.TryGetValue(name, out quote);
                    Console.WriteLine(quote);
                    switch (name)
                    {
                        case "fish":
                            health = health + 5;
                            Remove_Item("fish", 1);
                            break;
                        case "health potion":
                            health = health + 20;
                            Remove_Item("health potion", 1);
                            break;
                        case "rock":
                            health = health - 10;
                            Remove_Item("rock", 1);
                            break;
                        case "protective charm":
                            protection = true;
                            Console.WriteLine(protection);
                            break;

                        case "exit":
                            Console.WriteLine("exit");
                            break;
                        default:
                            Console.WriteLine("Invaild input");
                            break;
                    }
                    // item stuff
                    Console.ReadLine();
                }
                /*
                //if (c.Key == ConsoleKey.P)
                //{
                //    Console.WriteLine("you have a reaction time of " + x + " seconds");
                //    Console.WriteLine("yuh");
                //    Console.ReadLine();
                //    Console.Clear();
                //}
                */

            } while (game_running == true);
        }
        //Dodging();

        //code for dodging - returns an int depending on how much damage taken 0 dead 1 survived 2 perfect -1 something went wrong
        static int Dodging(int player_health, int damage_per_hit)
        {
            // updated variables to snake_case
            Console.CursorVisible = false;

            /* 10 rows by 31 columns grid.
             0 = empty, 1 = obstacle present.
            */
            int[,] grid = new int[10, 31];
            int loop_counter = 0, player_pos = 15;
            int score = 0;
            int damage = 0;

            int grid_x_limit = 31, grid_y_limit = 9;
            int x_number = 1;
            bool is_running = true;
            Random rand = new();

            while (is_running)
            {
                // Input Control 
                if (Console.KeyAvailable)                                   
                    {                                                       
                    var key = Console.ReadKey(true).Key;                    

                    if (key == ConsoleKey.A && player_pos > 0)              
                        player_pos--;                                       
                    if (key == ConsoleKey.D && player_pos < 30)             
                        player_pos++;                                       
                    while (Console.KeyAvailable)                            
                        Console.ReadKey(true);                              
                }                                                           
                // Game Logic: Move obstacles down (Iterating TOP to BOTTOM to prevent double-moving) 
                for (int i = grid_y_limit; i >= 0; i--)
                {
                    for (int x = 0; x < grid_x_limit; x++)
                    {
                        if (grid[i, x] == 1)
                        {
                            grid[i, x] = 0; // Clear current position 
                            if (i < grid_y_limit)
                            {
                                grid[i + 1, x] = 1; // Move down one row safely
                            }
                            else
                            {
                                score++; // Cleared the bottom row successfully
                                if ((score % 50 == 0) && (score < 50))
                                {
                                    x_number--;
                                }
                                if (score == 50)
                                {
                                    is_running = false;
                                }
                            }
                        }
                    }
                }
                // Spawn Logic (drop an X at row 0)
                if (score < 41)
                {
                    loop_counter++;
                    if (loop_counter % x_number == 0)
                    {
                        int spawn_pos = rand.Next(0, grid_x_limit);
                        grid[0, spawn_pos] = 1;
                    }
                }
                // Collision Detection (Player is always on row 9) 
                if (grid[9, player_pos] == 1)
                {
                    Console.Beep();
                    damage += damage_per_hit;
                    Thread.Sleep(100);

                    if (player_health <= 0)
                    {
                        is_running = false;
                    }
                    else
                    {
                        grid[9, player_pos] = 0; // Clear the obstacle so it doesn't instantly hit again
                        score += 1;
                    }
                }
                else
                {
                    // Rendering What will be displayed on the screen
                    Console.SetCursorPosition(0, 0);
                    Console.Write(WHITE);
                    Console.WriteLine("X's missed: " + score + "\t\t Health: " + (player_health) + "\n----------------------------------");

                    for (int i = 0; i < grid_y_limit + 1; i++)
                    {
                        for (int j = 0; j < grid_x_limit; j++)
                        {
                            if (i == grid_y_limit && j == player_pos)
                            {
                                Console.Write(GREEN);
                                Console.Write("P");

                            }
                            else if (grid[i, j] == 1)
                            {
                                Console.Write(RED);
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
            Console.Write(RESET);
            return damage;

            if (player_health >= 3)
            {
                Console.WriteLine($"You did very bad!\n\nPress Enter to exit...");
                return 0;
            }
            else if ((player_health > 0) && (player_health < 3))
            {
                Console.WriteLine($"You Survived! Score: {score}\n\nPress Enter to exit...");
                return 1;
            }
            else if (player_health == 0)
            {
                Console.WriteLine($"Perfect Dodge! Score: {score} Health: {player_health}\n\nPress Enter to exit...");
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
            Console.WriteLine("You open your eyes, and the villagers stare at you as if they already know who you are.");
            Console.WriteLine("But you have never been here before.");
            Console.WriteLine();

            Thread.Sleep(4000);
            Console.WriteLine("As the day passes...");
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
            Console.WriteLine("*WOULD YOU WANT TO OPEN IT (YES/NO)*");
            truth2 = Console.ReadLine();
            if (truth2 == "yes")
            {
                viewed_memory1 = true;
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
            string truth3, confirm, Dconfirm;
            Console.WriteLine("Did you get any memory fish? (yes/no)");
            confirm = Console.ReadLine();
            if (confirm == "yes" && viewed_memory1)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Did you talk with Jane before?");
                Thread.Sleep(1000);
                Dconfirm = Console.ReadLine();
                if (Dconfirm == "yes" && viewed_memory2)
                {
                    if (memory3 == false)
                    {
                        Console.WriteLine("You don't have the last memory");
                        return;
                    }
                    Console.WriteLine("*Then here your last TRUTH FRAGMENT*");
                    Console.WriteLine();
                    Console.WriteLine();
                    Thread.Sleep(1000);
                    Console.WriteLine("*WOULD YOU WANT TO OPEN IT (YES/NO)*");
                    truth3 = Console.ReadLine();
                    if (truth3 == "yes")
                    {
                        memory3 = true;
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

            if ((memory1 == true) && (memory2 == true) && (memory3 == true))
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("As the third Truth Fragment falls into your hands, the pieces finally come together.");
                Console.WriteLine("The fog surrounding the island begins to fade.");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1000);
                Console.WriteLine("Memories return.");

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("You remember all the things.");
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("The storm.");
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("The wave.");
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("And the shipwreck.");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("The villagers were never ordinary people.");
                Thread.Sleep(1500);
                Console.WriteLine("They were fragments of memories trapped on the island, repeating the same days over and over again.");
                Console.WriteLine();
                Thread.Sleep(1500);
                Console.WriteLine("And the island itself was never a real place.");
                Console.WriteLine();
                Thread.Sleep(1500);
                Console.WriteLine("It existed between memory and reality.");

                Thread.Sleep(3000);
                Console.WriteLine();
                Console.WriteLine("For a long time, you believed you were searching for a way to leave");
                Console.WriteLine();
                Thread.Sleep(1500);
                Console.WriteLine("But now you understand the truth.");
                Console.WriteLine();
                Thread.Sleep(1500);
                Console.WriteLine("You had already died that night at the sea.");
                Console.WriteLine();
                Console.WriteLine();
                Thread.Sleep(1500);
                Console.WriteLine("IThe island was your final chance to remember who you were.");

                Thread.Sleep(3000);
                Console.WriteLine();
                Console.WriteLine("As the last fragment merges with the others, the village slowly disappears.");
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("The fog lifts.");
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("The sea becomes calm.");
                Console.WriteLine();
                Console.WriteLine();

                Thread.Sleep(1500);
                Console.WriteLine();
                Console.WriteLine("The horizon is visible for the first time.");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("You close your eyes and take a final breath.");
                Thread.Sleep(3000);

                Console.Write(Color_Helper(231, false));
                Console.Write(Color_Helper(92, true));
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Then everything fades to white.");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Thread.Sleep(4500);
                Console.WriteLine("Thank you for playing.");
                Console.WriteLine();
                Console.WriteLine();
                Thread.Sleep(2500);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("=========================THE END==============================");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.ReadLine();

                Environment.Exit(0);
            }
        }

    }
}
