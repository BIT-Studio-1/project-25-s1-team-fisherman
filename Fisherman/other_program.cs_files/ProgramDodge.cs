
class Game
{
    static void Main()
    {

        Console.CursorVisible = false;

        // 10 rows by 21 columns grid.
        //0 = empty, 1 = obstacle present. 
        int[,] grid = new int[10, 31];
        int loopCounter = 0, playerPos = 15,
        score = 0, health = 3,
        gridXLimit = 31, gridYLimit = 9,
        xNumber = 3;
        bool isRunning = true;
        Random rand = new();

        while (isRunning)
        {
            // Input Control 
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.A && playerPos > 1) playerPos--;

                if (key == ConsoleKey.D && playerPos < 30) playerPos++;
                while (Console.KeyAvailable) Console.ReadKey(true);
            }

            // Game Logic: Move obstacles down (from top row too bottom row) 
            for (int i = gridYLimit; i >= 0; i--)
            {
                for (int x = 1; x < gridXLimit; x++)
                {
                    if (grid[i, x] == 1)
                    {
                        grid[i, x] = 0; // Clear current position 
                        if (i < gridYLimit)
                        {
                            grid[i + 1, x] = 1; // Move down one row 
                        }
                        else
                        {
                            score++; // Cleared the bottom row successfully
                            if ((score % 50 == 0) && (score < 150))
                            {
                                xNumber--;
                            }
                        }
                    }
                }
            }

            // Spawn Logic (drop an X at row 0, ever 3 rows)
            loopCounter++;
            if (loopCounter % xNumber == 0)
            {
                int spawnPos = rand.Next(1, gridXLimit);
                grid[0, spawnPos] = 1;
            }

            // Collision Detection (Player is always on row 9) 
            if (grid[9, playerPos] == 1)
            {
                if (health <= 1)
                {
                    Console.Beep();
                    isRunning = false; // Break the loop on next evaluation
                    Thread.Sleep(100);
                }
                else
                {
                    health--; //minus health
                    Thread.Sleep(100);
                }
            }
            else
            {
                // Rendering What will be displayed on the screen
                Console.SetCursorPosition(0, 0);

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
            }
        }
    }
}
