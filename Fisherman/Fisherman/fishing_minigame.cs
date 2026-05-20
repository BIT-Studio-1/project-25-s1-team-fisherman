    using System;
    using System.Collections.Generic;

    class Fish
    {
        public string Name;
        public string Rarity;
        public float MinWeight, MaxWeight;
        public int Value;
        public float Chance;

        public Fish(string name, string rarity, float minW, float maxW, int value, float chance)
        {
            Name = name; Rarity = rarity;
            MinWeight = minW; MaxWeight = maxW;
            Value = value; Chance = chance;
        }

        public bool Roll(float luck) => new Random().NextDouble() < Chance * (1 + luck);

        public float GetWeight()
        {
            var rng = new Random();
            return (float)Math.Round(MinWeight + rng.NextDouble() * (MaxWeight - MinWeight), 1);
        }
    }

    class Fishing_Base
    {
        static Random rng = new Random();

        static Fish[] table = {
        new Fish("Boot",      "common",   0.1f,  1f,    1,    0.30f),
        new Fish("Sardine",   "common",   0.1f,  0.5f,  3,    0.28f),
        new Fish("Carp",      "common",   1f,    3.5f,  8,    0.20f),
        new Fish("Catfish",   "common",   2f,    6f,    15,   0.13f),
        new Fish("Bass",      "uncommon", 1.5f,  5f,    30,   0.05f),
        new Fish("Trout",     "uncommon", 1f,    4f,    25,   0.05f),
        new Fish("Pike",      "rare",     3f,    9f,    80,   0.015f),
        new Fish("Salmon",    "rare",     4f,    12f,   120,  0.010f),
        new Fish("Swordfish", "epic",     8f,    25f,   500,  0.003f),
        new Fish("Leviathan", "epic",     30f,   99f,   2500, 0.001f),
    };

        static int coins = 0, caught = 0, casts = 0, rodLv = 0, luckLv = 0;
        static List<(string Name, string Rarity, float Weight, int Value)> inventory = new();
        static int[] upgradeCosts = { 30, 80, 200, 600, 1500, 4000 };

        static float Luck() => luckLv * 0.15f;
        static float RodBonus() => rodLv * 0.10f;

        static Fish RollFish()
        {
            foreach (var f in table)
                if (rng.NextDouble() < f.Chance * (1 + Luck())) return f;
            return null;
        }

        static int NextUpgradeCost()
        {
            int lvl = rodLv + luckLv;
            return lvl < upgradeCosts.Length ? upgradeCosts[lvl] : -1;
        }

        static void Cast()
        {
            casts++;
            Console.WriteLine("You cast your line out...");
            int waitMs = rng.Next(1000, 5000) - luckLv * 200;
            Thread.Sleep(Math.Max(500, waitMs));
            Console.WriteLine("Something is tugging! Type 'reel in'.");
        }

        static void ReelIn()
        {
            var f = RollFish();
            if (f == null) { Console.WriteLine("The fish got away!"); return; }

            float w = f.GetWeight();
            int earn = (int)Math.Round(f.Value * (1 + RodBonus()));
            coins += earn;
            caught++;
            inventory.Add((f.Name, f.Rarity, w, earn));
            Console.WriteLine($"Caught a {f.Name} ({f.Rarity}) — {w} kg — +{earn} coins");
        }

        static void ShowInventory()
        {
            if (inventory.Count == 0) { Console.WriteLine("Inventory is empty."); return; }

            var grouped = new Dictionary<string, (string Rarity, int Count, float TotalW, int TotalV)>();
            foreach (var i in inventory)
            {
                if (!grouped.ContainsKey(i.Name))
                    grouped[i.Name] = (i.Rarity, 0, 0, 0);
                var g = grouped[i.Name];
                grouped[i.Name] = (g.Rarity, g.Count + 1, g.TotalW + i.Weight, g.TotalV + i.Value);
            }

            Console.WriteLine($"── inventory ({inventory.Count} items) ──");
            foreach (var kv in grouped)
            {
                var g = kv.Value;
                float avg = (float)Math.Round(g.TotalW / g.Count, 1);
                Console.WriteLine($"  {kv.Key} x{g.Count}  {avg} kg avg  {g.TotalV}c total");
            }
            int sellable = 0;
            foreach (var i in inventory) sellable += i.Value;
            Console.WriteLine($"Sell value: {sellable}c — type 'sell all' or 'sell [name]'");
        }

        static void SellAll()
        {
            if (inventory.Count == 0) { Console.WriteLine("Inventory is empty."); return; }
            int total = 0;
            foreach (var i in inventory) total += i.Value;
            int n = inventory.Count;
            inventory.Clear();
            coins += total;
            Console.WriteLine($"Sold {n} items for +{total} coins. Total: {coins}");
        }

        static void Sell(string name)
        {
            int idx = inventory.FindIndex(i => i.Name.ToLower() == name);
            if (idx == -1) { Console.WriteLine($"No '{name}' in inventory."); return; }
            var item = inventory[idx];
            inventory.RemoveAt(idx);
            coins += item.Value;
            Console.WriteLine($"Sold {item.Name} for +{item.Value} coins. Total: {coins}");
        }

        static void Upgrade()
        {
            int cost = NextUpgradeCost();
            if (cost == -1) { Console.WriteLine("Gear is maxed out."); return; }
            if (coins < cost) { Console.WriteLine($"Need {cost} coins, have {coins}."); return; }
            coins -= cost;
            if (rodLv <= luckLv) rodLv++; else luckLv++;
            Console.WriteLine($"Upgraded! Rod {rodLv} / Luck {luckLv} — {coins} coins left");
        }

        static void Stats()
        {
            Console.WriteLine($"Coins: {coins}  Caught: {caught}  Casts: {casts}");
            Console.WriteLine($"Rod: {rodLv}  Luck: {luckLv}");
            int cost = NextUpgradeCost();
            if (cost > 0) Console.WriteLine($"Next upgrade: {cost} coins");
        }

        public static void Fishing()
        {
        Console.Clear();
            Console.WriteLine("Fishing time !");
            Console.WriteLine("Commands: cast, reel in, inventory, sell [name], sell all, upgrade, stats, quit");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "cast") Cast();
                else if (input == "reel in") ReelIn();
                else if (input == "inventory" || input == "inv") ShowInventory();
                else if (input == "sell all") SellAll();
                else if (input.StartsWith("sell ")) Sell(input[5..]);
                else if (input == "upgrade") Upgrade();
                else if (input == "stats") Stats();
                else if (input == "quit") break;
                else Console.WriteLine("Unknown command. Try: cast, reel in, inventory, sell, upgrade, stats");
            }
         
        }
    }

