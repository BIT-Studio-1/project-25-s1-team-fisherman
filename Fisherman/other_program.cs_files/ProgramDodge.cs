using System;
using System.Threading;

class Game
{
    static void Main()
    {
        // Preparing for dodge update 3
        Console.CursorVisible = false;

        // 10 rows by 31 columns grid.
        // 0 = empty, 1 = obstacle present. 
        int[,] grid = new int[10, 31];
        int loopCounter = 0, playerPos = 15;
        int score = 0, health = 3;
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

                if (key == ConsoleKey.A && playerPos > 0) playerPos--;
                if (key == ConsoleKey.D && playerPos < 30) playerPos++;
                while (Console.KeyAvailable) Console.ReadKey(true);
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
                health--;
                Thread.Sleep(100);

                if (health <= 0)
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
                Console.WriteLine($"X's missed: {score}\t\tHealth: {health}\n----------------------------------");

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

        if (health == 0)
        {
            Console.WriteLine($"You Died!\n\nPress Enter to exit...");
        }
        else if ((health > 0) && (health < 3))
        {
            Console.WriteLine($"You Survived! Score: {score} / 150\n\nPress Enter to exit...");
        }
        else if (health == 3)
        {
            Console.WriteLine($"Perfect Dodge! Score: {score} / 150 Health: {health}\n\nPress Enter to exit...");
        }
        Console.ReadLine();
    }
}