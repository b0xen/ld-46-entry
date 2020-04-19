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
        [NonSerialized] public List<Mob> mobs = new List<Mob>();
        [NonSerialized] public List<Mob> mobsToClear = new List<Mob>();

        public GameObject playerPrefab;
        public GameObject childPrefab;
        public GameObject copPrefab;
        public Camera ppCamera;

        public int level = 0;

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
            ppCamera = FindObjectOfType<Camera>();
        }

        void Start()
        {
            playerObject = Instantiate(playerPrefab, MapManager.s.tilemap.GetCellCenterWorld(new Vector3Int(2, 2, 0)), Quaternion.identity);
            DontDestroyOnLoad(playerObject);

            MapManager.s.CreateMap(level);
            ppCamera.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -5);
            ppCamera.transform.SetParent(playerObject.transform);

            InvokeRepeating("SpawnChild", .5f, .7f);
        }

        void SpawnChild()
        {
            Instantiate(childPrefab, MapManager.s.GetRandomChildSpawnWorld(), Quaternion.identity);
        }

        void FixedUpdate()
        {
            foreach (Mob mob in mobs)
            {
                mob.MoveMob();
            }
            foreach (Mob mob in mobsToClear)
            {
                mobs.Remove(mob);
                Destroy(mob.gameObject);
            }
            mobsToClear.Clear();
        }
    }
}