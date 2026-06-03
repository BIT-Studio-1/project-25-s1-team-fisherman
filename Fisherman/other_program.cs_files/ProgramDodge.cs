class Game
{
    static void Main()
    {
        // Updating dodge 0.1.2
        //commit 4
        Console.CursorVisible = false;

        // 10 rows by 31 columns grid.
        int[,] grid = new int[10, 31];
        int playerXPos = 15;
        int playerYPos = 9;
        int score = 0, health = 3;
        int gridXLimit = 31, gridYLimit = 9;
        bool isRunning = true;
        Random rand = new();

        // Timing variables
        int blinkCounter = 0;
        int framesPerDrop = 4; // how fast the x's fall
        bool shouldSpawnNext = true;

        // Flashing obstacles
        int flashCounter = 0;
        const int flashDurationFlashes = 50;
        bool flashToggle = false;

        while (isRunning)
        {
            // Input control ( W A S D )
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                // Horizontal movement
                if (key == ConsoleKey.A && playerXPos > 0) playerXPos--;
                if (key == ConsoleKey.D && playerXPos < 30) playerXPos++;

                // Vertical movement
                if (key == ConsoleKey.W && playerYPos > 0) playerYPos--;
                if (key == ConsoleKey.S && playerYPos < gridYLimit) playerYPos++;

                while (Console.KeyAvailable) Console.ReadKey(true);
            }

            // Increment the frame counter every time the main loop runs
            blinkCounter++;

            // Control the obstacles at the top row if the is flashing
            bool topRowIsFlashing = false;
            for (int x = 0; x < gridXLimit; x++)
            {
                if (grid[0, x] == 1)
                {
                    topRowIsFlashing = true;
                    break;
                }
            }

            if (topRowIsFlashing)
            {
                flashCounter++;
                // Flash color every 4 frames (80ms) for the blink/flashing X's effect
                if (blinkCounter % 4 == 0)
                {
                    flashToggle = !flashToggle;
                }

                // After 1 second (50 flashes), convert the flashing obstacles into falling obstacles
                if (flashCounter >= flashDurationFlashes)
                {
                    flashCounter = 0;
                    for (int x = 0; x < gridXLimit; x++)
                    {
                        if (grid[0, x] == 1) grid[0, x] = 2;
                    }
                }
            }

            // 2. Game tick logic (Only runs physics when the counter hits the threshold)
            if (blinkCounter >= framesPerDrop)
            {
                blinkCounter = 0; // Reset counter for the next interval

                // Check if the bottom row contains any non flashing falling obstacles
                bool bottomRowHasObstacles = false;
                for (int x = 0; x < gridXLimit; x++)
                {
                    if (grid[gridYLimit, x] == 2)
                    {
                        bottomRowHasObstacles = true;
                        break;
                    }
                }

                // Game Logic: Move non flashing obstacles
                for (int i = gridYLimit; i >= 0; i--)
                {
                    for (int x = 0; x < gridXLimit; x++)
                    {
                        if (grid[i, x] == 2)
                        {
                            grid[i, x] = 0; // Clear current position 
                            if (i < gridYLimit)
                            {
                                grid[i + 1, x] = 2; // Move down one row
                            }
                            else
                            {
                                score++; // Cleared the bottom row
                            }
                        }
                    }
                }

                // If obstacles just hit the bottom row, they disappear on the next step.
                if (bottomRowHasObstacles)
                {
                    shouldSpawnNext = true;
                }

                if (score >= 250)
                {
                    isRunning = false;
                }

                // Spawn Logic: Creates a wall with a 3 space gap
                if (score < 241 && shouldSpawnNext)
                {
                    shouldSpawnNext = false; // Reset flag so it doesn't spam walls
                    int gapStart = rand.Next(0, gridXLimit - 3);

                    for (int x = 0; x < gridXLimit; x++)
                    {
                        if (x >= gapStart && x < gapStart + 3)
                        {
                            grid[0, x] = 0;
                        }
                        else
                        {
                            grid[0, x] = 1; // Spawn as flashing state
                        }
                    }
                }
            }

            // Dynamic collision tracking based on the player's current Y position
            if (grid[playerYPos, playerXPos] == 2)
            {
                Console.Beep();
                health--;
                grid[playerYPos, playerXPos] = 0; // Clear the obstacle so it doesn't instantly hit again
                score += 1; // Correct score glitch
                shouldSpawnNext = true; // Force a respawn if player destroys the obstacle

                if (health <= 0)
                {
                    isRunning = false;
                }
            }
            else
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"X's missed: {score}\t\tHealth: {health}\n----------------------------------");

                for (int i = 0; i < gridYLimit + 1; i++)
                {
                    for (int j = 0; j < gridXLimit; j++)
                    {
                        if (i == playerYPos && j == playerXPos)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("P");
                        }
                        else if (grid[i, j] == 1) // Flashing
                        {
                            Console.ForegroundColor = flashToggle ? ConsoleColor.White : ConsoleColor.DarkGray;
                            Console.Write("X");
                        }
                        else if (grid[i, j] == 2) // Falling
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
            }

            // The loop checks for input every 20ms.
            Thread.Sleep(20);
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
            Console.WriteLine($"You Survived! Score: {score} / 252\n\nPress Enter to exit...");
        }
        else if (health == 3)
        {
            Console.WriteLine($"Perfect Dodge! Score: {score} / 252 Health: {health}\n\nPress Enter to exit...");
        }
        Console.ReadLine();
    }
}