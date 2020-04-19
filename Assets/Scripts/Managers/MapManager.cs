using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Clown
{
    public struct CellData 
    {
        public CellData(int _direction, Vector3Int _cell)
        {
            direction = _direction;
            cell = _cell;
        }
        public int direction {get;}
        public Vector3Int cell {get;}
    }
    public class MapManager : MonoBehaviour
    {
        [NonSerialized] public static MapManager s;
        [NonSerialized] public Tilemap tilemap;
        // Dear lord forgive me for this jank
        [NonSerialized] public List<Vector3Int> homeCells = new List<Vector3Int>();
        [NonSerialized] public Dictionary<Vector3Int, CellData> homeCellData = new Dictionary<Vector3Int, CellData>();
        [NonSerialized] public List<Vector3Int> sewerCells = new List<Vector3Int>();
        [NonSerialized] public Dictionary<Vector3Int, Vector3> sewerCellData = new Dictionary<Vector3Int, Vector3>();
        [NonSerialized] public List<Vector3Int> nodes = new List<Vector3Int>();
        [NonSerialized] public List<List<Vector3Int>> adjacentNodes = new List<List<Vector3Int>>();

        public RoadTile roadTile;
        public SewerTile sewerTile;
        public YardTile yardTile;
        public VoidTile voidTile;
        public HomeEntryTile homeEntryTile;
        public HomeOtherTile homeOtherTile;

        public static int[,] LEVEL_DEFINITION = new int[40,30] {
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,1,0,0,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,1,0,0,0, 0,0,0,0,0, 0,0,1,0,0},
            {1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},

            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,1,1, 1,1,1,1,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,1,1},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},

            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,0,0,0,1, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,1,1,1,1, 1,1,1,1,1, 1,1,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,1,0,0,0, 0,0,0,0,1, 0,0,1,0,0},
            {1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,0,0,0, 0,0,0,0,1, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,1,0,0},

            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,1, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,1,1, 1,1,1,1,1, 0,0,0,0,1, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,1,0, 0,0,0,0,1, 0,0,0,0,1, 0,0,1,0,0},

            {0,0,1,0,0, 0,0,1,1,1, 1,1,1,1,0, 0,0,0,0,1, 0,0,0,0,1, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,1,0, 0,0,0,0,1, 0,0,0,0,1, 1,1,1,1,1},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,1,1, 1,1,1,1,1, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,1,1,1},

            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,1,0,0},
            {1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 1,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 1,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0},

            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 1,1,1,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 0,0,0,1,0, 0,0,0,0,0, 0,0,1,1,1},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 0,0,0,1,1, 1,1,1,1,1, 1,1,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 0,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,1,1, 1,1,1,1,0, 0,0,0,0,0, 0,0,1,0,0},

            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 0,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,1,0,0, 0,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0},
            {0,0,1,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,1,0, 0,0,0,0,0, 0,0,1,0,0}
        };

        void Awake()
        {
            if (s == null)
            {
                s = this;
            }
            else if (s != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            tilemap = FindObjectOfType<Tilemap>();
        }

        public void CreateMap(int level)
        {
            int maxX = LEVEL_DEFINITION.GetLength(0);
            int maxY = LEVEL_DEFINITION.GetLength(1);
            // First create the outer border and put the roads in
            for (int x = -1; x <= maxX; x++)
            {
                for (int y = -1; y <= maxY; y++)
                {
                    if (x == -1 || x == maxX || y == -1 || y == maxY)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), voidTile);
                    }
                    else if (LEVEL_DEFINITION[x, y] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
                    }
                }
            }

            // Put the yards in
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (LEVEL_DEFINITION[x, y] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), yardTile);
                    }
                }
            }

            // Set up all the houses. Unfortunately my brain is not big enough to do this generation procedurally within 2 days...
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(0, 0, 0), new int[]{0, 1});
            PlaceHouses(ChooseHouseSizes(10), new Vector3Int(3, 0, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(13), new Vector3Int(14, 0, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(9), new Vector3Int(28, 0, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(38, 0, 0), new int[]{1, 2});

            PlaceHouses(ChooseHouseSizes(13), new Vector3Int(0, 3, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(10), new Vector3Int(3, 3, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(4), new Vector3Int(14, 3, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(9), new Vector3Int(16, 3, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(4), new Vector3Int(25, 3, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(28, 3, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(30, 3, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(15), new Vector3Int(38, 3, 0), new int[]{2});

            PlaceHouses(ChooseHouseSizes(5), new Vector3Int(3, 5, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(6), new Vector3Int(8, 7, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(11, 5, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(9), new Vector3Int(16, 5, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(28, 10, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(35, 5, 0), new int[]{0});

            PlaceHouses(ChooseHouseSizes(11), new Vector3Int(3, 8, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(6), new Vector3Int(5, 8, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(4), new Vector3Int(7, 14, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(11, 14, 0), new int[]{0, 1, 3});
            PlaceHouses(ChooseHouseSizes(9), new Vector3Int(14, 8, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(4), new Vector3Int(16, 8, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(4), new Vector3Int(21, 8, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(11), new Vector3Int(25, 8, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(6), new Vector3Int(28, 13, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(5), new Vector3Int(35, 13, 0), new int[]{0, 2});

            PlaceHouses(ChooseHouseSizes(10), new Vector3Int(0, 17, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(6), new Vector3Int(3, 20, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(6), new Vector3Int(5, 17, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(9, 20, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(12, 17, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(16, 13, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(3), new Vector3Int(18, 10, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(3), new Vector3Int(19, 14, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(3), new Vector3Int(19, 17, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(3), new Vector3Int(21, 10, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(7), new Vector3Int(23, 13, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(3), new Vector3Int(27, 19, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(28, 16, 0), new int[]{0, 1, 2, 3});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(30, 19, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(31, 15, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(33, 19, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(35, 19, 0), new int[]{0});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(38, 19, 0), new int[]{2});

            PlaceHouses(ChooseHouseSizes(5), new Vector3Int(3, 22, 0), new int[]{2});
            PlaceHouses(ChooseHouseSizes(3), new Vector3Int(5, 25, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(9), new Vector3Int(12, 25, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(14, 22, 0), new int[]{1});
            PlaceHouses(ChooseHouseSizes(5), new Vector3Int(18, 20, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(8), new Vector3Int(22, 25, 0), new int[]{1});
            
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(0, 28, 0), new int[]{0, 3});
            PlaceHouses(ChooseHouseSizes(5), new Vector3Int(3, 28, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(12), new Vector3Int(9, 28, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(22, 28, 0), new int[]{0, 2, 3});
            PlaceHouses(ChooseHouseSizes(6), new Vector3Int(25, 28, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(5), new Vector3Int(32, 28, 0), new int[]{3});
            PlaceHouses(ChooseHouseSizes(2), new Vector3Int(38, 28, 0), new int[]{2, 3});

            // Pick some road tiles to replace with sewer tiles and remember their locations
            int sewerCount = Math.Max(15 - GameManager.s.level, 4);
            while (sewerCount > 0)
            {
                Vector3Int cell = new Vector3Int(UnityEngine.Random.Range(0, LEVEL_DEFINITION.GetLength(0)), UnityEngine.Random.Range(0, LEVEL_DEFINITION.GetLength(1)), 0);
                Tile tile = tilemap.GetTile(cell) as Tile;
                int theCount = 0;
                if (tile is RoadTile && !sewerCells.Exists(x => x == cell))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3Int newCell = new Vector3Int(cell.x + Constants.ORTHOGONAL_DIRECTIONS[i,0], cell.y + Constants.ORTHOGONAL_DIRECTIONS[i,1], cell.z);
                        Tile adjacentTile = tilemap.GetTile(newCell) as Tile;
                        int direction = (i + 2) % 4;
                        if (adjacentTile is RoadTile || 
                            adjacentTile is SewerTile || 
                            adjacentTile is VoidTile || 
                            (adjacentTile is HomeEntryTile && homeCellData[newCell].direction == direction)
                        )
                        {
                            theCount += 1;
                        }
                    }
                    if (theCount < 4)
                    {
                        sewerCells.Add(cell);
                        tilemap.SetTile(cell, sewerTile);
                        sewerCount -= 1;
                    }
                }
            }
        }

        private List<int> ChooseHouseSizes(int length)
        {
            // length must be >= 2
            int[] houseChance = new int[] {4, 4, 4, 3, 3, 2};
            List<int> houseSizes = new List<int>();
            int remaining = length;
            while (remaining >= 4)
            {
                int houseSize = houseChance[UnityEngine.Random.Range(0, houseChance.Length)];
                houseSizes.Add(houseSize);
                remaining = remaining - houseSize;
            }
            if (remaining == 3 || remaining == 2)
            {
                houseSizes.Add(remaining);
            }
            else if (remaining == 1)
            {
                int houseIndex = houseSizes.FindIndex(x => x == 2 || x == 3);
                if (houseIndex == -1)
                {
                    int anotherTry = houseSizes.FindIndex(x => x == 4);
                    if (anotherTry > -1)
                    {
                        // Eat the rich
                        houseSizes[anotherTry] = houseSizes[anotherTry] - 1;
                        houseSizes.Add(2);
                    }
                    else
                    {
                        // Ya fucked up this is a special case
                        houseSizes = new List<int>(){2, 2};
                    }
                }
                else
                {
                    houseSizes[houseIndex] = houseSizes[houseIndex] + 1;
                }
            }

            return houseSizes;
        }

        private void PlaceHouses(List<int> houseSizes, Vector3Int startingCell, int[] directions)
        {
            // Assumes there's at least a 2x2 space to hold the houses.
            int direction = directions.Length > 1 ? directions[UnityEngine.Random.Range(0, directions.Length)] : directions[0];
            int countSize = direction % 2 == 0 ? startingCell.y : startingCell.x;
            foreach (int houseSize in houseSizes)
            {
                int yardSide = (houseSize == 3 || houseSize == 4) ? UnityEngine.Random.Range(0, 2) : 0;
                if (houseSize == 4)
                {
                    int yardCell = countSize + (yardSide == 1 ? 0 : 2);
                    if (direction % 2 == 0)
                    {
                        // Y-axis travel
                        tilemap.SetTile(new Vector3Int(startingCell.x, yardCell, 0), null);
                        tilemap.SetTile(new Vector3Int(startingCell.x + 1, yardCell, 0), null);
                        tilemap.SetTile(new Vector3Int(startingCell.x, yardCell, 0), yardTile);
                        tilemap.SetTile(new Vector3Int(startingCell.x + 1, yardCell, 0), yardTile);

                        tilemap.SetTile(new Vector3Int(startingCell.x, yardCell + 1, 0), null);
                        tilemap.SetTile(new Vector3Int(startingCell.x + 1, yardCell + 1, 0), null);
                        tilemap.SetTile(new Vector3Int(startingCell.x, yardCell + 1, 0), yardTile);
                        tilemap.SetTile(new Vector3Int(startingCell.x + 1, yardCell + 1, 0), yardTile);
                    }
                    else
                    {
                        // X-axis travel
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y, 0), null);
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y + 1, 0), null);
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y, 0), yardTile);
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y + 1, 0), yardTile);

                        tilemap.SetTile(new Vector3Int(yardCell + 1, startingCell.y, 0), null);
                        tilemap.SetTile(new Vector3Int(yardCell + 1, startingCell.y + 1, 0), null);
                        tilemap.SetTile(new Vector3Int(yardCell + 1, startingCell.y, 0), yardTile);
                        tilemap.SetTile(new Vector3Int(yardCell + 1, startingCell.y + 1, 0), yardTile);
                    }
                }
                if (houseSize == 3)
                {
                    int yardCell = countSize + (yardSide == 1 ? 0 : 2);
                    if (direction % 2 == 0)
                    {
                        // Y-axis travel
                        tilemap.SetTile(new Vector3Int(startingCell.x, yardCell, 0), null);
                        tilemap.SetTile(new Vector3Int(startingCell.x + 1, yardCell, 0), null);
                        tilemap.SetTile(new Vector3Int(startingCell.x, yardCell, 0), yardTile);
                        tilemap.SetTile(new Vector3Int(startingCell.x + 1, yardCell, 0), yardTile);
                    }
                    else
                    {
                        // X-axis travel
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y, 0), null);
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y + 1, 0), null);
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y, 0), yardTile);
                        tilemap.SetTile(new Vector3Int(yardCell, startingCell.y + 1, 0), yardTile);
                    }
                }

                int newStart = countSize + (yardSide * (houseSize - 2));
                Vector3Int[] newHomeCells = new Vector3Int[] {};
                if (direction % 2 == 0)
                {
                     newHomeCells = new Vector3Int[] {
                        new Vector3Int(startingCell.x, newStart, 0),
                        new Vector3Int(startingCell.x + 1, newStart, 0),
                        new Vector3Int(startingCell.x, newStart + 1, 0),
                        new Vector3Int(startingCell.x + 1, newStart + 1, 0),
                    };
                }
                else 
                {
                    newHomeCells = new Vector3Int[] {
                        new Vector3Int(newStart, startingCell.y, 0),
                        new Vector3Int(newStart, startingCell.y + 1, 0),
                        new Vector3Int(newStart + 1, startingCell.y, 0),
                        new Vector3Int(newStart + 1, startingCell.y + 1, 0),
                    };
                }
                CellData newHomeCellData = new CellData(direction, new Vector3Int(startingCell.x, newStart, 0));
                Tile[] homeTiles = new Tile[]{};
                Vector3Int homeCell = new Vector3Int(startingCell.x + 1, newStart, 0);

                switch (direction)
                {
                    case 0:
                        homeCell = new Vector3Int(startingCell.x + 1, newStart, 0);
                        homeTiles = new Tile[]{homeOtherTile, homeEntryTile, homeOtherTile, homeOtherTile};
                        break;
                    case 1:
                        homeCell = new Vector3Int(newStart + 1, startingCell.y + 1, 0);
                        homeTiles = new Tile[]{homeOtherTile, homeOtherTile, homeOtherTile, homeEntryTile};
                        break;
                    case 2:
                        homeCell = new Vector3Int(startingCell.x, newStart + 1, 0);
                        homeTiles = new Tile[]{homeOtherTile, homeOtherTile, homeEntryTile, homeOtherTile};
                        break;
                    case 3:
                        homeCell = new Vector3Int(newStart, startingCell.y, 0);
                        homeTiles = new Tile[]{homeEntryTile, homeOtherTile, homeOtherTile, homeOtherTile};
                        break;
                }
                newHomeCellData = new CellData(direction, homeCell);
                homeCells.Add(homeCell);

                for (int i = 0; i < newHomeCells.Length; i++)
                {
                    homeCellData.Add(newHomeCells[i], newHomeCellData);
                }

                tilemap.SetTiles(newHomeCells, homeTiles);
                countSize += houseSize;
            }
        }

        public Vector3Int GetRandomHomeEntryCell()
        {
            return homeCells[UnityEngine.Random.Range(0, homeCells.Count)];
        }

        public Vector3 GetRandomChildSpawnWorld()
        {
            Vector3Int randomHomeEntry = GetRandomHomeEntryCell();
            return CellToRandomValidEntryPoint(randomHomeEntry);
        }

        public Vector3 GetRandomCopSpawnWorld()
        {
            Vector3Int[] possibleCells = new Vector3Int[] {
                new Vector3Int(0, 2, 0),
                new Vector3Int(2, 0, 0),
                new Vector3Int(LEVEL_DEFINITION.GetLength(0) - 3, 0, 0),
                new Vector3Int(LEVEL_DEFINITION.GetLength(0) - 1, 2, 0),

                new Vector3Int(0, LEVEL_DEFINITION.GetLength(1) - 3, 0),
                new Vector3Int(2, LEVEL_DEFINITION.GetLength(1) - 1, 0),
                new Vector3Int(LEVEL_DEFINITION.GetLength(0) - 3, LEVEL_DEFINITION.GetLength(1) - 1, 0),
                new Vector3Int(LEVEL_DEFINITION.GetLength(0) - 1, LEVEL_DEFINITION.GetLength(1) - 3, 0),
            };
            return tilemap.GetCellCenterWorld(possibleCells[UnityEngine.Random.Range(0, possibleCells.Length)]);
        }

        public Vector3 CellToRandomValidEntryPoint(Vector3Int homeEntry)
        {
            Vector3 randomHomeEntryWorld = tilemap.CellToWorld(homeEntry);
            float x = 0;
            float y = 0;
            // Oh boy here come the magic numbers.
            switch (homeCellData[homeEntry].direction)
            {
                case 0:
                    x = randomHomeEntryWorld.x + 32;
                    y = UnityEngine.Random.Range(randomHomeEntryWorld.y + 9, randomHomeEntryWorld.y + 24);
                    break;
                case 1:
                    x = UnityEngine.Random.Range(randomHomeEntryWorld.x + 9, randomHomeEntryWorld.x + 24);
                    y = randomHomeEntryWorld.y + 32;
                    break;
                case 2:
                    x = randomHomeEntryWorld.x;
                    y = UnityEngine.Random.Range(randomHomeEntryWorld.y + 9, randomHomeEntryWorld.y + 24);
                    break;
                case 3:
                    x = UnityEngine.Random.Range(randomHomeEntryWorld.x + 9, randomHomeEntryWorld.x + 24);
                    y = randomHomeEntryWorld.y;
                    break;
            }
            return new Vector3(x, y, 0);
        }

        public Queue<Vector3> FindPath(Vector3Int startCell, Vector3Int endCell)
        {
            int nodeIndex = nodes.FindIndex(x => x == endCell);
            // BFS BABYYYY
            Queue<Vector3Int> nodeQueue = new Queue<Vector3Int>();
            Queue<Vector3> foundPath = new Queue<Vector3>();
            Dictionary<Vector3Int, bool> visited = new Dictionary<Vector3Int, bool>();
            Dictionary<Vector3Int, Vector3Int> map = new Dictionary<Vector3Int, Vector3Int>();
            visited[endCell] = true;

            foreach (Vector3Int node in adjacentNodes[nodeIndex])
            {
                map[node] = endCell;
                nodeQueue.Enqueue(node);
            }
            while (nodeQueue.Count > 0)
            {
                Vector3Int nextNode = nodeQueue.Dequeue();
                bool isVisited = false;
                visited.TryGetValue(nextNode, out isVisited);
                if (!isVisited)
                {
                    visited[nextNode] = true;
                    int nextNodeIndex = nodes.FindIndex(x => x == nextNode);
                    foreach (Vector3Int nextAdjacentNode in adjacentNodes[nextNodeIndex])
                    {
                        bool isAdjacentVisited = false;
                        visited.TryGetValue(nextAdjacentNode, out isAdjacentVisited);
                        if (!isAdjacentVisited)
                        {
                            map[nextAdjacentNode] = nextNode;
                            if (nextAdjacentNode == startCell)
                            {
                                // WE DID IT
                                nodeQueue.Clear();
                            }
                            nodeQueue.Enqueue(nextAdjacentNode);
                        }
                    }
                }
            }

            // Build the path queue to return since we're done finding the path
            Vector3Int nextMappedCell = startCell;
            while(nextMappedCell != endCell)
            {
                // Apply some randomization to positions of road cells
                foundPath.Enqueue(tilemap.GetCellCenterWorld(map[nextMappedCell]) + ((tilemap.GetTile(nextMappedCell) is RoadTile) ? 
                    new Vector3(UnityEngine.Random.Range(-6, 7), UnityEngine.Random.Range(-6, 7), 0) : 
                    Vector3.zero
                ));
                nextMappedCell = map[nextMappedCell];
            }

            return foundPath;
        }
    }
}