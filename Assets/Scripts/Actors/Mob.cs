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
        public Vector3 targetPosition;
        [NonSerialized] public Vector3Int targetCell;
        [NonSerialized] public Vector3 movePosition;
        [NonSerialized] public Queue<Vector3> pathNodes;
        [NonSerialized] public Player player;
        public float baseSpeed;
        [NonSerialized] public float currentSpeed;
        [NonSerialized] public Collider2D mobCollider;
        [NonSerialized] public Color originalColor;
        [NonSerialized] public Color lightenedColor;

        void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            currentSpeed = baseSpeed;
            mobCollider = GetComponent<Collider2D>();
            player = FindObjectOfType<Player>();
            originalColor = GetComponent<SpriteRenderer>().color;
            lightenedColor = new Color(originalColor.r * 1.5f, originalColor.g * 1.5f, originalColor.b * 1.5f);
        }

        void Start()
        {
            GameManager.s.mobs.Add(this);
            currentCell = homeCell = MapManager.s.tilemap.WorldToCell(transform.position);
            do
            {
                // Time to traverse the nodes
                SetTargetAndPath();
            }
            while (pathNodes.Count == 0);
            movePosition = pathNodes.Dequeue();
            DoOnStart();
        }

        public abstract Vector3 GetTargetPosition();

        public abstract void MoveMob();

        public virtual void DoOnStart() {}

        public virtual void SetTargetAndPath()
        {
            targetPosition = GetTargetPosition();
            targetCell = MapManager.s.tilemap.WorldToCell(targetPosition);
            pathNodes = MapManager.s.FindPath(homeCell, targetCell);
        }

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