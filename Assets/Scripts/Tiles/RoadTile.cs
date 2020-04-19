using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Clown
{
    public class RoadTile : Tile 
    {
        // Most of this file is copy pasted from Unity's example on scriptable tiles.

        public Sprite[] tileSprites;
        public override void RefreshTile(Vector3Int cell, ITilemap tilemap)
        {
            if (!MapManager.s.nodes.Exists(x => x == cell))
            {
                MapManager.s.nodes.Add(cell);
                MapManager.s.adjacentNodes.Add(new List<Vector3Int>());
            }
            int nodeIndex = MapManager.s.nodes.FindIndex(x => x == cell);
            tilemap.RefreshTile(cell);
            for (int i = 0; i < 4; i++)
            {
                Vector3Int newCell = new Vector3Int(cell.x + Constants.ORTHOGONAL_DIRECTIONS[i,0], cell.y + Constants.ORTHOGONAL_DIRECTIONS[i,1], cell.z);
                if (HasValidAdjacentTile(tilemap, newCell, (i + 2) % 4)) 
                {
                    int adjacentNodeIndex = MapManager.s.nodes.FindIndex(x => x == newCell);
                    if (!MapManager.s.adjacentNodes[nodeIndex].Exists(x => x == newCell))
                    {
                        MapManager.s.adjacentNodes[nodeIndex].Add(newCell);
                    }
                    if (!MapManager.s.adjacentNodes[adjacentNodeIndex].Exists(x => x == cell))
                    {
                        MapManager.s.adjacentNodes[adjacentNodeIndex].Add(cell);
                    }
                }
                if (tilemap.GetTile(newCell) == this)
                {
                    tilemap.RefreshTile(newCell);
                }
            }
        }
        // This determines which sprite is used based on the RoadTiles that are adjacent to it and rotates it to fit the other tiles.
        // As the rotation is determined by the RoadTile, the TileFlags.OverrideTransform is set for the tile.
        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            int mask = HasValidAdjacentTile(tilemap, location + new Vector3Int(1, 0, 0), 2) ? 1 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(0, 1, 0), 3) ? 2 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(-1, 0, 0), 0) ? 4 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(0, -1, 0), 1) ? 8 : 0;
            int index = GetIndex((byte)mask);
            tileData.sprite = tileSprites[index];
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation((byte) mask), Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;
        }
        private bool HasValidAdjacentTile(ITilemap tilemap, Vector3Int position, int direction)
        {
            // direction is the direction that HomeEntryTile would have to face for it to be compatible
            Tile adjacentTile = tilemap.GetTile(position) as Tile;
            return adjacentTile == this || (adjacentTile is HomeEntryTile && MapManager.s.homeCellData[position].direction == direction);
        }
        // The following determines which sprite to use based on the number of adjacent RoadTiles
        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 0: return 0;
                case 1:
                case 2:
                case 4:
                case 8: 
                case 5:
                case 10: return 1;
                case 3:
                case 6:
                case 9:
                case 12: return 2;
                case 7:
                case 11:
                case 13:
                case 14: return 3;
                case 15: return 4;
            }
            return -1;
        }
    // The following determines which rotation to use based on the positions of adjacent RoadTiles
        private Quaternion GetRotation(byte mask)
        {
            switch (mask)
            {
                case 2:
                case 3:
                case 8:
                case 10:
                case 11: return Quaternion.Euler(0f, 0f, -90f);
                case 9:
                case 13: return Quaternion.Euler(0f, 0f, -180f);
                case 12:
                case 14: return Quaternion.Euler(0f, 0f, -270f);
            }
            return Quaternion.Euler(0f, 0f, 0f);
        }
    #if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
        [MenuItem("Assets/Create/RoadTile")]
        public static void CreateRoadTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "Asset", "Save Road Tile", "Assets");
            if (path == "")
            {
                return;
            }
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoadTile>(), path);
        }
    #endif
    }
}