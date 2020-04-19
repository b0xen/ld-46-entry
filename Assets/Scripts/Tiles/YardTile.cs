using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Clown
{
    public class YardTile : Tile 
    {
        public Sprite[] tileSprites;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            int mask = HasValidAdjacentTile(tilemap, location + new Vector3Int(1, 0, 0)) ? 1 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(0, 1, 0)) ? 2 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(-1, 0, 0)) ? 4 : 0;
            mask += HasValidAdjacentTile(tilemap, location + new Vector3Int(0, -1, 0)) ? 8 : 0;
            int index = GetIndex((byte)mask);
            tileData.sprite = tileSprites[index];
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation((byte) mask), Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;
        }
        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 0:
                return 0;
                case 1:
                case 2:
                case 4:
                case 8:
                return 1;
                case 3:
                case 6:
                case 9:
                case 12:
                return 2;
            }
            return 0;
        }
        private Quaternion GetRotation(byte mask)
        {
            switch (mask)
            {
                case 2:
                case 6:
                return Quaternion.Euler(0f, 0f, 90f);
                case 4:
                case 12: 
                return Quaternion.Euler(0f, 0f, 180f);
                case 8:
                case 9: 
                return Quaternion.Euler(0f, 0f, 270f);
            }
            return Quaternion.Euler(0f, 0f, 0f);
        }
        private bool HasValidAdjacentTile(ITilemap tilemap, Vector3Int position)
        {
            Tile adjacentTile = tilemap.GetTile(position) as Tile;
            return adjacentTile is RoadTile;
        }
        #if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a YardTile Asset
        [MenuItem("Assets/Create/YardTile")]
        public static void CreateYardTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Yard Tile", "New Yard Tile", "Asset", "Save Yard Tile", "Assets");
            if (path == "")
            {
                return;
            }
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<YardTile>(), path);
        }
        #endif
    }
}