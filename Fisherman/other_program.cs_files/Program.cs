namespace menuThing
{
    internal class Program
    {
        static void Main(string[] args)
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

            // the enemys attack
            void enemyAttack()
            {
                //damge
                int damage = random.Next(20, 40);
                health -= damage;
                badGuyAttack = badGuyName + " does " + damage + " damage";
                return;
            }

            // this function converts the string num relateing to the "item" to a int then adds or subtracts the "opper" amount
            void invtoryNum(string item, int opper)
            {
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] == item)
                    {
                        Console.WriteLine(inventory[i - 1]);
                        
                        int intinv = Convert.ToInt32(inventory[i-1]);
                        if (intinv > 0)
                        {
                            intinv += opper;
                            inventory[i - 1] = intinv.ToString();
                        }
                        else 
                        {
                            Console.WriteLine("you dont have enough");
                        }
                        return;
                    }
                }
            }



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

                
                

                //string enemyIcon1 = "               ";
                //string enemyIcon2 = "  _____        ";
                //string enemyIcon3 = " | >:) |    /  ";
                //string enemyIcon4 = "  -----    /   ";
                //string enemyIcon5 = "   |_____ /    ";
                //string enemyIcon6 = "   /|    /|    ";

                 
                string enemyIcon1 = "   _/\\___/\\_  ";
                string enemyIcon2 = "  |  @ __ @ |   ";
                string enemyIcon3 = "  |_________|   ";
                string enemyIcon4 = "   /|.   .|\\   ";
                string enemyIcon5 = "  / |_____| \\  ";
                string enemyIcon6 = "    |     |     ";
                Console.Clear();
                
                Console.WriteLine("0~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~0"); // 1
                Console.WriteLine("|                                                                                                                      |"); // 2
                Console.WriteLine("|                                                                                                                      |"); // 3
                Console.WriteLine("|                                                                                                                      |"); // 4
                Console.WriteLine("|                                                                                                                      |"); // 5
                Console.WriteLine("|                                                                                                                      |"); // 6
                Console.WriteLine("|                                                                                 "+ enemyIcon1.PadRight(30) + "       |"); // 7
                Console.WriteLine("|                                                                                 "+ enemyIcon2.PadRight(30) + "       |"); // 8
                Console.WriteLine("|                                                                                 "+ enemyIcon3.PadRight(30) + "       |"); // 9
                Console.WriteLine("|                                                                                 "+ enemyIcon4.PadRight(30) + "       |"); // 10
                Console.WriteLine("|                                                                                 "+ enemyIcon5.PadRight(30) + "       |"); // 11
                Console.WriteLine("|                                                                                 "+ enemyIcon6.PadRight(30) + "       |"); // 12
                Console.WriteLine("|                                                                              " + badGuyName.PadRight(33) +   "       |"); // 13
                Console.WriteLine("|                                                                         enemy Health: "+ badGuyHealth.ToString().PadRight(30) + " |"); // 14
                Console.WriteLine("|                                                                                                                      |"); // 15
                Console.WriteLine("|                                                                          "   +  badGuyAttack.PadRight(41) +      "   |"); // 16
                Console.WriteLine("|                                                                                                                      |"); // 17
                Console.WriteLine("|                                                                                                                      |"); // 18
                Console.WriteLine("|                                                                                                                      |"); // 19
                Console.WriteLine("|                                                                                                                      |"); // 20
                Console.WriteLine("|                                                                                                                      |"); // 21
                Console.WriteLine("|                                                                                                                      |"); // 22
                Console.WriteLine("|        Health: " + health.ToString().PadRight(34) +  "                                                                    |"); // 23
                Console.WriteLine("|         A.attacks                                                                                                    |"); // 24
                Console.WriteLine("|         B.inventory                                                                                                  |"); // 25
                Console.WriteLine("|                                                                                                                      |"); // 26
                Console.WriteLine("|                                                                                                                      |"); // 27
                Console.WriteLine("|                                                                                                                      |"); // 28
                Console.WriteLine("0~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~0"); // 29



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
                    Console.WriteLine("       ==== inventory ====");
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
                            Console.Write("### ".PadRight(4) + inventory[i].PadRight(2) );
                        }
                    }
                    Console.Write("what item do you want to use: ");
                    string inv = Console.ReadLine();
                    Console.WriteLine(inv);


                    switch (inv)
                    {
                        case "fish":
                            Console.WriteLine("you eat the fish");
                            invtoryNum("fish", -1);
                            break;
                        case "health potion":
                            Console.WriteLine("you drink potion");
                            invtoryNum("health potion", -1);
                            break;
                        case "rock":
                            Console.WriteLine("you rock");
                            invtoryNum("rock", -1);
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










            //while (c.Key != ConsoleKey.Z);


            Console.WriteLine("Hello, World!");
            ConsoleKeyInfo input = Console.ReadKey(true);
            //ConsoleKeyInfo c = new ConsoleKeyInfo();
            while (true)
            {

                if (Console.KeyAvailable == false)
                {
                    Console.WriteLine("yuh");
                    //Console.KeyAvailable = false;
                    Console.WriteLine("yuh yuh");

                }
                //input = Console.ReadKey(true);
                //if (input.Key == ConsoleKey.A)
                //{
                //    Console.WriteLine("print ay ay");
                //    //input.Key = ConsoleKey.Q;
                //}
                //else if (input.Key == ConsoleKey.B)
                //{
                //    Console.WriteLine("print B yuh");
                //}
                //else
                //{
                //    Console.WriteLine("else");
                //}
                //Console.WriteLine("run");
                //input = Console.ReadKey(false);
            }
        }
    }
}
