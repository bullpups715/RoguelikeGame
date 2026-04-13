using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using MEEm;

namespace MEEm
{
    internal class Combat
    {
       public static void Fight(Enemy enemy, int enemyY, int enemyX)
        {
            if (enemy == null) return;
            int CenterX = (Console.WindowWidth - Game.map.GetLength(1)) / 2;
            int CenterY = (Console.WindowHeight - Game.map.GetLength(0)) / 2;
            int barStartX = (Console.WindowWidth - 30 - 10) / 2;
            Console.SetCursorPosition(barStartX, Console.CursorTop);
            Console.WriteLine($"{enemy.Name}! Бой!");
            Console.ReadKey();
            Console.SetCursorPosition(barStartX, Console.CursorTop);
            Console.WriteLine("Нажмите Space для атаки");
            DateTime startTime = DateTime.Now;
            double reactionSeconds = -1;
            while (true)
            {
                double elapsed = (DateTime.Now - startTime).TotalSeconds;
                int barLength = 30;
                int pos = (int)(elapsed /2.0* barLength);
                if (pos> barLength)
                {
                    pos = barLength;
                }
                if (elapsed < 0.15 || (elapsed > 0.85 && elapsed < 1.0))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if ((elapsed >= 0.15 && elapsed < 0.4 - Game.bonusEvasion) || (elapsed >=0.45 + Game.bonusEvasion && elapsed < 0.75 + Game.bonusEvasion))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (elapsed >= 0.4 +Game.bonusEvasion && elapsed < 0.45 + Game.bonusEvasion)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.Write("[");
                for (int i = 0; i < barLength; i++)
                {
                    Console.Write(i < pos ? '-' : ' ');
                }
                Console.Write($"] {elapsed:F2} сек");
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar)
                    {
                        reactionSeconds = elapsed;
                        break;
                    }
                }
                if (elapsed > 2.0)
                {
                    break;
                }
                Thread.Sleep(20);
            }
            Console.WriteLine();
            if (reactionSeconds < 0.15 )
            {

                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine("Вы не успели увернуться");
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Урон: {enemy.damage + Game.bonusEnemyDamage}");
                Console.ReadKey();
                Game.playerHealth -= enemy.damage + Game.bonusEnemyDamage;
            }
            else if (reactionSeconds < (0.4 - Game.bonusEvasion))
            {

                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Вы частично уклонились, но {enemy.Name} всё равно успел попасть");
                Console.ReadKey();
                if (Game.GetDamage() <= 0) { Game.enemiesHealth[enemyY, enemyX]--; }
                else { Game.enemiesHealth[enemyY, enemyX] -= Game.GetDamage();  }
                if ((enemy.damage + Game.bonusDamage) / 2 == 0) { Game.playerHealth--; }
                Game.playerHealth -= (enemy.damage + Game.bonusEnemyDamage) / 2;
            }
            else if (reactionSeconds < (0.45 + Game.bonusEvasion))
            {
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine("Идеальное уклонение");
                Console.ReadKey();
                if (Game.GetDamage() <= 0) {Game.enemiesHealth[enemyY, enemyX]--; }
                else { Game.enemiesHealth[enemyY, enemyX] -= Game.GetDamage(); }
            }
            else if (reactionSeconds >= (0.45 + Game.bonusEvasion) && reactionSeconds < (0.85 + Game.bonusEvasion))
            {
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Вы частично уклонились, но {enemy.Name} всё равно успел попасть");
                Console.ReadKey();
                if (Game.GetDamage() <= 0) {Game.enemiesHealth[enemyY, enemyX]--; }
                else { Game.enemiesHealth[enemyY, enemyX] -= Game.GetDamage(); }
                if ((enemy.damage + Game.bonusDamage) / 2 == 0) { Game.playerHealth--; }
                Game.playerHealth -= (enemy.damage + Game.bonusEnemyDamage) / 2;
            }
            else
            {
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine("Вы не успели увернуться");
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Урон: {enemy.damage + Game.bonusEnemyDamage}");
                Console.ReadKey();
                Game.playerHealth -= enemy.damage + Game.bonusEnemyDamage;
            }
            if (Game.enemiesHealth[enemyY, enemyX] > 0)
            {
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"У врага осталось {Game.enemiesHealth[enemyY, enemyX]} ХП");
                Console.ResetColor();
                Console.ReadKey();
            }
            else if (Game.enemiesHealth[enemyY, enemyX] <= 0)
            {
                if (Game.hasBloodHarvest && Game.playerHealth != Game.GetMaxHealth() && reactionSeconds >= 0.35 + Game.bonusEvasion && reactionSeconds < 0.55 + Game.bonusEvasion)
                {
                    Console.SetCursorPosition(barStartX, Console.CursorTop);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Боги излечили вас");
                    Console.ResetColor();
                    Game.playerHealth++;
                    Console.ReadKey();
                }
                Game.kill++;
                Game.Souls++;
                Game.map[enemyY, enemyX] = '.';
                Game.enemyCount--;
                Random rnd = new Random();
                if (Game.hasLuckyThief)
                {
                    if (rnd.NextDouble() < enemy.chance + 0.15)
                    {
                        if (Game.Inventory.Count < Game.MaxInvetorySize)
                        {
                            Item Key = new Item();
                            Key.Name = "Ключ от сундука";
                            Key.Type = "Ключ";
                            Key.Value = 1;
                            Game.Inventory.Add(Key);
                        }
                        else
                        {
                            Console.SetCursorPosition(barStartX, Console.CursorTop);
                            Console.WriteLine("Инвентарь полон");
                            Console.ReadKey();
                        }
                    }
                }
                else if (rnd.NextDouble() < enemy.chance)
                {
                    if (Game.Inventory.Count < Game.MaxInvetorySize)
                    {
                        Item Key = new Item();
                        Key.Name = "Ключ от сундука";
                        Key.Type = "Ключ";
                        Key.Value = 1;
                        Game.Inventory.Add(Key);
                    }
                    else
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.WriteLine("Инвентарь полон");
                        Console.ReadKey();
                    }
                }
            }
        }
       public static void FightBoss(Boss boss, int bossY, int bossX)
        {
            if (boss == null) return;
            int CenterX = (Console.WindowWidth - Game.map.GetLength(1)) / 2;
            int CenterY = (Console.WindowHeight - Game.map.GetLength(0)) / 2;
            int barStartX = (Console.WindowWidth - 30 - 10) / 2;
            int barLine = Console.CursorTop;
            Console.SetCursorPosition(barStartX, barLine);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"     {boss.NameB}! Вы в опасности!");
            Console.ResetColor();
            Console.ReadKey();
            Console.SetCursorPosition(barStartX, barLine);
            Console.WriteLine("     Нажмите Space для атаки");
            DateTime startTime = DateTime.Now;
            double reactionSeconds = -1;
            while (true)
            {
                double elapsed = (DateTime.Now - startTime).TotalSeconds;
                int barLength = 30;
                int pos = (int)(elapsed / 2.0 * barLength);
                if (pos > barLength)
                {
                    pos = barLength;
                }
                if (elapsed < 0.1 || (elapsed > 0.85 && elapsed < 1.0))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if ((elapsed >= 0.1 && elapsed < 0.4 - Game.bonusEvasion) || (elapsed >= 0.5 + Game.bonusEvasion && elapsed < 0.75 + Game.bonusEvasion))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (elapsed >= 0.4 + Game.bonusEvasion && elapsed < 0.5 + Game.bonusEvasion)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.SetCursorPosition(barStartX, barLine);
                Console.Write("[");
                for (int i = 0; i < barLength; i++)
                {
                    Console.Write(i < pos ? '-' : ' ');
                }
                Console.Write($"] {elapsed:F2} сек");
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar)
                    {
                        reactionSeconds = elapsed;
                        break;
                    }
                }
                if (elapsed > 2.0)
                {
                    break;
                }
                Thread.Sleep(20);
            }
            Console.WriteLine();
            if (reactionSeconds < 0.1)
            {

                Console.SetCursorPosition(barStartX, barLine);
                Console.WriteLine("Вы не успели увернуться");
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Урон: {boss.damageB + Game.bonusEnemyDamage}");
                Console.ReadKey();
                Game.playerHealth -= boss.damageB + Game.bonusEnemyDamage;
            }
            else if (reactionSeconds < (0.4 - Game.bonusEvasion))
            {
                Console.SetCursorPosition(barStartX, barLine);
                Console.WriteLine($"Вы частично уклонились, но {boss.NameB} всё равно успел попасть");
                Console.ReadKey();
                if (Game.GetDamage() <= 0) { Game.enemiesHealth[bossY, bossX]--; }
                else { Game.enemiesHealth[bossY, bossX] -= Game.GetDamage(); }
                if ((boss.damageB + Game.bonusDamage) / 2 == 0) { Game.playerHealth--; }
                Game.playerHealth -= (boss.damageB + Game.bonusEnemyDamage) / 2;
            }
            else if (reactionSeconds < (0.5 + Game.bonusEvasion))
            {
                Console.SetCursorPosition(barStartX, barLine);
                Console.WriteLine("Идеальное уклонение");
                Console.ReadKey();
                if (Game.GetDamage() <= 0) { Game.enemiesHealth[bossY, bossX]--; }
                else { Game.enemiesHealth[bossY, bossX] -= Game.GetDamage(); }
            }
            else if (reactionSeconds >= (0.5 + Game.bonusEvasion) && reactionSeconds < (0.85 + Game.bonusEvasion))
            {

                Console.SetCursorPosition(barStartX, barLine);
                Console.WriteLine($"Вы частично уклонились, но {boss.NameB} всё равно успел попасть");
                Console.ReadKey();
                if (Game.GetDamage() <= 0) { Game.enemiesHealth[bossY, bossX]--; }
                else { Game.enemiesHealth[bossY, bossX] -= Game.GetDamage(); }
                if ((boss.damageB + Game.bonusDamage) / 2 == 0) { Game.playerHealth--; }
                Game.playerHealth -= (boss.damageB + Game.bonusEnemyDamage) / 2;
            }
            else
            {
                Console.SetCursorPosition(barStartX, barLine);
                Console.WriteLine("Вы не успели увернуться");
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Урон: {boss.damageB + Game.bonusEnemyDamage}");
                Console.ReadKey();
                Game.playerHealth -= boss.damageB + Game.bonusEnemyDamage;
            }
            if (Game.enemiesHealth[bossY, bossX] > 0)
            {
                Console.SetCursorPosition(barStartX, barLine);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"У врага осталось {Game.enemiesHealth[bossY, bossX]} ХП");
                Console.ResetColor();
                Console.ReadKey();
            }
            else if (Game.enemiesHealth[bossY, bossX] <= 0)
            {
                if (Game.hasBloodHarvest && Game.playerHealth != Game.GetMaxHealth() && reactionSeconds >= 0.35 + Game.bonusEvasion && reactionSeconds < 0.55 + Game.bonusEvasion)
                {
                    Console.SetCursorPosition(barStartX, barLine);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Боги излечили вас");
                    Console.ResetColor();
                    Game.playerHealth++;
                    Console.ReadKey();
                }
                Game.map[bossY, bossX] = '.';
                Game.BossDefeated = true;
                Game.countBosses = 0;
                Game.Souls += boss.SoulValue;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"Вы убили босса: {boss.NameB}");
                Console.ResetColor();
                Console.ReadKey();
                Random rnd = new Random();
                while (true)
                {
                    int x = rnd.Next(0, Game.map.GetLength(1));
                    int y = rnd.Next(0, Game.map.GetLength(0));
                    if (Game.map[y, x] == '.')
                    {
                        Game.map[y, x] = '>';
                        break;
                    }
                }
                Random rnd2 = new Random();
                while (true)
                {
                    int x = rnd2.Next(0, Game.map.GetLength(1));
                    int y = rnd2.Next(0, Game.map.GetLength(0));
                    if (Game.map[y, x] == '.')
                    {
                        Game.map[y,x] = '$';
                        break;
                    }
                }
                if (Game.hasLuckyThief)
                {
                    if (rnd.NextDouble() < boss.chanceB + 0.15)
                    {
                        if (Game.Inventory.Count < Game.MaxInvetorySize)
                        {
                            Item Key = new Item();
                            Key.Name = "Ключ от сундука";
                            Key.Type = "Ключ";
                            Key.Value = 1;
                            Game.Inventory.Add(Key);
                        }
                        else
                        {
                            Console.WriteLine("Инвентарь полон");
                            Console.ReadKey();
                        }
                    }
                }
                else if (rnd.NextDouble() < boss.chanceB)
                {
                    if (Game.Inventory.Count < Game.MaxInvetorySize)
                    {
                        Item Key = new Item();
                        Key.Name = "Ключ от сундука";
                        Key.Type = "Ключ";
                        Key.Value = 1;
                        Game.Inventory.Add(Key);
                    }
                    else
                    {
                        Console.WriteLine("Инвентарь полон");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(barStartX, Console.CursorTop);
                Console.WriteLine($"У босса осталось {Game.enemiesHealth[bossY, bossX]} ХР");
                Console.ReadKey();
            }
        }
    }
}
