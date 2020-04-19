using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Clown
{
    public class SewerTile : Tile 
    {
        // Most of this file is copy pasted from Unity's example on scriptable tiles.

        public Sprite[] tileSprites;

        public override void RefreshTile(Vector3Int cell, ITilemap tilemap)
        {
            tilemap.RefreshTile(cell);
            for (int i = 0; i < 4; i++)
            {
                Vector3Int newCell = new Vector3Int(cell.x + Constants.ORTHOGONAL_DIRECTIONS[i,0], cell.y + Constants.ORTHOGONAL_DIRECTIONS[i,1], cell.z);
                Tile newTile = tilemap.GetTile(newCell) as Tile;
                if (newTile is RoadTile || newTile is SewerTile)
                {
                    tilemap.RefreshTile(newCell);
                }
            }
        }
        
        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            int mask = HasValidAdjacentTile(tilemap, location + new Vector3Int(1, 0, 0), 2) ? 1 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(0, 1, 0), 3) ? 2 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(-1, 0, 0), 0) ? 4 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(0, -1, 0), 1) ? 8 : 0;
            int index = GetIndex((byte)mask);
            tileData.sprite = tileSprites[index];
            if (!MapManager.s.sewerCellData.ContainsKey(location))
            {
                Vector3 sewerPosition = GetSewerPosition((byte) mask, location);
                MapManager.s.sewerCellData.Add(location, sewerPosition);
            }
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
            return adjacentTile is RoadTile || adjacentTile is SewerTile || adjacentTile is VoidTile || (adjacentTile is HomeEntryTile && MapManager.s.homeCellData[position].direction == direction);
        }

        private Vector3 GetSewerPosition(byte mask, Vector3Int location)
        {
            Vector3 sewerPosition = MapManager.s.tilemap.CellToWorld(location);
            Vector3 offset = Vector3.zero;
            switch(mask)
            {
                case 1:
                case 4:
                case 5:
                    offset = new Vector3(9, 31, 0);
                    break;
                case 2:
                case 8: 
                case 10:
                    offset = new Vector3(31, 22, 0);
                    break;
                case 3:
                    offset = new Vector3(20, 0, 0);
                    break;
                case 6:
                    offset = new Vector3(31, 20, 0);
                    break;
                case 9:
                    offset = new Vector3(0, 11, 0);
                    break;
                case 12:
                    offset = new Vector3(11, 31, 0);
                    break;
                case 7:
                    offset = new Vector3(11, 0, 0);
                    break;
                case 11:
                    offset = new Vector3(0, 20, 0);
                    break;
                case 13:
                    offset = new Vector3(20, 31, 0);
                    break;
                case 14:
                    offset = new Vector3(31, 11, 0);
                    break;
            }
            return sewerPosition + offset;
        }

        // The following determines which sprite to use based on the number of adjacent SewerTiles
        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 1:
                case 2:
                case 4:
                case 8: 
                case 5:
                case 10: return 0;
                case 3:
                case 6:
                case 9:
                case 12: return 1;
                case 7:
                case 11:
                case 13:
                case 14: return 2;
            }
            return -1;
        }
        // The following determines which rotation to use based on the positions of adjacent SewerTiles
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
        // The following is a helper that adds a menu item to create a SewerTile Asset
        [MenuItem("Assets/Create/SewerTile")]
        public static void CreateSewerTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "Asset", "Save Road Tile", "Assets");
            if (path == "")
            {
                return;
            }
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SewerTile>(), path);
        }
        #endif
    }
}