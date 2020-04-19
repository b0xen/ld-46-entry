using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clown
{
    public class Loader : MonoBehaviour
    {
        public GameObject gameManager;
        public GameObject mapManager;
        
        void Awake()
        {
            if (MapManager.s == null)
            {
                Instantiate(mapManager);
            }
            if (GameManager.s == null)
            {
                Instantiate(gameManager);
            }
        }
    }
}