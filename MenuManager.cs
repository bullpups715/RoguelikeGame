using MEEm;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace MEEm
{
    class ShopItem
    {
        public string Name;
        public int Price;
        public Item Item;
    }
    internal class MenuManager
    {
        public static Buff ShowBuffSelection(List<Buff> buffs)
        {
            Console.Clear();
            Console.WriteLine("=== ВЫБЕРИТЕ БАФФ ===");
            Console.WriteLine();
            for (int i = 0; i < buffs.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {buffs[i].Name}");
                Console.WriteLine($"   {buffs[i].Description}");
                Console.WriteLine();
            }

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.D1) return buffs[0];
                if (key == ConsoleKey.D2) return buffs[1];
                if (key == ConsoleKey.D3) return buffs[2];
            }
        }
        public static void ShopMenu()
        {
            List<ShopItem> shopItems = new List<ShopItem>();
            shopItems.Add(new ShopItem
            {
                Name = "Малое зелье лечения",
                Price = 15,
                Item = new Item { Name = "Малое зелье лечения", Type = "Зелье", Value = 1, }
            });
            shopItems.Add(new ShopItem
            {
                Name = "Среднее зелье лечения",
                Price = 35,
                Item = new Item { Name = "Среднее зелье лечения", Type = "Зелье", Value = 3, }
            });
            shopItems.Add(new ShopItem
            {
                Name = "Большое зелье лечения",
                Price = 50,
                Item = new Item { Name = "Большое зелье лечения", Type = "Зелье", Value = 5, }
            });
            shopItems.Add(new ShopItem
            {
                Name = "Ключ от сундука",
                Price = 20,
                Item = new Item { Name = "Ключ от сундука", Type = "Ключ", Value = 1, }
            });
            while (true) 
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=== Магазин ===");
                Console.WriteLine();
                Console.ResetColor();
                Console.WriteLine($"Количество душ: {Game.Souls}");
                Console.WriteLine();
                for (int i = 0; i < shopItems.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {shopItems[i].Name}");
                }
                Console.WriteLine();
                Console.WriteLine("Выберите номер предмета");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape) break;
                if (key >= ConsoleKey.D1 && key <= ConsoleKey.D9)
                {
                    int Index = key - ConsoleKey.D1;
                    if (Index < shopItems.Count)
                    {
                        if (Game.Souls >= shopItems[Index].Price)
                        {
                            if(Game.Inventory.Count < Game.MaxInvetorySize)
                            {
                                Game.Souls -= shopItems[Index].Price;
                                Game.Inventory.Add(shopItems[Index].Item);
                                shopItems.RemoveAt(Index);
                                Console.WriteLine("Товар куплен!");
                                Console.ReadKey();
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("Инвентарь запонен");
                            }
                        } 
                        else
                        {
                            Console.WriteLine("Недостаточно душ");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }
        public static void ShowMenu()
        {
            Console.Clear();
            int CenterY = (Console.WindowHeight - Game.map.GetLength(0)) / 2;
            int startY = Math.Max(0, CenterY - 1);

            string[] lines = {
        "=== ПОДЗЕМЬЕ ===",
        "",
        "1. Новая игра",
        "2. Выход"
    };

            for (int i = 0; i < lines.Length; i++)
            {
                int CenterX = (Console.WindowWidth - lines[i].Length) / 2;
                Console.SetCursorPosition(Math.Max(0, CenterX), startY + i);

                if (i == 0)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ResetColor();

                Console.WriteLine(lines[i]);
            }
            Console.ResetColor();
        }
        public static void ShowDeathStats(string e)
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine("=== Игра окончена ===");
            Console.WriteLine();
            Console.WriteLine($"Этаж Подземья: {Game.Level}");
            Console.WriteLine($"Количество убийств: {Game.kill}");
            Console.WriteLine($"Количество шагов: {Game.steps}");
            Console.WriteLine($"Количество предметов в инвентаре: {Game.Inventory.Count}");
            Console.WriteLine($"Вы были убиты: {e}");
            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню");
            Console.ReadKey();
        }
    }
}
