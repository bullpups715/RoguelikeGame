using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Timers;
using System.Xml.Serialization;
using MEEm;
class Buff
{
    public string Name;
    public string Description;
    public int BuffDamage;
    public int BuffHealth;
    public double BuffEvasion;
    public int BuffEnemyDamage;
    public bool BloodHarvest;   // лечение при убийстве
    public bool LuckyThief;     // +шанс ключа
    public bool NightEye = false;       // видимость ловушек
    public int BuffInventorySlots;
}
class Enemy
{
    public char Symbol;
    public string Name;
    public int HP;
    public int damage;
    public double chance;
    public ConsoleColor Color;
    public int MinLevel;
    public int MaxLevel;
    public double evasion;
}
class Boss
{
    public char SymbolB;
    public string NameB;
    public int HPB;
    public int damageB;
    public double chanceB;
    public ConsoleColor ColorB;
    public int LevelB;
    public int SoulValue;
    public double evasionB;
}
class Item
{
    public string Name;
    public string Type;
    public int Value;
}
class Game
{
    const int width = 20;
    const int heigth = 30;
    public static char[,] map = new char[width, heigth];
    public static int[,] enemiesHealth = new int[width, heigth];
    public static List<Item> Inventory = new List<Item>();
    public static List<Enemy> Enemies = new List<Enemy>();
    public static List<Boss> bosses = new List<Boss>();
    public static List<Buff> buffs;
    public static int playerX = 2, playerY = 2, enemyCount = 0, steps, playerHealth, kill;
    public static int MaxInvetorySize = 5;
    public static int Level = 0;
    public static int Souls = 0;
    public static int bonusDamage = 0;
    public static int bonusHealth = 0;
    public static double bonusEvasion = 0;
    public static int bonusEnemyDamage = 0;
    public static int countBosses = 0;
    public static bool BossDefeated = false;
    public static bool BossSpawn = false;
    public static int trollStepCount = 0;
    public static bool hasBloodHarvest = false;
    public static bool hasLuckyThief = false;
    public static bool hasNightEye = false;
    public static int GetMaxHealth()
    {
        if (hasBloodHarvest) 
        { 
            return 3;
        }
        else
        {
            return 5 + bonusHealth;
        }
    }

    public static int GetDamage() => 1 + bonusDamage;


