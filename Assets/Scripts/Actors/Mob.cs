using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Clown
{
    public abstract class Mob : MonoBehaviour
    {
        public Vector3 targetPosition;
        [NonSerialized] public Vector3Int homeCell;
        [NonSerialized] public Vector3Int currentCell;
        [NonSerialized] public Rigidbody2D rb2D;

        [NonSerialized] public Vector3 movePosition;
        [NonSerialized] public Queue<Vector3> pathNodes;
        public float speed;

        void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            GameManager.s.mobs.Add(this);
            currentCell = homeCell = MapManager.s.tilemap.WorldToCell(transform.position);
            targetPosition = GetTargetPosition();

            Vector3Int targetCell = MapManager.s.tilemap.WorldToCell(targetPosition);
            // Time to traverse the nodes
            pathNodes = MapManager.s.FindPath(homeCell, targetCell);
            movePosition = pathNodes.Dequeue();
        }

        public abstract Vector3 GetTargetPosition();

        public abstract void MoveMob();

        protected void Move()
        {
            Vector2 moveDirection = (movePosition - transform.position).normalized;
            float moveX = transform.position.x + (moveDirection.x * speed * Time.deltaTime);
            float moveY = transform.position.y + (moveDirection.y * speed * Time.deltaTime);
            rb2D.MovePosition(new Vector2(moveX, moveY));
        }
    }
}