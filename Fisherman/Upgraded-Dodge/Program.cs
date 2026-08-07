namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            Dodging(3, 1);
        }

        static void Dodging(int player_health, int damage_per_hit)
        {
            Random rand = new Random();
            int selection = rand.Next(0, 3);

            if (selection == 0)
            {
                Dodging_1(player_health, damage_per_hit);
            }
            else if (selection == 1)
            {
                Dodging_2(player_health, damage_per_hit);
            }
            else if (selection == 2)
            {
                Dodging_3(player_health, damage_per_hit);
            }

            static int Dodging_1(int player_health, int damage_per_hit)
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

                        if (key == ConsoleKey.A && player_pos > 0) player_pos--;
                        if (key == ConsoleKey.D && player_pos < 30) player_pos++;
                        while (Console.KeyAvailable) Console.ReadKey(true);
                    }

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
                        player_health -= damage_per_hit;
                        Thread.Sleep(100);

                        if (player_health <= 0)
                        {
                            is_running = false;
                        }
                        else
                        {
                            grid[9, player_pos] = 0;
                            score += 1;
                        }
                    }
                    else
                    {
                        // Rendering What will be displayed on the screen
                        //Console.Write(WHITE);
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("X's missed: " + score + "\t\t Health: " + (player_health) + "\n----------------------------------");

                        for (int i = 0; i < grid_y_limit + 1; i++)
                        {
                            for (int j = 0; j < grid_x_limit; j++)
                            {
                                if (i == grid_y_limit && j == player_pos)
                                {
                                    //Console.Write(GREEN);
                                    Console.Write("P");
                                }
                                else if (grid[i, j] == 1)
                                {
                                    //Console.Write(RED);
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
                //Console.Write(WHITE);
                Console.Clear();

                if (player_health <= 1)
                {
                    Console.WriteLine($"You did very bad!\n\nPress Enter to exit...");
                }
                else if ((player_health > 1) && (player_health < 3))
                {
                    Console.WriteLine($"You Survived! Score: {score}\n\nPress Enter to exit...");
                }
                else if (player_health <= 3)
                {
                    Console.WriteLine($"Perfect Dodge! Score: {score} Health: {player_health}\n\nPress Enter to exit...");
                }

                Console.ReadLine();
                return damage;
            }

            static int Dodging_2(int player_health, int damage_per_hit)
            {
                Console.CursorVisible = false;

                // 10 rows by 31 columns grid. 0 = empty, 1 = obstacle present.

                int[,] grid = new int[10, 31];
                int player_pos = 15;
                int score = 0;
                int damage = 0;
                int grid_x_limit = 31, grid_y_limit = 9;
                bool is_running = true;
                Random rand = new();
                int spawn_timer = 0;
                int fall_timer = 0;
                int fall_delay_frames = 4;

                while (is_running)
                {
                    // Input Control 
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.A && player_pos > 0) player_pos--;
                        if (key == ConsoleKey.D && player_pos < 30) player_pos++;
                        while (Console.KeyAvailable) Console.ReadKey(true);
                    }

                    fall_timer++;
                    if (fall_timer >= fall_delay_frames)
                    {
                        for (int i = grid_y_limit; i >= 0; i--)
                        {
                            for (int x = 0; x < grid_x_limit; x++)
                            {
                                if (grid[i, x] == 1)
                                {
                                    grid[i, x] = 0;
                                    if (i < grid_y_limit)
                                    {
                                        grid[i + 1, x] = 1;
                                    }
                                    else
                                    {
                                        score++;
                                        if (score >= 150)
                                        {
                                            is_running = false;
                                        }
                                    }
                                }
                            }
                        }
                        fall_timer = 0;
                    }

                    // Spawn Logic: Generate a line of X's with a 3-space gap
                    if (spawn_timer >= 30 && score < 140)
                    {
                        int gap_start = rand.Next(0, grid_x_limit - 2);

                        for (int x = 0; x < grid_x_limit; x++)
                        {
                            if (x < gap_start || x > gap_start + 2)
                            {
                                grid[0, x] = 1;
                            }
                        }
                        spawn_timer = 0;
                    }
                    else
                    {
                        spawn_timer++;
                    }

                    // Collision Detection 
                    if (grid[9, player_pos] == 1)
                    {
                        Console.Beep();
                        damage += damage_per_hit;
                        player_health -= damage_per_hit;

                        for (int x = 0; x < grid_x_limit; x++)
                        {
                            grid[9, x] = 0;
                        }

                        Thread.Sleep(100);
                        if (player_health <= 0)
                        {
                            is_running = false;
                        }
                        else
                        {
                            score += 1;
                        }
                    }
                    else
                    {
                        // Rendering 
                        //Console.Write(WHITE);
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("X's missed: " + score + "\t\t Health: " + (player_health) + "\n----------------------------------");
                        for (int i = 0; i < grid_y_limit + 1; i++)
                        {
                            for (int j = 0; j < grid_x_limit; j++)
                            {
                                if (i == grid_y_limit && j == player_pos)
                                {
                                    //Console.Write(GREEN);
                                    Console.Write("P");
                                }
                                else if (grid[i, j] == 1)
                                {
                                    //Console.Write(RED);
                                    Console.Write("X");
                                }
                                else
                                {
                                    Console.Write(" ");
                                }
                            }
                            Console.WriteLine();
                        }
                        Thread.Sleep(50);
                    }
                }
                // Game Over Screen 
                //Console.Write(WHITE);
                Console.Clear();
                if (player_health <= 0)
                {
                    Console.WriteLine($"You did very bad!\n\nPress Enter to exit...");
                }
                else if (player_health < 3)
                {
                    Console.WriteLine($"You Survived! Score: {score}\n\nPress Enter to exit...");
                }
                else
                {
                    Console.WriteLine($"Perfect Dodge! Score: {score} Health: {player_health}\n\nPress Enter to exit...");
                }
                Console.ReadLine();
                return damage;
            }

            static int Dodging_3(int player_health, int damage_per_hit)
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

                        if (key == ConsoleKey.A && player_pos > 0) player_pos--;
                        if (key == ConsoleKey.D && player_pos < 30) player_pos++;
                        while (Console.KeyAvailable) Console.ReadKey(true);
                    }

                    for (int i = grid_y_limit; i >= 0; i--)
                    {
                        for (int x = 0; x < grid_x_limit; x++)
                        {
                            if (grid[i, x] == 1)
                            {
                                grid[i, x] = 0;
                                if (i < grid_y_limit)
                                {
                                    grid[i + 1, x] = 1;
                                }
                                else
                                {
                                    score++;
                                    if ((score % 40 == 0) && (score < 40))
                                    {
                                        x_number--;
                                    }
                                    if (score == 40)
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
                        if (rand.Next(0, 100) < 30)
                        {
                            int spawn_pos = rand.Next(0, grid_x_limit);
                            grid[0, player_pos] = 1;
                        }
                    }
                }
                return 0;
            }
        }
    }
}