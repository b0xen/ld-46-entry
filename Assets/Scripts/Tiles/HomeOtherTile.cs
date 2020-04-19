using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Clown
{
    public class HomeOtherTile : Tile 
    {
        public Sprite[] tileSprites;

        public override void GetTileData(Vector3Int cell, ITilemap tilemap, ref TileData tileData)
        {
            int spriteIndex = 0;
            // This shit better exist before you try this
            int spriteDirection = MapManager.s.homeCellData[cell].direction;
            Vector3Int vectorDirection = cell - MapManager.s.homeCellData[cell].cell;
            if (vectorDirection.x < 0)
            {
                if (vectorDirection.y < 0)
                {
                    spriteIndex = 1;
                }
                else if (vectorDirection.y == 0)
                {
                    if (spriteDirection > 0)
                    {
                        spriteIndex = 0;
                    }
                    else
                    {
                        spriteIndex = 2;
                    }
                }
                else
                {
                    spriteIndex = 1;
                }
            }
            else if (vectorDirection.x == 0)
            {
                if (vectorDirection.y < 0)
                {
                    if (spriteDirection == 1)
                    {
                        spriteIndex = 2;
                    }
                    else
                    {
                        spriteIndex = 0;
                    }
                }
                else
                {
                    if (spriteDirection == 0)
                    {
                        spriteIndex = 0;
                    }
                    else
                    {
                        spriteIndex = 2;
                    }
                }
            }
            else
            {
                if (vectorDirection.y < 0)
                {
                    spriteIndex = 1;
                }
                else if (vectorDirection.y == 0)
                {
                    if (spriteDirection == 2)
                    {
                        spriteIndex = 2;
                    }
                    else 
                    {
                        spriteIndex = 0;
                    }
                }
                else
                {
                    spriteIndex = 1;
                }
            }

            // House sizes ARE ONLY 2, but they may have some fence tiles to pad to the nearest road.
            tileData.sprite = tileSprites[spriteIndex];
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation((byte) spriteDirection), Vector3.one);
            tileData.transform = m;

            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;
        }
        private Quaternion GetRotation(byte direction)
        {
            switch (direction)
            {
                case 1: return Quaternion.Euler(0f, 0f, 90f);
                case 2: return Quaternion.Euler(0f, 0f, 180f);
                case 3: return Quaternion.Euler(0f, 0f, 270f);
            }
            return Quaternion.Euler(0f, 0f, 0f);
        }
        #if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a HomeOtherTile Asset
        [MenuItem("Assets/Create/HomeOtherTile")]
        public static void CreateHomeOtherTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save HomeOther Tile", "New HomeOther Tile", "Asset", "Save HomeOther Tile", "Assets");
            if (path == "")
            {
                return;
            }
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<HomeOtherTile>(), path);
        }
        #endif
    }
}