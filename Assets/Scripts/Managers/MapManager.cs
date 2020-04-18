using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Clown
{
    public class MapManager : MonoBehaviour
    {
        [NonSerialized] public static MapManager s;
        [NonSerialized] public Tilemap tilemap;

        public RoadTile roadTile;
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

        void Start()
        {
            tilemap.SetTile(new Vector3Int(0, 0, 0), roadTile);
            Debug.Log(tilemap.GetColliderType(new Vector3Int(0,0,0)));
            tilemap.SetTile(new Vector3Int(2, 0, 0), roadTile);
            tilemap.SetTile(new Vector3Int(0, 2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(1, 2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(2, 2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(2, 1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(3, 0, 0), roadTile);
            tilemap.SetTile(new Vector3Int(3, -1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(3, -2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(3, -3, 0), roadTile);
            tilemap.SetTile(new Vector3Int(3, -4, 0), roadTile);
            tilemap.SetTile(new Vector3Int(2, -4, 0), roadTile);
            tilemap.SetTile(new Vector3Int(1, -4, 0), roadTile);
            tilemap.SetTile(new Vector3Int(0, -4, 0), roadTile);
            tilemap.SetTile(new Vector3Int(0, -3, 0), roadTile);
            tilemap.SetTile(new Vector3Int(0, -2, 0), roadTile);
            tilemap.SetTile(new Vector3Int(-1, -1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(-1, 1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(1, 0, 0), roadTile);
            tilemap.SetTile(new Vector3Int(0, 1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(0, -1, 0), roadTile);
            tilemap.SetTile(new Vector3Int(-1, 0, 0), roadTile);
        }
    }
}