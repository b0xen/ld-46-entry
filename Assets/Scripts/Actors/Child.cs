using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public class Child : Mob
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override Vector3Int GetTargetCell()
        {
            int targetCellIndex;
            do
            {
                targetCellIndex = UnityEngine.Random.Range(0, MapManager.s.homeCells.Count);
            }
            while (MapManager.s.homeCells[targetCellIndex] != homeCell);

            return MapManager.s.homeCells[targetCellIndex];
        }

        public override void MoveMob()
        {
            
        }
    }
}