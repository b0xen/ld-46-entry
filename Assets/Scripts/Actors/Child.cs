using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public class Child : Mob
    {
        [NonSerialized] public bool isGrabbed = false;

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
            if (isGrabbed)
            {
                MoveTowards((transform.position - playerTransform.position).normalized, false);
                return;
            }

            // Random wiggling
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-.2f, .2f), UnityEngine.Random.Range(-.2f, .2f), 0);
            // STOP COLLIDING WITH EACH OTHER MORONS LMAO
            float smallestDistance = 10;
            Vector3 antiCollisionVector = Vector3.zero;
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0) continue;
                float angle = Mathf.Deg2Rad * ((45 * i) + Vector3.SignedAngle(Vector3.right, movePosition - transform.position, Vector3.forward));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 10);
                if (hit.collider != null && (hit.transform.GetComponent<Player>() || hit.transform.GetComponent<Mob>()))
                {
                    if (hit.distance < smallestDistance)
                    {
                        smallestDistance = hit.distance;
                        // Repulsion calculation as inverse of distance
                        float newAngle = Mathf.Deg2Rad * ((-180 + (45 * i)) + Vector3.SignedAngle(Vector3.right, new Vector3(hit.point.x, hit.point.y) - transform.position, Vector3.forward));
                        antiCollisionVector = ((1 / hit.distance) * new Vector3(Mathf.Cos(newAngle), Mathf.Sin(newAngle), 0)).normalized;
                    }
                }
            }
            offset += antiCollisionVector;
            
            if (Vector3.Distance(transform.position, targetPosition) < 1)
            {
                // You're donezo kid
                GameManager.s.mobsToClear.Add(this);
            }
            else if (pathNodes.Count == 0 && movePosition != targetPosition)
            {
                movePosition = targetPosition;
            }
            else if (
                // You're on the last node and close enough so fuck it
                (pathNodes.Count == 1 && Vector3.Distance(movePosition, transform.position) < 10) ||
                // You're not on the last node and you need to get closer
                (Vector3.Distance(movePosition, transform.position) < 5 && pathNodes.Count > 0)
            )
            {
                movePosition = pathNodes.Dequeue();
            }
            MoveTowards(((movePosition - transform.position).normalized + offset).normalized);
        }
    }
}