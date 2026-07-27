using static Team_Fisherman.Program;



namespace test_fish
{
    using System;
    using System.Collections.Generic;

    class Fish
    {
        public string Name;
        public string Rarity;
        public float MinWeight, MaxWeight;
        public int Value;
        public float Chance;
        public ConsoleColor Color;
        



        public Fish(string name, string rarity, float minW, float maxW, int value, float chance, ConsoleColor color)
        {
            Name = name; Rarity = rarity;
            MinWeight = minW; MaxWeight = maxW;
            Value = value; Chance = chance;
            Color = color;
        }

        // Checks if a random roll between 0.0 and 1.0 is successful,
        // using the base catch rate boosted by the player's luck.
        public bool Roll(float luck) => new Random().NextDouble() < Chance * (1 + luck);

        // Returns a random weight between MinWeight and MaxWeight, rounded to the nearest tenth public float GetWeight()
        public float GetWeight()
        {
            var rng = new Random();
            return (float)Math.Round(MinWeight + rng.NextDouble() * (MaxWeight - MinWeight), 1);
        }
    }

    class Fishing_Base
    {
        static Random rng = new Random();
        public static int coins = 0;
        public static bool memory = false;
        static Fish[] table = 
        {
            new Fish ("Boot",      "common",   0.1f,  1f,    5,    0.30f, ConsoleColor.White),
            new Fish("Sardine",   "common",   0.1f,  0.5f,  10,    0.28f, ConsoleColor.White),
            new Fish("Carp",      "common",   1f,    3.5f,  15,    0.20f, ConsoleColor.Yellow),
            new Fish("Catfish",   "common",   2f,    6f,    20,   0.13f, ConsoleColor.Gray),
            new Fish("Bass",      "uncommon", 1.5f,  5f,    30,   0.05f, ConsoleColor.Green),
            new Fish("Trout",     "uncommon", 1f,    4f,    40,   0.05f, ConsoleColor.DarkGreen),
            new Fish("Sturgeon",      "rare",     3f,    9f,    80,   0.015f, ConsoleColor.DarkGray),
            new Fish("Salmon",    "rare",     4f,    12f,   120,  0.010f, ConsoleColor.Red),
            new Fish("Great White Shark",    "rare",     6f,    15f,   400,  0.07f, ConsoleColor.Red),
            new Fish ("Memory Fish", "epic", 10f, 20f, 1000, 0.0f, ConsoleColor.Magenta),
            new Fish("Swordfish", "epic",     8f,    25f,   500,  0.003f, ConsoleColor.Blue),
            new Fish("Leviathan", "epic",     30f,   99f,   2500, 0.002f, ConsoleColor.Cyan),
            new Fish("Cthulhu", "mythic",     60f,   120f,   5000, 0.001f, ConsoleColor.Cyan),
        };

        static int  caught = 0, casts = 0, rodLv = 0, luckLv = 0;
        static List<(string Name, string Rarity, float Weight, int Value)> inventory = new();
        static int[] upgradeCosts = { 30, 80, 200, 600, 1500, 3000 };
        static int pityCounter = 0;

        static float Luck() => luckLv * 0.40f;
        static float RodBonus() => rodLv * 0.20f;

        //This function goes through the fish table from start to finish, with the rarest fish coming last.
        //it‍ checks each fish and returns the first one that passes its chance roll
        // Since common fish are ‍listed at the top,
        //they get checked first, but rarer fish down the list can still win if all the earlier ‍rolls miss.
        //If none of the fish pass their rolls, the function returns nothing, meaning the fish got‍ away.
        static Fish RollFish()
        {
            foreach (var f in table)
                if (rng.NextDouble() < f.Chance * (1 + Luck())) return f;
            return null;
        }

        // The upgrade cost depends on how high your rod and luck levels are combined together.
        // This mean‍s every upgrade will cost more than the last one, no matter which stat you leveled up most recently.‍
        // The function gives back -1 once you've used up all available upgrade levels.
        static int NextUpgradeCost()
        {
            int lvl = rodLv + luckLv;
            return lvl < upgradeCosts.Length ? upgradeCosts[lvl] : -1;
        }

        static void Cast()
        {
            casts++;
            Console.WriteLine("You cast your line out...");

            // Increased luck reduces wait time by 200ms per level, with a floor of 500ms
            int waitMs = rng.Next(1000, 5000) - luckLv * 200;
            Thread.Sleep(Math.Max(500, waitMs));
            Console.WriteLine("Something is tugging! Type 'reel in'.");
        }

        static void ReelIn()
        {
            Fish f = null;
            bool caughtFish = Team_Fisherman.Program.fishyGame();

            //Every 10 fish you caught a Memory Fish is Guaranteed
            if (pityCounter >= 10)
            {
                f = new Fish ("Memory Fish", "epic", 10f, 20f, 1000, 0.0f, ConsoleColor.Magenta);
                memory = true;
                pityCounter = 0;
                
            }
            else
            {
                do
                {
                    if (!caughtFish)
                    {
                        Console.WriteLine("The Fish got away");
                        return;
                    }
                    f = RollFish();
                    
                } while (f == null);
                
                pityCounter++;
            
            }
            float w = f.GetWeight(); 
            int earn = (int)Math.Round(f.Value * (1 + RodBonus()));

            coins += earn;
            caught++;
            inventory.Add((f.Name, f.Rarity, w, earn));

            Console.ForegroundColor = f.Color;
            Console.WriteLine($"Caught a {f.Name} ({f.Rarity}) - {w} kg - +{earn} coins");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ResetColor();
            }
            
        static void Upgrade()
        {
            int cost = NextUpgradeCost();
            if (cost == -1) { Console.WriteLine("Gear is maxed out."); return; }
            if (coins < cost) { Console.WriteLine($"Need {cost} coins, have {coins}."); return; }
            coins -= cost;
            // Switch between upgrading rod and luck so they stay about even.
            // Pick rod first if it's not ahead; otherwise go with luck.
            if (rodLv <= luckLv) rodLv++; else luckLv++;
            Console.WriteLine($"Upgraded! Rod {rodLv} / Luck {luckLv} — {coins} coins left");
        }

        static void Stats()
        {
            Console.WriteLine($"Coins: {coins}  Caught: {caught}  Casts: {casts}");
            Console.WriteLine($"Rod: {rodLv}  Luck: {luckLv}");
            int cost = NextUpgradeCost();
            if (cost > 0) Console.WriteLine($"Next upgrade: {cost} coins");
            Console.WriteLine($"Memory Fish pity: {pityCounter}/10");
        }

        public static void Fishing()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Fishing time !");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Commands: cast, reel in, inventory, upgrade, stats, quit");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "cast") Cast();
                else if (input == "reel in") ReelIn();
                //The slice operation input[5..] removes the initial "sell " text, leaving only the fish name behind.
                else if (input == "upgrade") Upgrade();
                else if (input == "stats") Stats();
                else if (input == "quit") break;
                else Console.WriteLine("Unknown command. Try: cast, reel in, upgrade, stats");
                Console.ForegroundColor= ConsoleColor.White;
            }

        }
    }
}
