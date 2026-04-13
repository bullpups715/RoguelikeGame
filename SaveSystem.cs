using System;
using System.Collections.Generic;
using System.Text;
using MEEm;

namespace MEEm
{
    internal class SaveSystem
    {
        public static void LoadSouls()
        {
            if (!File.Exists("souls.txt")) return;
            string[] lines = File.ReadAllLines("souls.txt");
            foreach (string line in lines)
            {
                Game.Souls = int.Parse(line);
            }

        }
        public static void SaveSouls()
        {
            using StreamWriter writer = new StreamWriter("souls.txt");
            writer.WriteLine(Game.Souls);
        }
    }
}
