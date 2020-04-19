using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Clown
{
    public class VoidTile : Tile 
    {
        public Sprite tileSprite;

        public override void GetTileData(Vector3Int cell, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = tileSprite;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;
        }

        #if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a VoidTile Asset
        [MenuItem("Assets/Create/VoidTile")]
        public static void CreateVoidTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Void Tile", "New Void Tile", "Asset", "Save Void Tile", "Assets");
            if (path == "")
            {
                return;
            }
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VoidTile>(), path);
        }
        #endif
    }
}