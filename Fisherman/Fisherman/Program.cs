using System.Numerics;
using System.Text;
//using static Fish;
//using static Fishing_Base;

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
        static void Main(string[] args)
        {

            Random rand = new Random();
            //this is a stringbuilder that makes it easy to edit the console output. it works like an a array 
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

            Menu();
            Wait();
            GameIntro();
            while (true)
            {
                path = Get_path(map_tile, path);
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
                map_tile = Vector2.Clamp(map_tile, new Vector2(0, -1), Vector2.One);
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
            }

            Vector2 clamped_offset = Vector2.Clamp(offset, Vector2.Zero, new Vector2(119, 27));
            map_tile = clamped_offset - offset;
            return clamped_offset;
        }

        //up 
        //start right
        //down
        static string Get_path(Vector2 map_pos, string current)
        {
            string path = "";

            if (map_pos == Vector2.UnitY)
            {
                path = "Map/map_up.txt";
            }
            else if (map_pos == Vector2.Zero)
            {
                path = "Map/map_start.txt";
            }
            else if (map_pos == Vector2.UnitX)
            {
                path = "Map/map_start.txt";
            }
            else
            {
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
                Fishing();
                return false;
            }
            else if (buffer[To_Index(coords, size)] == 'S')
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
        static void Menu()
        {
            //this is a stringbuilder that makes it easy to edit the console output. it works like an a array
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

            //Fishing_Base.Fishing();

            Thread.Sleep(500);
            Console.Clear();

        }
        //Put the code for shopping in here.
        static void Shopping()
        {


            Console.WriteLine("Shopping");
            Thread.Sleep(500);
            Console.Clear();


        }
        //Put the code for fighting in here.
        static void Fighting()
        {
            string[] inventory = { "23", "fish", "2", "health potion", "56", "rock" };
            string[] attacks = { "slash", "jab", "bow", "other" };

            //object[] mixedArray = new object[] { 10, "Hello World", 25, "C# Rocks" };


            ConsoleKeyInfo c = new ConsoleKeyInfo();

            Random random = new Random();
            int x = 0;
            int health = 100;
            int badGuyHealth = 100;
            string badGuyAttack = "";
            bool gameRunning = true;
            string badGuyName = "evil guy of doom";
            int waity = 0;
            // the enemies attack
            void enemyAttack()
            {
                //damage
                int damage = random.Next(20, 40);
                health -= damage;
                badGuyAttack = badGuyName + " does " + damage + " damage";
                waity = 1000;
                return;
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

            static void WriteTing(ref StringBuilder buff, string guy, Vector2 screen_size, Vector2 pos)
            {
                int count = 0;
                foreach (char ch in guy)
                {
                    buff[To_Index(new Vector2(count + pos.X, pos.Y), screen_size)] = ch;
                    count++;
                }
            }

            StringBuilder buffer = new StringBuilder();
            bool in_menu = true;
            Vector2 screen_size = new Vector2(120, 28);
            Vector2 play_pos = new Vector2(34, 24);
            Vector2 exit_pos = new Vector2(62, 24);
            Vector2 current = play_pos;

            string enemyIcon1 = "               ";
            string enemyIcon2 = "  _____        ";
            string enemyIcon3 = " | >:) |    /  ";
            string enemyIcon4 = "  -----    /   ";
            string enemyIcon5 = "   |_____ /    ";
            string enemyIcon6 = "   /\\    /\\    ";

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
                Console.SetCursorPosition(0, 0);

                foreach (string line in File.ReadLines("Map/fighting/FightingMenu.txt")) // loops through txt file
                {

                    foreach (char p in line) //each line
                    {
                        //Console.Write(p);
                        buffer.Append(p);
                    }
                    buffer.Append("\n");
                }


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
                WriteTing(ref buffer, badGuyAttack, screen_size, new Vector2(66, 15));
                WriteTing(ref buffer, "health: " + health, screen_size, new Vector2(10, 22));

                Console.Write(buffer.ToString()); // writes the thing
                                                  //Console.ReadLine();


                if (waity < 0)
                {
                    Console.WriteLine("waiting ran");

                    WriteTing(ref buffer, "                        ", screen_size, new Vector2(66, 15));
                }
                
                    waity--;
                Console.WriteLine("waiting");

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
                            badGuyHealth -= 31;
                            enemyAttack();
                            break;
                        case "jab":
                            Console.Write("you do the jab");
                            badGuyHealth -= 20;
                            enemyAttack();
                            break;
                        case "bow":
                            Console.Write("you shoot the bow");
                            badGuyHealth -= 15;
                            enemyAttack();
                            break;
                        case "other":
                            Console.Write("you do the other");
                            badGuyHealth -= 3;
                            enemyAttack();
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
                else if (c.Key == ConsoleKey.B)
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


            } while (gameRunning == true);
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }
        //Dodging();

        //Put the code for dodging in here.
        static void Dodging()
        {

            Console.WriteLine("Dodging");
            Thread.Sleep(2000);
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
    }
}
