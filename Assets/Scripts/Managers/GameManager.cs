using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

namespace Clown
{
    public class GameManager : MonoBehaviour
    {
        [NonSerialized] public static GameManager s;
        [NonSerialized] public GameObject playerObject;

        public GameObject playerPrefab;
        public Camera ppCamera;
        public List<Mob> mobs;
        public int level;

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
            mobs = new List<Mob>();
            ppCamera = FindObjectOfType<Camera>();
            level = 0;
        }

        void Start()
        {
            playerObject = Instantiate(playerPrefab, MapManager.s.tilemap.GetCellCenterWorld(new Vector3Int(2, 2, 0)), Quaternion.identity);
            DontDestroyOnLoad(playerObject);

            MapManager.s.CreateMap(level);
            ppCamera.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -5);
            ppCamera.transform.SetParent(playerObject.transform);
        }

        void Update()
        {
            foreach (Mob mob in mobs)
            {
                mob.MoveMob();
            }
        }
    }
}