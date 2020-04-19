using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public abstract class Mob : MonoBehaviour
    {
        int unstuckCounter = 0;
        [NonSerialized] public Vector3Int homeCell;
        [NonSerialized] public Vector3Int currentCell;
        [NonSerialized] public Rigidbody2D rb2D;

        [NonSerialized] public Vector3 lastPosition;
        [NonSerialized] public Vector3 targetPosition;
        [NonSerialized] public Vector3Int targetCell;
        [NonSerialized] public Vector3 movePosition;
        [NonSerialized] public Queue<Vector3> pathNodes;
        [NonSerialized] public Transform playerTransform;
        public float baseSpeed;
        [NonSerialized] public float currentSpeed;

        void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            currentSpeed = baseSpeed;
        }

        void Start()
        {
            GameManager.s.mobs.Add(this);
            currentCell = homeCell = MapManager.s.tilemap.WorldToCell(transform.position);
            targetPosition = GetTargetPosition();
            targetCell = MapManager.s.tilemap.WorldToCell(targetPosition);

            // Time to traverse the nodes
            pathNodes = MapManager.s.FindPath(homeCell, targetCell);
            movePosition = pathNodes.Dequeue();
        }

        public abstract Vector3 GetTargetPosition();

        public abstract void MoveMob();

        protected void MoveTowards(Vector3 direction, bool unstuck = true)
        {
            if (unstuck)
            {
                if (Vector3.Distance(lastPosition, transform.position) <= .1)
                {
                    unstuckCounter += 1;
                }
                else
                {
                    unstuckCounter = 0;
                }
                if (unstuckCounter > 20)
                {
                    transform.position = movePosition;
                    unstuckCounter = 0;
                }
            }
            float moveX = transform.position.x + (direction.x * currentSpeed * Time.deltaTime);
            float moveY = transform.position.y + (direction.y * currentSpeed * Time.deltaTime);
            lastPosition = transform.position;
            rb2D.MovePosition(new Vector2(moveX, moveY));
            currentCell = MapManager.s.tilemap.WorldToCell(transform.position);
        }
    }
}