using System;
using System.Collections.Generic;
using System.Text;
using MEEm;

namespace MEEm
{
    internal class InventoryManager
    {
        public static void ShowInventory(List<Item> Inventory)
        {
            while (true)
            {
                Console.Clear();

                if (Inventory.Count == 0)
                {
                    Console.WriteLine("Инвентарь пуст");
                }
                else
                {
                    for (int i = 0; i < Inventory.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {Inventory[i].Name}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Выберите номер предмета");
                }
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape) break;
                if (key >= ConsoleKey.D1 && key <= ConsoleKey.D9)
                {
                    int Index = key - ConsoleKey.D1;
                    if (Index < Inventory.Count)
                    {
                        Item item = Inventory[Index];
                        if (item.Type == "Зелье")
                        {
                            Console.WriteLine("Выберете действие: ");
                            Console.WriteLine();
                            Console.WriteLine("1. Использовать");
                            Console.WriteLine("2. Выбросить");
                            Console.WriteLine("3. Отмена");
                            var key_ = Console.ReadKey(true).Key;
                            if (key_ == ConsoleKey.D1)
                            {
                                if (Game.playerHealth < Game.GetMaxHealth())
                                {
                                    if ((Game.GetMaxHealth()) - Game.playerHealth <= item.Value)
                                    {
                                        Game.playerHealth = Game.GetMaxHealth();
                                        Inventory.RemoveAt(Index);
                                        Console.WriteLine($"Вы использовали {item.Name}. Здоровье - {Game.playerHealth}");
                                        Console.ReadKey();
                                        break;
                                    }
                                    else
                                    {
                                        Game.playerHealth += item.Value;
                                        Inventory.RemoveAt(Index);
                                        Console.WriteLine($"Вы использовали {item.Name}. Здоровье - {Game.playerHealth}");
                                        Console.ReadKey();
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Здоровье на максимуме");
                                    Console.ReadKey();
                                    break;
                                }
                            }
                            else if (key_ == ConsoleKey.D2)
                            {
                                Inventory.RemoveAt(Index);
                                Console.WriteLine($"Вы выбросили {item.Name}");
                                Console.ReadKey();
                                break;
                            }
                            else if (key_ == ConsoleKey.D3)
                            {
                                break;
                            }
                        }
                        else if (item.Type == "Ключ")
                        {
                            Console.WriteLine("Выберете действие: ");
                            Console.WriteLine();
                            Console.WriteLine("1. Описание");
                            Console.WriteLine("2. Выбросить");
                            Console.WriteLine("3. Отмена");
                            var key_ = Console.ReadKey(true).Key;
                            if (key_ == ConsoleKey.D1)
                            {
                                Console.WriteLine("Это ключ, которым можно открыть сундук.");
                                Console.ReadKey(); break;
                            }
                            else if (key_ == ConsoleKey.D2)
                            {
                                Inventory.RemoveAt(Index);
                                Console.WriteLine($"Вы выбросили {item.Name}");
                                Console.ReadKey();
                                break;
                            }
                            else if (key_ == ConsoleKey.D3)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
