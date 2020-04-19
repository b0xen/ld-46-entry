using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Clown
{
    public struct HomeCellData 
    {
        public HomeCellData(int _direction, Vector3Int _homeEntryCell)
        {
            direction = _direction;
            homeEntryCell = _homeEntryCell;
        }
        public int direction {get;}
        public Vector3Int homeEntryCell {get;}
    }
    public class MapManager : MonoBehaviour
    {
        [NonSerialized] public static MapManager s;
        [NonSerialized] public Tilemap tilemap;
        [NonSerialized] public List<Vector3Int> homeCells = new List<Vector3Int>();
        // Dear lord forgive me for this jank
        [NonSerialized] public Dictionary<Vector3Int, HomeCellData> homeCellData = new Dictionary<Vector3Int, HomeCellData>();
        [NonSerialized] public List<Vector3Int> nodes = new List<Vector3Int>();
        [NonSerialized] public List<List<Vector3Int>> adjacentNodes = new List<List<Vector3Int>>();

        public RoadTile roadTile;
        public YardTile yardTile;
        public VoidTile voidTile;
        public HomeEntryTile homeEntryTile;
        public HomeOtherTile homeOtherTile;

        public static int[,] LEVEL_SIZE = new int[,] {
            {21, 12},
            {25, 15}
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
            int maxX = LEVEL_SIZE[level, 0];
            int maxY = LEVEL_SIZE[level, 1];
            // First create the outer border and tile the inside with yard
            for (int x = -1; x <= maxX; x++)
            {
                for (int y = -1; y <= maxY; y++)
                {
                    if (x == -1 || x == maxX || y == -1 || y == maxY)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), voidTile);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), yardTile);
                    }
                }
            }

            // Create the inner loop. 
            for (int x = 2; x < maxX - 2; x++)
            {
                for (int y = 2; y < maxY - 2; y++)
                {
                    if (x == 2 || x == maxX - 3 || y == 2 || y == maxY - 3)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
                    }
                }
            }

            // Create exits to the outside where the po-po can spawn
            tilemap.SetTile(new Vector3Int(2, 0, 0), roadTile);
            tilemap.SetTile(new Vector3Int(2, 1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(maxX - 3, 0, 0), roadTile);
            tilemap.SetTile(new Vector3Int(maxX - 3, 1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(2, maxY - 2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(2, maxY - 1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(maxX - 3, maxY - 2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(maxX - 3, maxY - 1, 0), roadTile);

            // Set up the outer rows of houses
            PlaceHouses(ChooseHouseSizes(maxY), new Vector3Int(0, 0, 0), 0);
            PlaceHouses(ChooseHouseSizes(maxX - 6), new Vector3Int(3, 0, 0), 1);
            PlaceHouses(ChooseHouseSizes(maxY), new Vector3Int(maxX - 2, 0, 0), 2);
            PlaceHouses(ChooseHouseSizes(maxX - 6), new Vector3Int(3, maxY - 2, 0), 3);

            // Set up some inner rows...RANDOMLY
            PlaceHouses(ChooseHouseSizes(maxX - 6), new Vector3Int(3, 3, 0), 3);
            PlaceHouses(ChooseHouseSizes(maxX - 6), new Vector3Int(3, maxY - 5, 0), 1);
            PlaceHouses(ChooseHouseSizes(maxY - (6 + 4)), new Vector3Int(3, 3 + 2, 0), 2);
            PlaceHouses(ChooseHouseSizes(maxY - (6 + 4)), new Vector3Int(maxX - 5, 3 + 2, 0), 0);
        }

        private List<int> ChooseHouseSizes(int length)
        {
            // length must be >= 3
            int[] houseChance = new int[] {4, 4, 3, 3, 2};
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
                    // Ya fucked up this is a special case
                    houseSizes = new List<int>(){2, 2};
                }
                else
                {
                    houseSizes[houseIndex] = houseSizes[houseIndex] + 1;
                }
            }

            return houseSizes;
        }

        private void PlaceHouses(List<int> houseSizes, Vector3Int startingCell, int direction)
        {
            // Assumes there's at least a 2x2 space to hold the houses.
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
                Vector3Int[] homeCells = new Vector3Int[] {};
                if (direction % 2 == 0)
                {
                     homeCells = new Vector3Int[] {
                        new Vector3Int(startingCell.x, newStart, 0),
                        new Vector3Int(startingCell.x + 1, newStart, 0),
                        new Vector3Int(startingCell.x, newStart + 1, 0),
                        new Vector3Int(startingCell.x + 1, newStart + 1, 0),
                    };
                }
                else 
                {
                    homeCells = new Vector3Int[] {
                        new Vector3Int(newStart, startingCell.y, 0),
                        new Vector3Int(newStart, startingCell.y + 1, 0),
                        new Vector3Int(newStart + 1, startingCell.y, 0),
                        new Vector3Int(newStart + 1, startingCell.y + 1, 0),
                    };
                }
                HomeCellData newHomeCellData = new HomeCellData(direction, new Vector3Int(startingCell.x, newStart, 0));
                Tile[] homeTiles = new Tile[]{};

                switch (direction)
                {
                    case 0:
                        newHomeCellData = new HomeCellData(direction, new Vector3Int(startingCell.x + 1, newStart, 0));
                        homeTiles = new Tile[]{homeOtherTile, homeEntryTile, homeOtherTile, homeOtherTile};
                        break;
                    case 1:
                        newHomeCellData = new HomeCellData(direction, new Vector3Int(newStart + 1, startingCell.y + 1, 0));
                        homeTiles = new Tile[]{homeOtherTile, homeOtherTile, homeOtherTile, homeEntryTile};
                        break;
                    case 2:
                        newHomeCellData = new HomeCellData(direction, new Vector3Int(startingCell.x, newStart + 1, 0));
                        homeTiles = new Tile[]{homeOtherTile, homeOtherTile, homeEntryTile, homeOtherTile};
                        break;
                    case 3:
                        newHomeCellData = new HomeCellData(direction, new Vector3Int(newStart, startingCell.y, 0));
                        homeTiles = new Tile[]{homeEntryTile, homeOtherTile, homeOtherTile, homeOtherTile};
                        break;
                }

                for (int i = 0; i < homeCells.Length; i++)
                {
                    homeCellData.Add(homeCells[i], newHomeCellData);
                }

                tilemap.SetTiles(homeCells, homeTiles);
                countSize += houseSize;
            }
        }
    }
}