    static void InitializeBuffs()
    {
        buffs = new List<Buff>();
        buffs.Add(new Buff
        {
            Name = "Крепкая кость",
            Description = "Твои кости закалились в подземных кузницах. Ты выдержишь больше ударов, но каждый твой удар стал тяжелее и медленнее",
            BuffDamage = -1,
            BuffHealth = 3,
            BuffEvasion = 0,
            BuffEnemyDamage = 0,
            BuffInventorySlots = 0
        });
        buffs.Add(new Buff
        {
            Name = "Острые клинки",
            Description = "Твой меч наточили сами боги Подземья. Ты режешь врагов как масло, но c тяжёлым мечом ты потерял былую ловкость",
            BuffDamage = 2,
            BuffHealth = 0,
            BuffEvasion = -0.05,
            BuffInventorySlots = 0,
            BuffEnemyDamage = 0,
        });
        buffs.Add(new Buff
        {
            Name = "Лёгкие ноги",
            Description = "Ветер шепчет тебе на ухо. Ты скользишь между ударами, но твоё тело стало хрупким",
            BuffHealth = -1,
            BuffDamage = 0,
            BuffEnemyDamage = 0,
            BuffInventorySlots = 0,
            BuffEvasion = 0.15,
        });
        buffs.Add(new Buff
        {
            Name = "Рюкзак путешественника",
            Description = "Старый походный мешок хранит много тайн. Ты можешь нести больше, но лишний груз сковывает движения",
            BuffHealth = 0,
            BuffDamage = 0,
            BuffEvasion = -0.1,
            BuffInventorySlots = 2,
            BuffEnemyDamage = 0
        });
        buffs.Add(new Buff
        {
            Name = "Каменная кожа",
            Description = "Боги наградили тебя твердыней. Враги ломают зубы о твою плоть, но ты стал неповоротлив как скала",
            BuffDamage = 0,
            BuffHealth = 0,
            BuffEnemyDamage = -1,
            BuffInventorySlots = 0,
        });
        buffs.Add(new Buff
        {
            Name = "Кровавая жатва",
            Description = "Каждая смерть наполняет тебя жизнью. Но цена этой силы — часть твоей собственной души",
            BuffDamage = 0,
            BuffHealth = -2,
            BuffEvasion = 0,
            BuffEnemyDamage = 0,
            BuffInventorySlots = 0,
            BloodHarvest = true,
        });
        buffs.Add(new Buff
        {
            Name = "Удачливый вор",
            Description = "Ты заключил сделку с тенью. Ключи сами падают в твои руки, но зелья горчат проклятием",
            BuffInventorySlots = 0,
            BuffEnemyDamage = 0,
            BuffDamage = 0,
            BuffEvasion = 0,
            BuffHealth = 0,
            LuckyThief = true,
        });
        buffs.Add(new Buff
        {
            Name = "Ночное око",
            Description = "Тьма раскрыла тебе свои секреты. Ты видишь все ловушки в лабиринте, но заплатил за это частью своего зрения",
            BuffHealth = 0,
            BuffEvasion = -0.05,
            BuffDamage = 0,
            BuffEnemyDamage = 0,
            BuffInventorySlots = 0,
            NightEye = true,
        });
    }
    static void PrintCentered(string text)
    {
        int x = (Console.WindowWidth - text.Length) / 2;
        Console.SetCursorPosition(x, Console.CursorTop);
        Console.WriteLine(text);
    }
    static void InitializeEnemies()
    {
        Enemies.Add(new Enemy { Symbol = 'R', Name = "Крыса", HP = 1, damage = 1, chance = 0.09, Color = ConsoleColor.DarkGray, MinLevel = 1, MaxLevel = 2, evasion = 0.5});
        Enemies.Add(new Enemy { Symbol = 'S', Name = "Скелет", HP = 2, damage = 2, chance = 0.15, Color = ConsoleColor.Gray, MinLevel = 3, MaxLevel = 5, evasion = 0.2});
        Enemies.Add(new Enemy { Symbol = 'G', Name = "Гоблин", HP = 2, damage = 1, chance = 0.25, Color = ConsoleColor.DarkGreen, MinLevel = 2, MaxLevel = 3, evasion = 0.3 });
        Enemies.Add(new Enemy { Symbol = 'B', Name = "Летучая мышь", HP = 1, damage = 1, chance = 0.05, Color = ConsoleColor.DarkMagenta, MinLevel = 1, MaxLevel = 3, evasion = 0.15 });
        Enemies.Add(new Enemy { Symbol = 'P', Name = "Паук", HP = 2, damage = 1, chance = 0.05, Color = ConsoleColor.DarkGray, MinLevel = 3, MaxLevel = 5, evasion = 0.15 });
        Enemies.Add(new Enemy { Symbol = 'N', Name = "Гном", HP = 1, damage = 2, chance = 0.6, Color = ConsoleColor.DarkYellow, MinLevel = 3, MaxLevel = 4, evasion = 0.1 });
        Enemies.Add(new Enemy { Symbol = 'H', Name = "Голем", HP = 8, damage = 2, chance = 0.2, Color = ConsoleColor.Gray, MinLevel = 6, MaxLevel = 8, evasion = 0.13 });
    }
    static void InitializeBoss()
    {
        bosses.Add(new Boss { SymbolB = 'T', NameB = "Тролль", HPB = 15, damageB = 3, chanceB = 1, ColorB = ConsoleColor.DarkGreen, LevelB = 5, SoulValue = 25, evasionB = 0.09 });
    }
    static Enemy GetEnemyBySymbol(char symbol)
    {
        foreach (Enemy enemy in Enemies)
        {
            if (enemy.Symbol == symbol) return enemy;
        }
        return null;
    }
    static Boss GetBossBySymbol(char symbol)
    {
        foreach (Boss bosses in bosses)
        {
            if (bosses.SymbolB == symbol) return bosses;
        }
        return null;
    }
    static void Draw(int HP, int K)
    {
        Console.Clear();
        int CenterX = (Console.WindowWidth - map.GetLength(1)) / 2;
        int CenterY = (Console.WindowHeight - map.GetLength(0)) / 2;
        Console.SetCursorPosition(CenterX, CenterY - 1);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Уровень: {Level}");
        Console.ResetColor();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            Console.SetCursorPosition(CenterX, CenterY + y);
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == '@')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if ((map[y, x] == '#') || map[y, x] == '%')
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (map[y, x] == 'C')
                {
                    Console.ForegroundColor= ConsoleColor.Yellow;
                }
                else if (map[y, x] == '>')
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (map[y, x] == '$')
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                else if (map[y, x] == 'A' || map[y, x] == 'a')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                else if (map[y, x] == 't')
                {
                    if (hasNightEye == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (hasNightEye == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                else 
                {
                    Boss bosses = GetBossBySymbol(map[y, x]);
                    Enemy enemy = GetEnemyBySymbol(map[y, x]);
                    if (bosses != null)
                    {
                        Console.ForegroundColor = bosses.ColorB;
                    }
                    else if (enemy != null)
                    {
                        Console.ForegroundColor = enemy.Color;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                }
                Console.Write(map[y, x]);
                Console.ResetColor();                
            }
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(CenterX, Console.CursorTop);
        Console.WriteLine($"Здоровье игрока: {HP}   ");
        Console.SetCursorPosition(CenterX, Console.CursorTop);
        Console.WriteLine($"Количество душ: {Souls}");
        Console.ResetColor();
        Console.SetCursorPosition(CenterX, Console.CursorTop);
        Console.Write($"Врагов повержено: {K}   ");
        Console.SetCursorPosition(CenterX, Console.CursorTop);
        Console.Write($"Количество шагов: {steps}");
        Console.WriteLine();
        Console.SetCursorPosition(CenterX, Console.CursorTop);
        Console.WriteLine($"Предметов в инвентаре: {Inventory.Count}/{MaxInvetorySize}");
    }
    static void SpawnEnemies()
    {
        if (countBosses == 1)
        {
            return;
        }
        else if (Level % 5 == 0 && !BossSpawn && !BossDefeated)
        {
            BossSpawn = true;
            Random rnd = new Random();
            List<Boss> available = new List<Boss>();
            foreach(Boss b in bosses)
            {
                if (b.LevelB == Level)
                {
                    available.Add(b);
                }
            }
                Boss chosen  = available[rnd.Next(available.Count)];
                for (int i = 0; i < 1; i++)
                {
                    int x = rnd.Next(0, map.GetLength(1));
                    int y = rnd.Next(0, map.GetLength(0));
                    if (map[y, x] == '.')
                    {
                        map[y, x] = chosen.SymbolB;
                        enemiesHealth[y, x] = chosen.HPB;
                        countBosses++;
                    }
                    else
                    {
                        i--;
                    }
                }
        }
        else if (enemyCount == 5)
        {
            return;
        }
        else
        {
            Random rnd = new Random();
            List<Enemy> available = new List<Enemy>();
            foreach (Enemy e in Enemies)
            {
                if (e.MinLevel <= Level && e.MaxLevel > Level)
                    available.Add(e);
            }
            if (available.Count == 0) { return; }
            if (rnd.NextDouble() < 0.2)
            {
                Enemy chosen = available[rnd.Next(available.Count)];
                for (int i = 0; i < 1; i++)
                {
                    int x = rnd.Next(0, map.GetLength(1));
                    int y = rnd.Next(0, map.GetLength(0));
                    if (map[y, x] == '.')
                    {
                        map[y, x] = chosen.Symbol;
                        enemiesHealth[y, x] = chosen.HP;
                        enemyCount++;
                    }
                    else
                    {
                        i--;
                    }
                }
            }
        }
    }
  
    static List<Buff> GetRandomBuffs(int count)
    {
        List<Buff> randomBuffs = new List<Buff>();
        Random rnd = new Random();
        List<Buff> temp = new List<Buff>(buffs);

        for (int i = 0; i < count && temp.Count > 0; i++)
        {
            int index = rnd.Next(temp.Count);
            randomBuffs.Add(temp[index]);
            temp.RemoveAt(index);
        }
        return randomBuffs;
    }
    static void MoveTrollTowardsPlayer()
    {
        int bossX = -1, bossY = -1;
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 'T')
                {
                    bossX = x;
                    bossY = y;
                    break;
                }
            }
        }
        if (bossX == -1) return;
        int newX = bossX, newY = bossY;
        if (bossX < playerX) newX++;
        else if (bossX > playerX) newX--;
        if (bossY < playerY) newY++;
        else if (bossY > playerY) newY--;
        if (map[newY, newX] != '.') return;
        int health = enemiesHealth[bossY, bossX];
        map[bossY, bossX] = '.';
        map[newY, newX] = 'T';
        enemiesHealth[bossY, bossX] = 0;
        enemiesHealth[newY, newX] = health;
    }
    static void StartGame()
    {
        bonusHealth = 0;
        bonusEvasion = 0;
        bonusEnemyDamage = 0;
        bonusDamage = 0;
        MaxInvetorySize = 5;
        List<Buff> randomBuffs = GetRandomBuffs(3);
        Buff chosen = MenuManager.ShowBuffSelection(randomBuffs);
        Inventory.Clear();
        Level = 0;
        MapGenerator.GenerateMap();
        hasBloodHarvest = chosen.BloodHarvest;
        hasLuckyThief = chosen.LuckyThief;
        hasNightEye = chosen.NightEye;
        bonusDamage += chosen.BuffDamage;
        bonusHealth += chosen.BuffHealth;
        bonusEvasion += chosen.BuffEvasion;
        bonusEnemyDamage += chosen.BuffEnemyDamage;
        MaxInvetorySize += chosen.BuffInventorySlots;
        playerHealth = 5 + bonusHealth;
        int CenterX = (Console.WindowWidth - map.GetLength(1)) / 2;
        int CenterY = (Console.WindowHeight - map.GetLength(0)) / 2;
        kill = 0;
        steps = 0;
        while (true)
        {
            Draw(playerHealth, kill);
            var key = Console.ReadKey(true).Key;
            int newX = playerX, newY = playerY;
            if (key == ConsoleKey.W) newY--;
            if (key == ConsoleKey.S) newY++;
            if (key == ConsoleKey.A) newX--;
            if (key == ConsoleKey.D) newX++;
            if (key == ConsoleKey.Escape) break;
            if (key == ConsoleKey.I) InventoryManager.ShowInventory(Inventory);
            if (map[newY, newX] == '.')
            {
                steps++;
                map[playerY, playerX] = '.';
                playerX = newX;
                playerY = newY;
                map[playerY, playerX] = '@';
                SpawnEnemies();
                Boss Boss = GetBossBySymbol('T');
                if (Boss != null && Boss.NameB == "Тролль" && !BossDefeated)
                {
                    trollStepCount++;
                    if (trollStepCount >= 3)
                    {
                        MoveTrollTowardsPlayer();
                        trollStepCount = 0;
                    }
                }
            }
            else if (map[newY, newX] == 't')
            {
                playerHealth--;
                map[newY, newX] = '.';
                PrintCentered("Вы попали в ловушку. -1 ХП");
                Console.ReadKey();
                if (playerHealth <= 0)
                {
                    MenuManager.ShowDeathStats("Ловушка");
                    break;
                }
            }
            else if (map[newY, newX] == '%')
            {
                PrintCentered("Вы нашли секретную комнату!");
                map[newY, newX] = '.';
                int roomX = newX - 2;
                int roomY = newY - 2;
                for (int y = roomY; y < roomY + 5; y++)
                {
                    for (int x = roomX; x < roomX + 5; x++)
                    {
                        if (x >= 0 && x < map.GetLength(1) && y >= 0 && y < map.GetLength(0))
                        {
                            if (map[y, x] == '#') 
                                map[y, x] = '.';
                        }
                    }
                }
                map[newY, newX] = 'C';
            }
            else if (map[newY, newX] == 'A')
            {
                if (steps <= 40)
                {
                    bonusDamage++;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    PrintCentered("Вы принесли жертву богам. +1 к урону");
                    Console.ReadKey();
                }
                else if (steps <= 60)
                {
                    bonusHealth++;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    PrintCentered("Вы принесли жертву богам Подземья. +1 к максимальному здоровью");
                    Console.ReadKey();
                }
                else if (steps <= 80)
                {
                    bonusEvasion += 0.05;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    PrintCentered("Вы принесли жертву богам Подземья. Вы стали ловчее");
                    Console.ReadKey();
                }
                else if (steps <= 100)
                {
                    bonusEnemyDamage++;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    PrintCentered("Боги Подземья прокляли вас. Враги стали наносить больше урона");
                    Console.ReadKey();
                }
                else if (steps >= 101) 
                {
                    bonusEnemyDamage += 2;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    PrintCentered("Боги Подземья прокляли вас. Враги стали наносить больше урона");
                    Console.ReadKey();
                }
                map[newY, newX] = 'a';
            }
            else if (map[newY, newX] == 'a')
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                PrintCentered("Вы уже принесли жертву богам. Алтарь не активен");
                Console.ReadKey ();
            }
            else if (map[newY, newX] == '$')
            {
                MenuManager.ShopMenu();
            }
                else if ((map[newY, newX] == 'C'))
                    {
                        bool hasKey = false;
                        Item keyItem = null;
                        foreach (Item item in Inventory)
                        {
                            if (item.Type == "Ключ")
                            {
                                hasKey = true;
                                keyItem = item;
                                break;
                            }
                        }
                        if (hasKey)
                        {
                            Random rnd = new Random();
                            double roll = rnd.NextDouble();
                            Inventory.Remove(keyItem);
                            if (roll < 0.5)
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    if (Inventory.Count < MaxInvetorySize)
                                    {
                                        Item potion = new Item();
                                        potion.Type = "Зелье";
                                int rand = rnd.Next(3);
                                if (rand == 0)
                                {
                                    potion.Name = "Малое зелье лечения";
                                    potion.Value = 1;
                                    Inventory.Add(potion);
                                    PrintCentered("Сундук открыт! Вы получлили Малое зелье лечения");
                                }
                                else if (rand == 1)
                                {
                                    potion.Name = "Среднне зелье лечения";
                                    potion.Value = 3;
                                    Inventory.Add(potion);
                   PrintCentered("Сундук открыт! Вы получлили Среднее зелье лечения");
                                }
                                else
                                {
                                    potion.Name = "Большое зелье лечения";
                                    potion.Value = 5;
                                    Inventory.Add(potion);
                           PrintCentered("Сундук открыт! Вы получлили Большое зелье лечения");
                                }
                                    }
                                    else
                                    {
                     PrintCentered("Инвентарь полон");
                                        Console.ReadKey();
                                    }
                                }
                            }
                            else if (roll < 0.6)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    if (Inventory.Count < MaxInvetorySize)
                                    {
                                        Item potion = new Item();
                                        potion.Name = "Малое зелье лечения";
                                        potion.Type = "Зелье";
                                        potion.Value = 1;
                                        Inventory.Add(potion);
                        PrintCentered("Сундук открыт! Вы получлили Зелье лечения");
                                    }
                                    else
                                    {
                        PrintCentered("Инвентарь полон");
                                        Console.ReadKey();
                                    }
                                }
                            }
                            else if (roll < 0.61)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    if (Inventory.Count < MaxInvetorySize)
                                    {
                                        Item potion = new Item();
                                        potion.Name = "Малое зелье лечения";
                                        potion.Type = "Зелье";
                                        potion.Value = 1;
                                        Inventory.Add(potion);
                          PrintCentered("Сундук открыт! Вы получлили Зелье лечения");
                                    }
                                    else
                                    {
                                   PrintCentered("Инвентарь полон");
                                        Console.ReadKey();
                                    }
                                }
                            }
                            else
                            {
                PrintCentered("В сундуке только пыль");
                            }
                            map[newY, newX] = '.';
                            Console.ReadKey();
                        }
                        else
                        {
  PrintCentered("Чтобы открыть сундук, необходим Ключ");
                            Console.ReadKey();
                        }
                    }
                    else if (map[newY, newX] == '>')
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
          PrintCentered("Вы хотите перейти на следующий уровень? (Y)");
                        Console.ResetColor();
                        var key_ = Console.ReadKey(true).Key;
                        if (key_ == ConsoleKey.Y)
                        {
                            MapGenerator.GenerateMap();
                        }
                    }
            Boss boss = GetBossBySymbol(map[newY, newX]);
            if (boss != null)
            {
                Combat.FightBoss(boss, newY, newX);
                if (playerHealth <= 0)
                {
                    MenuManager.ShowDeathStats(boss.NameB);
                    break;
                }
            }
            Enemy enemy = GetEnemyBySymbol(map[newY, newX]);
            if (enemy != null)
            {
                Combat.Fight(enemy, newY, newX);
                if (playerHealth <= 0)
                {
                    MenuManager.ShowDeathStats(enemy.Name);
                    break;
                }
            }
        } 
    }
    static void Main()
    {
        SaveSystem.LoadSouls();
        while (true)
        {
            InitializeBuffs();
            InitializeBoss();
            InitializeEnemies();
            MenuManager.ShowMenu();
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
            {
                StartGame();
            }
            else if (key == ConsoleKey.D2 || key == ConsoleKey.Escape)
            {
                SaveSystem.SaveSouls();
                break;
            }
        }
    }
}