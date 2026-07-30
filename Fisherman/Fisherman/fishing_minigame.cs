using static Team_Fisherman.Program;



namespace test_fish
{
    using System;
    using System.Collections.Generic;

    class Fish
    {
        public string name;
        public string rarity;
        public float Min_weight, Max_weight;
        public int Value;
        public float Chance;
        public ConsoleColor Color;
        



        public Fish(string name, string rarity, float min_W, float max_W, int value, float chance, ConsoleColor color)
        {
            this.name = name; this.rarity = rarity;
            Min_weight = min_W; Max_weight = max_W;
            Value = value; Chance = chance;
            Color = color;
        }

        // Checks if a random roll between 0.0 and 1.0 is successful,
        // using the base catch rate boosted by the player's luck.
        public bool Roll(float luck) => new Random().NextDouble() < Chance * (1 + luck);

        // Returns a random weight between MinWeight and MaxWeight, rounded to the nearest tenth public float GetWeight()
        public float Get_Weight()
        {
            var rng = new Random();
            return (float)Math.Round(Min_weight + rng.NextDouble() * (Max_weight - Min_weight), 1);
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

        static int  caught = 0, casts = 0, rod_lv = 0, luck_lv = 0;
        static List<(string Name, string Rarity, float Weight, int Value)> inventory = new();
        static int[] upgrade_costs = { 30, 80, 200, 600, 1500, 3000 };
        static int pity_counter = 0;

        static float Luck() => luck_lv * 0.40f;
        static float Rod_Bonus() => rod_lv * 0.20f;
        /*
          This function goes through the fish table from start to finish, with the rarest fish coming last.
          it‍ checks each fish and returns the first one that passes its chance roll
          Since common fish are ‍listed at the top,
          they get checked first, but rarer fish down the list can still win if all the earlier ‍rolls miss.
          If none of the fish pass their rolls, the function returns nothing, meaning the fish got‍ away.
        */
        static Fish Roll_Fish()
        {
            foreach (var f in table)
                if (rng.NextDouble() < f.Chance * (1 + Luck())) return f;
            return null;
        }
        /*
           The upgrade cost depends on how high your rod and luck levels are combined together.
           This mean‍s every upgrade will cost more than the last one, no matter which stat you leveled up most recently.‍
           The function gives back -1 once you've used up all available upgrade levels.
        */
        static int Next_Upgrade_Cost()
        {
            int lvl = rod_lv + luck_lv;
            return lvl < upgrade_costs.Length ? upgrade_costs[lvl] : -1;
        }

        static void Cast()
        {
            casts++;
            Console.WriteLine("You cast your line out...");

            // Increased luck reduces wait time by 200ms per level, with a floor of 500ms
            int waitMs = rng.Next(1000, 5000) - luck_lv * 200;
            Thread.Sleep(Math.Max(500, waitMs));
            Console.WriteLine("Something is tugging! Type 'reel in'.");
        }

        static void Reel_In()
        {
            Fish f = null;
            bool caught_Fish = Team_Fisherman.Program.fishy_Game();

            //Every 10 fish you caught a Memory Fish is Guaranteed
            if (pity_counter >= 10)
            {
                f = new Fish ("Memory Fish", "epic", 10f, 20f, 1000, 0.0f, ConsoleColor.Magenta);
                memory = true;
                pity_counter = 0;
                
            }
            else
            {
                do
                {
                    if (!caught_Fish)
                    {
                        Console.WriteLine("The Fish got away");
                        return;
                    }
                    f = Roll_Fish();
                    
                } while (f == null);
                
                pity_counter++;
            
            }
            float w = f.Get_Weight(); 
            int earn = (int)Math.Round(f.Value * (1 + Rod_Bonus()));

            coins += earn;
            caught++;
            inventory.Add((f.name, f.rarity, w, earn));

            Console.ForegroundColor = f.Color;
            Console.WriteLine($"Caught a {f.name} ({f.rarity}) - {w} kg - +{earn} coins");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ResetColor();
            }
            
        static void Upgrade()
        {
            int cost = Next_Upgrade_Cost();
            if (cost == -1) { Console.WriteLine("Gear is maxed out."); return; }
            if (coins < cost) { Console.WriteLine($"Need {cost} coins, have {coins}."); return; }
            coins -= cost;
            // Switch between upgrading rod and luck so they stay about even.
            // Pick rod first if it's not ahead; otherwise go with luck.
            if (rod_lv <= luck_lv) rod_lv++; else luck_lv++;
            Console.WriteLine($"Upgraded! Rod {rod_lv} / Luck {luck_lv} — {coins} coins left");
        }

        static void Stats()
        {
            Console.WriteLine($"Coins: {coins}  Caught: {caught}  Casts: {casts}");
            Console.WriteLine($"Rod: {rod_lv}  Luck: {luck_lv}");
            int cost = Next_Upgrade_Cost();
            if (cost > 0) Console.WriteLine($"Next upgrade: {cost} coins");
            Console.WriteLine($"Memory Fish pity: {pity_counter}/10");
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
                else if (input == "reel in") Reel_In();
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
