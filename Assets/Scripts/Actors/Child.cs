using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public class Child : Mob
    {
        public override Vector3 GetTargetPosition()
        {
            int targetCellIndex = UnityEngine.Random.Range(0, MapManager.s.homeCells.Count);
            Vector3Int targetCell = MapManager.s.homeCells[targetCellIndex];
            while (targetCell == homeCell)
            {
                targetCellIndex = UnityEngine.Random.Range(0, MapManager.s.homeCells.Count);
                targetCell = MapManager.s.homeCells[targetCellIndex];
            }

            return MapManager.s.CellToRandomValidEntryPoint(targetCell);
        }

        public override void MoveMob()
        {
            if (Vector3.Distance(transform.position, targetPosition) < .5)
            {
                // You're donezo kid
                GameManager.s.mobsToClear.Add(this);
            }
            else if (pathNodes.Count == 0)
            {
                movePosition = targetPosition;
            }
            else if ((transform.position - movePosition).sqrMagnitude < 5 && pathNodes.Count > 0)
            {
                movePosition = pathNodes.Dequeue();
            }
            Move();
        }
    }
}