using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Security.Cryptography;

namespace SnakeGame
{
    enum MapType
    {
        Idle = 0,
        Food = 1,
        Player = 2,
        Wall = 3
    }

    class Tile
    {
        public MapType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }

        public Tile()
        {
            Type = MapType.Idle;
        }

    }

    class Map()
    {
        Tile[] tiles;
        bool isTile = false;
        Vector2 goVector = new Vector2();
        List<Tile> player = new List<Tile>();
        public Map(int mapSize) : this()
        {
            tiles = new Tile[mapSize * mapSize];
            bool isPlayer = false;

            for (int i = 0; i < mapSize * mapSize; i++)
            {
                tiles[i] = new Tile();
                tiles[i].Index = i;
                tiles[i].X = i % mapSize;
                tiles[i].Y = i / mapSize;

                if (tiles[i].X == 0 || tiles[i].X == mapSize - 1 || tiles[i].Y == 0 || tiles[i].Y == mapSize - 1)
                {
                    tiles[i].Type = MapType.Wall;
                }
                else if (isPlayer == false)
                {
                    int r = new Random().Next(1, 100);
                    if (r > 60)
                    {
                        isPlayer = true;
                        tiles[i].Type = MapType.Player;
                        player.Add(tiles[i]);
                    }
                }
            }
                    MakeFood(mapSize);
        }

        void MakeFood(int mapSize)
        {
            while (true)
            {
                isTile = false;
                int r = new Random().Next(1, (mapSize * mapSize));
                if(tiles[r].X == 0 || tiles[r].X == mapSize - 1 || tiles[r].Y == 0 || tiles[r].Y == mapSize - 1 
                    || player[0].Index == tiles[r].Index)
                {
                    continue;
                }
                tiles[r].Type = MapType.Food;
                break;
            }
        }

        void Move()
        {
            const int WAIT_TICK = 1000 / 30;
            int lastTick = 0;

            do
            {
                int currentTick = System.Environment.TickCount;
                if (currentTick - lastTick < WAIT_TICK)
                    continue;
                lastTick = currentTick;

                ConsoleKeyInfo keys;
                if(Console.KeyAvailable)
                {
                    keys = Console.ReadKey(true);
                    switch(keys.Key)
                    {
                        case ConsoleKey.UpArrow:
                            {
                                goVector.Y = 1;
                                goVector.X = 0;
                                break;
                            }
                        case ConsoleKey.RightArrow:
                            {
                                goVector.X = 1;
                                goVector.Y = 0;
                                break;
                            }
                        case ConsoleKey.LeftArrow:
                            {
                                goVector.X = -1;
                                goVector.Y = 0;
                                break;
                            }
                        case ConsoleKey.DownArrow:
                            {
                                goVector.Y = -1;
                                goVector.X = 0;
                                break;
                            }
                    }
                }
                if (UpdatePos() == false)
                    break;
                else
                {
                    UpdateFrame();
                }
                Task.Delay(1000);


            } while (true);
        }

        void UpdateFrame()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            int lateY = 0;
            foreach (var tile in tiles)
            {
                if (tile.Y != lateY)
                {
                    Console.WriteLine("");
                }
                if (tile.X == 0)
                    Console.Write("\t\t\t\t\t");
                switch (tile.Type)
                {
                    case MapType.Idle:
                        {
                            Console.Write("  ");
                            break;
                        }
                    case MapType.Food:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("★ ");
                            Console.ResetColor();
                            break;
                        }
                    case MapType.Player:
                        {
                            Console.Write("● ");
                            break;
                        }
                    case MapType.Wall:
                        {
                            Console.Write("□ ");
                            break;
                        }
                }
                lateY = tile.Y;
            }
            Console.WriteLine();
        }

        bool CheckType(MapType type)
        {
            if (type == MapType.Player || type == MapType.Wall)
            {
                return false;
            }
                return true;
        }

        void GameOver()
        {
            Console.WriteLine("\t\t===============================================================================================");
            Console.WriteLine("\t\t\t\t\t\t\tPlayerDie");
            Console.WriteLine("\t\t===============================================================================================");
        }

        bool UpdatePos()
        {
            int mapSize = (int)Math.Sqrt(tiles.Length);
            
               Tile nextTile;

            if (goVector.X < 0)
            {
                nextTile = tiles[(player[0].X - 1) + (player[0].Y * mapSize)];
                if (CheckType(nextTile.Type) == false)
                {
                    GameOver();
                    return false;
                }else if(nextTile.Type == MapType.Food)
                {
                    nextTile.Type = MapType.Player;
                    player.Add(player[0]);
                    player[0] = nextTile;
                    MakeFood(mapSize);
                    return true;
                }
                Tile last = player[0];
                player[0].Type = nextTile.Type;
                nextTile.Type = MapType.Player;
                player[0] = nextTile;

                if (player.Count > 1)
                {
                    player[player.Count - 1].Type = MapType.Idle;
                    last.Type = MapType.Player;
                    player[player.Count - 1] = last;
                    player.Insert(1, player[player.Count - 1]);
                    player.RemoveAt(player.Count - 1);
                }
            }
            else if (goVector.X > 0)
            {
                nextTile = tiles[(player[0].X + 1) + (player[0].Y * mapSize)];
                if (CheckType(nextTile.Type) == false)
                {
                    GameOver();
                    return false;
                }
                else if (nextTile.Type == MapType.Food)
                {
                    nextTile.Type = MapType.Player;
                    player.Add(player[0]);
                    player[0] = nextTile;
                    MakeFood(mapSize);
                    return true;
                }
                Tile last = player[0];
                player[0].Type = nextTile.Type;
                nextTile.Type = MapType.Player;
                player[0] = nextTile;
                if (player.Count > 1)
                {
                    player[player.Count - 1].Type = MapType.Idle;
                    last.Type = MapType.Player;
                    player[player.Count - 1] = last;
                    player.Insert(1, player[player.Count - 1]);
                    player.RemoveAt(player.Count - 1);
                }
            }
            else if (goVector.Y < 0)
            {
                nextTile = tiles[player[0].X + ((player[0].Y + 1) * mapSize)];
                if (CheckType(nextTile.Type) == false)
                {
                    GameOver();
                    return false;
                }else if (nextTile.Type == MapType.Food)
                {
                    nextTile.Type = MapType.Player;
                    player.Add(player[0]);
                    player[0] = nextTile;
                    MakeFood(mapSize);
                    return true;
                }
                Tile last = player[0];
                player[0].Type = nextTile.Type;
                nextTile.Type = MapType.Player;
                player[0] = nextTile;
                if (player.Count > 1)
                {
                    player[player.Count - 1].Type = MapType.Idle;
                    last.Type = MapType.Player;
                    player[player.Count - 1] = last;
                    player.Insert(1, player[player.Count - 1]);
                    player.RemoveAt(player.Count - 1);
                }
            }
            else if (goVector.Y > 0)
            {
                nextTile = tiles[player[0].X + ((player[0].Y - 1) * mapSize)];
                if (CheckType(nextTile.Type) == false)
                {
                    GameOver();
                    return false;
                }else if (nextTile.Type == MapType.Food)
                {
                    nextTile.Type = MapType.Player;
                    player.Add(player[0]);
                    MakeFood(mapSize);
                }
                Tile last = player[0];
                player[0].Type = nextTile.Type;
                nextTile.Type = MapType.Player;
                player[0] = nextTile;
                if (player.Count > 1)
                {
                    player[player.Count - 1].Type = MapType.Idle;
                    last.Type = MapType.Player;
                    player[player.Count - 1] = last;
                    player.Insert(1, player[player.Count - 1]);
                    player.RemoveAt(player.Count - 1);
                }
            }


            //꼬리 따라오게 만들기
            return true;
        }

        public void Update()
        {
            Move();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int mapSize = 25;
            Map map = new Map(mapSize);

            //map.Update(mapSize);
            map.Update();

        }
    }
}
