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
        public GameObject ppCameraPrefab;

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
        }

        void Start()
        {
            playerObject = Instantiate(playerPrefab, new Vector3(16, 16, 0), Quaternion.identity);
            DontDestroyOnLoad(playerObject);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}