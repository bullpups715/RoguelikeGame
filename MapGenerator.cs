using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using MEEm;
namespace MEEm
{
    internal class Room
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Room(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    internal class MapGenerator
    {
        static bool IsSecretSpot(int y, int x)
        {
            int wallCount = 0;
            int floorCount = 0;
            if (Game.map[y - 1, x] == '#') wallCount++;
            if (Game.map[y + 1, x] == '#') wallCount++;
            if (Game.map[y, x - 1] == '#') wallCount++;
            if (Game.map[y, x + 1] == '#') wallCount++;
            if (Game.map[y - 1, x] == '.') floorCount++;
            if (Game.map[y + 1, x] == '.') floorCount++;
            if (Game.map[y, x - 1] == '.') floorCount++;
            if (Game.map[y, x + 1] == '.') floorCount++;
            return wallCount == 3 && floorCount == 1;
        }
        public static void ConnectRooms(Room a, Room b)
        {
            int startX = a.X + (a.Width) / 2;
            int startY = a.Y + (a.Height) / 2;
            int endX = b.X + (b.Width) / 2;
            int endY = b.Y + (b.Height) / 2;
            for (int x = Math.Min(startX, endX); x <= Math.Max(startX, endX); x++)
            {
               Game.map[startY, x] = '.';
            }
            for (int y = Math.Min(startY, endY); y <= Math.Max(startY, endY); y++)
            {
               Game.map[y, startX] = '.';
            }
        }
        public static void GenerateRoom(int startX, int startY, int roomWidth, int roomHeigth)
        {
            for (int y = startY; y < startY + roomHeigth; y++)
            {
                for (int x = startX; x < startX + roomWidth; x++)
                {
                    Game.map[y, x] = '.';
                }
            }

        }
        public static bool Intersects(int x, int y, int w, int h, Room other)
        {
            return !(x + w <= other.X ||
                     x >= other.X + other.Width ||
                     y + h <= other.Y ||
                     y >= other.Y + other.Height);
        }
        public static void GenerateMap()
        {
            Game.Level++;
            Game.enemyCount = 0;
            Game.BossDefeated = false;
           Game.BossSpawn = false;
            for (int y = 0; y < Game.map.GetLength(0); y++)
            {
                for (int x = 0; x < Game.map.GetLength(1); x++)
                {
                    Game.map[y, x] = '#';
                }
            }
            if (Game.Level % 5 != 0)
            {
                List<Room> rooms = new List<Room>();
                Random rnd = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int w = rnd.Next(3, 8);
                    int h = rnd.Next(3, 6);
                    int x = rnd.Next(1, Game.map.GetLength(1) - w - 1);
                    int y = rnd.Next(1, Game.map.GetLength(0) - h - 1);
                    bool overlap = false;
                    foreach (Room room in rooms)
                    {
                        if (Intersects(x, y, w, h, room))
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                    {
                        GenerateRoom(x, y, w, h);
                        rooms.Add(new Room(x, y, w, h));
                    }
                }
                for (int i = 0; i < rooms.Count; i++)
                {
                    ConnectRooms(rooms[0], rooms[i]);
                }
                for (int i = 0; i < rooms.Count - 1; i++)
                {
                    ConnectRooms(rooms[i], rooms[i + 1]);
                }
                int secretY = rnd.Next(2, Game.map.GetLength(0) - 3);
                int secretX = rnd.Next(2, Game.map.GetLength(1) - 3);
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    int x = rnd.Next(1, Game.map.GetLength(1) - 1);
                    int y = rnd.Next(1, Game.map.GetLength(0) - 1);
                    if (Game.map[y, x] == '#' && IsSecretSpot(y, x))
                    {
                        Game.map[y, x] = '%';
                        break;
                    }
                }
                if (rooms.Count > 0)
                {
                    if (Game.Level % 5 != 0)
                    {
                        while (true)
                        {
                            Room randomRoom = rooms[rnd.Next(rooms.Count)];
                            int x = rnd.Next(0, Game.map.GetLength(1));
                            int y = rnd.Next(0, Game.map.GetLength(0));
                            if (Game.map[y, x] == '.')
                            {
                                Game.map[y, x] = '>';
                                break;
                            }
                        }
                    }
                }
                if (rooms.Count > 0)
                {
                    while (true)
                    {
                        Room randomRoom = rooms[rnd.Next(rooms.Count)];
                        int x = rnd.Next(0, Game.map.GetLength(1));
                        int y = rnd.Next(0, Game.map.GetLength(0));
                        if (Game.map[y, x] == '.')
                        {
                            Game.map[y, x] = 't';
                            break;
                        }
                    }
                }
                if (rooms.Count > 0)
                {
                    if (Game.Level == 2)
                    {
                        while (true)
                        {
                            Room randomRoom = rooms[rnd.Next(rooms.Count)];
                            int x = rnd.Next(0, Game.map.GetLength(1));
                            int y = rnd.Next(0, Game.map.GetLength(0));
                            if (Game.map[y, x] == '.')
                            {
                                Game.map[y, x] = 'A';
                                break;
                            }
                        }
                    }
                }
                if (rooms.Count > 0)
                {
                    if (Game.Level % 5 != 0)
                    {
                        while (true)
                        {
                            Room randomRoom = rooms[rnd.Next(rooms.Count)];
                            int x = rnd.Next(0, Game.map.GetLength(1));
                            int y = rnd.Next(0, Game.map.GetLength(0));
                            if (Game.map[y, x] == '.')
                            {
                                Game.map[y, x] = 'C';
                                break;
                            }
                        }
                    }
                }
                Game.playerX = rooms[0].X + rooms[0].Width / 2;
                Game.playerY = rooms[0].Y + rooms[0].Height / 2;
                Game.map[Game.playerY, Game.playerX] = '@';
            }
            else
            {
                int roomWidth = 20;
                int roomHeight = 18;
                int startX = (Game.map.GetLength(1) - roomWidth) / 2;
                int startY = (Game.map.GetLength(0) - roomHeight) / 2;
                for (int y = startY; y < startY + roomHeight; y++)
                {
                    for (int x = startX; x < startX + roomWidth; x++)
                    {
                        Game.map[y, x] = '.';
                    }
                }

                Game.playerX = startX + roomWidth / 2;
                Game.playerY = startY + roomHeight / 2;
                Game.map[Game.playerY, Game.playerX] = '@';
            }
        }

    }
}
