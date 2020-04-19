using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public abstract class Mob : MonoBehaviour
    {
        [NonSerialized] public Vector3Int targetCell;
        [NonSerialized] public Vector3Int homeCell;

        void Start()
        {
            GameManager.s.mobs.Add(this);
            homeCell = MapManager.s.tilemap.WorldToCell(transform.position);
            targetCell = GetTargetCell();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        public abstract Vector3Int GetTargetCell();

        public abstract void MoveMob();
    }
}