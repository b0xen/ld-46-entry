using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public class Cop : Mob
    {
        public override void DoOnStart()
        {
            // Repath every two seconds
            InvokeRepeating("SetTargetAndPath", .2f, .2f);
        }

        public override Vector3 GetTargetPosition()
        {
            return player.transform.position;
        }

        public override void SetTargetAndPath()
        {
            targetPosition = GetTargetPosition();
            targetCell = MapManager.s.tilemap.WorldToCell(targetPosition);
            pathNodes = MapManager.s.FindPath(currentCell, targetCell);
        }

        public override void MoveMob()
        {
            bool teleport = true;
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
                if (hit.collider != null && hit.transform.GetComponent<Mob>())
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
            
            if (mobCollider.IsTouching(player.GetComponent<Collider2D>()))
            {
                GameManager.s.DamagePlayer();
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= 32)
            {
                movePosition = player.transform.position;
                teleport = false;
            }
            else if (Vector3.Distance(movePosition, transform.position) < 5 && pathNodes.Count > 0)
            {
                movePosition = pathNodes.Dequeue();
            }
            MoveTowards(((movePosition - transform.position).normalized + offset).normalized, teleport);
        }
    }
}