using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Clown
{
    public class HomeEntryTile : Tile 
    {
        public Sprite tileSprite;
        public override void RefreshTile(Vector3Int cell, ITilemap tilemap)
        {
            // HomeEntryTile MUST only be called ONCE per cell. IT WILL NEVER BE CALLED MORE THAN ONCE PER CELL.
            MapManager.s.nodes.Add(cell);
            MapManager.s.adjacentNodes.Add(new List<Vector3Int>());
            int nodeIndex = MapManager.s.nodes.FindIndex(x => x == cell);
            tilemap.RefreshTile(cell);
            int direction = MapManager.s.homeCellData[cell].direction;
            Vector3Int newCell = new Vector3Int(cell.x + Constants.ORTHOGONAL_DIRECTIONS[direction,0], cell.y + Constants.ORTHOGONAL_DIRECTIONS[direction,1], cell.z);

            // Tile must be adjacent because you generated the home tile with a specific facing direction didn't you? yeah you did. skip the adjacency check
            int adjacentNodeIndex = MapManager.s.nodes.FindIndex(x => x == newCell);
            MapManager.s.adjacentNodes[nodeIndex].Add(newCell);
            MapManager.s.adjacentNodes[adjacentNodeIndex].Add(cell);
            tilemap.RefreshTile(newCell);
        }

        public override void GetTileData(Vector3Int cell, ITilemap tilemap, ref TileData tileData)
        {
            // House sizes ARE ONLY 2, but they may have some fence tiles to pad to the nearest road.
            tileData.sprite = tileSprite;
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation(MapManager.s.homeCellData[cell].direction), Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;
        }

        private Quaternion GetRotation(int direction)
        {
            return Quaternion.Euler(0f, 0f, 90f * direction);
        }

        #if UNITY_EDITOR
        // The following is a helper that adds a menu item to create a HomeEntryTile Asset
        [MenuItem("Assets/Create/HomeEntryTile")]
        public static void CreateHomeEntryTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save HomeEntry Tile", "New HomeEntry Tile", "Asset", "Save HomeEntry Tile", "Assets");
            if (path == "")
            {
                return;
            }
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<HomeEntryTile>(), path);
        }
        #endif
    }
}